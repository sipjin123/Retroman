using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Advertisements;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.Popup;
    using Sandbox.Services;
    using Sandbox.UnityAds;

    public enum CustomAdType
    {
        Interstitial,
        Reward,
        Revive,
        ShopCoin
    }

    public static class GraphQLAdsFsmStates
    {
        public static readonly string FSM_NAME = "AdManager";
        public static readonly string LOGIN = "LOGIN";
        public static readonly string LOGIN_FAILED = "LOGIN_FAILED";
        public static readonly string FETCH_ADS_LIST = "FETCH_ADS_LIST";
        public static readonly string FETCH_ADS_FAILED = "FETCH_ADS_FAILED";
        public static readonly string ADS_READY = "ADS_READY";
        public static readonly string CACHE_ADS = "CACHE_ADS";
    }

    public static class GraphQLAdsFsmTransitions
    {
        public static readonly string ATTEMPT_LOGIN = "ATTEMPT_LOGIN";
        public static readonly string LOGIN_SUCCESSFUL = "LOGIN_SUCCESSFUL";
        public static readonly string LOGIN_FAILED = "LOGIN_FAILED";
        public static readonly string ATTEMPT_FETECH_AD_DATA = "ATTEMPT_FETECH_AD_DATA";
        public static readonly string FETCH_AD_DATA_SUCCESSFUL = "FETCH_AD_DATA_SUCCESSFUL";
        public static readonly string FETCH_AD_DATA_FAILED = "FETCH_AD_DATA_FAILED";
        public static readonly string CACHE_DATA_COMPLETE = "CACHE_DATA_COMPLETE";
    }

    public struct PlayAdRequestSignal
    {
        public ObscuredBool IsSkippable;
        public CustomAdType CustomAdType;
        public UnityAds.AdReward FallbackAdType;
    }

    public class CustomAdAvailableSignal
    {
        public CustomAdType Type;
        public Action<ObscuredBool> CallBack;
    }

    public struct CustomAdUnavailableSignal { }

    public struct AdFinishedPlayingSignal
    {
        public ObscuredString ad_id;
        public ObscuredBool was_skipped;
        public ObscuredFloat timemark;
        public CustomAdType ad_type;
    }

    [Serializable]
    public class PendingTransactions
    {
        public AdvertisementPlay Advertisment;
        public bool WasSkipped;
        public float TimeMark;
    }

    public class GraphQLDataManager : BaseService
    {
        [SerializeField]
        private GraphQLSetupData GraphQLSetupData;

        [SerializeField]
        private GraphQLAdDownloader GraphQLAdDownloader;

        [SerializeField]
        private GraphQLOfflineData GraphQLOfflineData;

        private Fsm AdManagerFsm;

        private Dictionary<GraphQLRequestType, Action<GraphQLRequestFailedSignal>> FailedActionMap = new Dictionary<GraphQLRequestType, Action<GraphQLRequestFailedSignal>>();
        private Dictionary<GraphQLRequestType, Action<GraphQLRequestSuccessfulSignal>> SuccessActionMap = new Dictionary<GraphQLRequestType, Action<GraphQLRequestSuccessfulSignal>>();
        private PopupCollectionRoot PopupCollectionRoot;
        
        private Coroutine PendingTransactions_CR;

        private ObscuredString Token;
        public List<Advertisement> AllOldAds;
        public List<Advertisement> AllNewAds;
        public List<Advertisement> AdsToServe;

        public AdvertisementPlay CurrAdPlayTransaction;
        public List<PendingTransactions> PendingTransactions;
        public List<PendingTransactions> FailedTransactions;
        public List<PendingTransactions> RecentTransactions;

        private ObscuredInt CurrAd; // save
        private ObscuredInt InterstitialAdsServed;
        private ObscuredInt RewardAdsServed;
        private ObscuredBool SendEndAdEvent;
        
        #region IService
        public override void InitializeService()
        {
            #region Init Fields
            SendEndAdEvent = false;
            CurrAd = 0;
            #endregion

            #region Setup
            GraphQLOfflineData.Initialize();
            FailedTransactions = new List<PendingTransactions>(GraphQLOfflineData.GetPendingTransactionList());

            if (GraphQLOfflineData.AdsPlayData.DateOfService.Date != DateTime.Now.Date)
            {
                InterstitialAdsServed = 0;
                RewardAdsServed = 0;
            }
            else
            {
                InterstitialAdsServed = GraphQLOfflineData.AdsPlayData.InterstitialAds;
                RewardAdsServed = GraphQLOfflineData.AdsPlayData.RewardAds;
            }
            SetupResolversForSignals();
            SetupFsm();
            #endregion

            #region Receivers
            this.Receive<PlayAdRequestSignal>().Subscribe(_ =>
            {
                if (DebugUseUnityAds)
                {
                    this.Publish(new ShowUnityAdsSignal() { isRewardedVideo = !_.IsSkippable, rewardType = _.FallbackAdType });
                    return;
                }


                if (
                    (AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.CACHE_ADS) // in process of caching ads. assumed online
                    || AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.ADS_READY) // assummed all ads cached. assumed online
                    ||(AllOldAds != null) //local cache exists  
                    )
                    && AdsToServe.Count > 0                   
                   )
                {
                    //check if chosen ad to play is already cached
                    if (GraphQLAdDownloader.AdDownloaded(AdsToServe[CurrAd].id))
                    {
                        bool ServeCustomAd = false;
                        if (_.IsSkippable && InterstitialAdsServed < GraphQLSetupData.MaxInterstitialAdsPerDay)
                        {
                            ServeCustomAd = true;
                            InterstitialAdsServed++;
                        }
                        else if (!_.IsSkippable && RewardAdsServed < GraphQLSetupData.MaxRewardAdsPerDay)
                        {
                            ServeCustomAd = true;
                            RewardAdsServed++;
                        }

                        if (ServeCustomAd)
                        {
                            GraphQLAdData AdData = new GraphQLAdData();
                            //check if curradd is within ads to serve count
                            AdData.AdId = AdsToServe[CurrAd].id;
                            AdData.AdPath = GraphQLAdDownloader.GetAdLocalPath(AdsToServe[CurrAd].id);
                            AdData.IsSkippable = _.IsSkippable;
                            AdData.AdType = _.CustomAdType;
                            switch (AdsToServe[CurrAd].GetAdType())
                            {
                                case AdType.image:
                                    AdData.Skiptime = GraphQLSetupData.SkipTimeForImageAds;
                                    AdData.EndTime = GraphQLSetupData.EndTimeForImageAds;
                                    this.Publish(new OnShowPopupSignal() { Popup = Popup.CustomAdPopupImage, PopupData = new PopupData() { Data = AdData }});

                                    Debug.LogFormat(D.L("[ADS]") + "GraphQLDataManager::OnShowPopupSignal CustomAdPopupImage\n");
                                    break;
                                case AdType.video:
                                    AdData.Skiptime = GraphQLSetupData.SkipTimeForVideoAds;
                                    this.Publish(new OnShowPopupSignal() { Popup = Popup.CustomAdPopupVideo, PopupData = new PopupData() { Data = AdData } });

                                    Debug.LogFormat(D.L("[ADS]") + "GraphQLDataManager::OnShowPopupSignal CustomAdPopupVideo\n");
                                    break;
                            }
                            this.Publish(new GraphQLPlayAdRequestSignal() { token = Token, ad_id = AdsToServe[CurrAd].id });
                            CurrAd = (CurrAd + 1) % AdsToServe.Count;
                            GraphQLOfflineData.SaveServicedAds(InterstitialAdsServed, RewardAdsServed, DateTime.Now);
                        }
                        else
                        {
                            this.Publish(new ShowUnityAdsSignal() { isRewardedVideo = !_.IsSkippable, rewardType = _.FallbackAdType });
                        }
                    }
                    else
                    {
                        //this.Publish(new ShowUnityAdsSignal() { isRewardedVideo = false, rewardType = UnityAds.AdReward.NoReward });
                        this.Publish(new ShowUnityAdsSignal() { isRewardedVideo = !_.IsSkippable, rewardType = _.FallbackAdType});
                    }
                }
                else
                {
                    // play unity ads
                    //this.Publish(new CustomAdUnavailableSignal());
                    this.Publish(new ShowUnityAdsSignal() { isRewardedVideo = !_.IsSkippable, rewardType = _.FallbackAdType});
                }
            }).AddTo(this);

            this.Receive<AdFinishedPlayingSignal>().Subscribe(_ => 
            {
                if (SendEndAdEvent)
                {
                    PendingTransactions.Add(new PendingTransactions
                    {
                        Advertisment = CurrAdPlayTransaction.ShallowCopy(),
                        TimeMark = _.timemark,
                        WasSkipped = _.was_skipped
                    });

                    this.Publish(new GraphQLEndAdSignal()
                    {
                        token = Token,
                        was_skipped = _.was_skipped,
                        timemark = _.timemark,
                        AdRequest = CurrAdPlayTransaction,
                    });
                }
            }).AddTo(this);

            this.Receive<CustomAdAvailableSignal>()
                .Where(_ => _.Type == CustomAdType.Interstitial)
                .Subscribe(_ =>
                {
                    _.CallBack
                    (
                        RewardAdsServed < GraphQLSetupData.MaxInterstitialAdsPerDay 
                        && (
                            AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.CACHE_ADS) // in process of caching ads. assumed online
                            || AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.ADS_READY) 
                            || (AllOldAds != null)
                           )
                        &&
                        AdsToServe.Count > 0
                    );
                }).AddTo(this);

            this.Receive<CustomAdAvailableSignal>()
                .Where(_ => _.Type == CustomAdType.Reward)
                .Subscribe(_ =>
                {
                    _.CallBack
                    (
                        RewardAdsServed < GraphQLSetupData.MaxRewardAdsPerDay 
                        && (
                            AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.CACHE_ADS) // in process of caching ads. assumed online
                            || AdManagerFsm.GetCurrentStateName().Equals(GraphQLAdsFsmStates.ADS_READY) 
                            || (AllOldAds != null)
                           )
                        &&
                        AdsToServe.Count > 0
                    );
                }).AddTo(this);
            #endregion

            Observable
                .FromCoroutine(FlushFailedTransaction)
                .RepeatSafe()
                .Subscribe()
                .AddTo(this);

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
        }
        #endregion

        #region Setup
        private void SetupFsm()
        {
            AdManagerFsm = new Fsm(GraphQLAdsFsmStates.FSM_NAME);

            #region FSM States
            FsmState Login = AdManagerFsm.AddState(GraphQLAdsFsmStates.LOGIN);
            FsmState LoginFailed = AdManagerFsm.AddState(GraphQLAdsFsmStates.LOGIN_FAILED);
            FsmState FetchAdsList = AdManagerFsm.AddState(GraphQLAdsFsmStates.FETCH_ADS_LIST);
            FsmState FetchAdsFailed = AdManagerFsm.AddState(GraphQLAdsFsmStates.FETCH_ADS_FAILED);
            FsmState CacheAds = AdManagerFsm.AddState(GraphQLAdsFsmStates.CACHE_ADS);
            FsmState AdsReady = AdManagerFsm.AddState(GraphQLAdsFsmStates.ADS_READY);
            #endregion
            
            #region FSM Actions
            Login.AddAction(new FsmDelegateAction(Login, _ => 
            {
                this.Publish(new GraphQLLoginRequestSignal() { unique_id = Platform.DeviceId });
            }));

            LoginFailed.AddAction(new FsmDelegateAction(LoginFailed, _ => 
            {
                StartCoroutine(WaitBeforeSendEvent(GraphQLSetupData.StartLoginRetryTimer, GraphQLAdsFsmTransitions.ATTEMPT_LOGIN));
            }));

            FetchAdsList.AddAction(new FsmDelegateAction(FetchAdsList, delegate 
            {
                AllOldAds = GraphQLOfflineData.GetOldAdList();
                this.Publish(new GraphQLGetAllAdsRequestSignal() { token = Token });
            }));

            FetchAdsFailed.AddAction(new FsmDelegateAction(FetchAdsFailed, delegate 
            {
                StartCoroutine(WaitBeforeSendEvent(GraphQLSetupData.StartLoginRetryTimer, GraphQLAdsFsmTransitions.ATTEMPT_FETECH_AD_DATA));
            }));

            CacheAds.AddAction(new FsmDelegateAction(CacheAds, delegate 
            {
                //request to check cached images/videos
                GraphQLAdDownloader.DownloadAds(AllNewAds);
                
            }, 
            delegate 
            {
                //when done, go to ads ready
                //if (GraphQLAdDownloader.DownloadComplete())
                //{
                //    AdManagerFsm.SendEvent(GraphQLAdsFsmTransitions.CACHE_DATA_COMPLETE);
                //}
            }));

            AdsReady.AddAction(new FsmDelegateAction(AdsReady, delegate
            {
                //this state ensures you can play any of the ads
                //HasCashedAllAds = true;
            }));
            #endregion

            #region FSM Transitions
            Login.AddTransition(GraphQLAdsFsmTransitions.LOGIN_SUCCESSFUL, FetchAdsList);
            Login.AddTransition(GraphQLAdsFsmTransitions.LOGIN_FAILED, LoginFailed);

            LoginFailed.AddTransition(GraphQLAdsFsmTransitions.ATTEMPT_LOGIN, Login);


            FetchAdsList.AddTransition(GraphQLAdsFsmTransitions.FETCH_AD_DATA_SUCCESSFUL, CacheAds);
            FetchAdsList.AddTransition(GraphQLAdsFsmTransitions.FETCH_AD_DATA_FAILED, FetchAdsFailed);

            FetchAdsFailed.AddTransition(GraphQLAdsFsmTransitions.ATTEMPT_FETECH_AD_DATA, FetchAdsList);

            CacheAds.AddTransition(GraphQLAdsFsmTransitions.CACHE_DATA_COMPLETE, AdsReady);
            #endregion

            #region FSM Start
            AdManagerFsm.Start(GraphQLAdsFsmStates.LOGIN);

            this.UpdateAsObservable()
                .Subscribe(_ => AdManagerFsm.Update())
                .AddTo(this);
            #endregion

        }

        private void SetupResolversForSignals()
        {
            SuccessActionMap.Add(GraphQLRequestType.LOGIN, LoginSuccessResolver);
            SuccessActionMap.Add(GraphQLRequestType.GET_ALL_ADS, GetAllAdsSuccessResolver);
            SuccessActionMap.Add(GraphQLRequestType.GET_RANDOM_AD, GetRandomAdsSuccessResolver);
            SuccessActionMap.Add(GraphQLRequestType.END_AD, AdEndSuccessResolver);
            SuccessActionMap.Add(GraphQLRequestType.PLAY_AD, PlayAdSuccessResolver);

            FailedActionMap.Add(GraphQLRequestType.LOGIN, LoginFailedResolver);
            FailedActionMap.Add(GraphQLRequestType.GET_ALL_ADS, GetAllAdsFailedResolver);
            FailedActionMap.Add(GraphQLRequestType.GET_RANDOM_AD, GetRandomAdFailedResolver);
            FailedActionMap.Add(GraphQLRequestType.END_AD, EndAdFailedResolver);
            FailedActionMap.Add(GraphQLRequestType.PLAY_AD, PlayAdFailedResolver);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => SuccessActionMap.ContainsKey(_.Type))
                .Subscribe(_ => SuccessActionMap[_.Type](_)).AddTo(this);

            this.Receive<GraphQLRequestFailedSignal>()
                .Where(_ => FailedActionMap.ContainsKey(_.Type))
                .Subscribe(_ => FailedActionMap[_.Type](_)).AddTo(this);
        }
        #endregion

        #region Failed Resolvers
        private void LoginFailedResolver(GraphQLRequestFailedSignal sig)
        {
            AdManagerFsm.SendEvent(GraphQLAdsFsmTransitions.LOGIN_FAILED);
        }


        private void GetAllAdsFailedResolver(GraphQLRequestFailedSignal sig)
        {
            if (AllOldAds != null)
            {
                //use old ads
                GraphQLAdDownloader.DownloadAds(AllOldAds);
                AdsToServe = AllOldAds;
            }
            AdManagerFsm.SendEvent(GraphQLAdsFsmTransitions.FETCH_AD_DATA_FAILED);
        }

        private void GetRandomAdFailedResolver(GraphQLRequestFailedSignal sig)
        {

        }

        private void PlayAdFailedResolver(GraphQLRequestFailedSignal sig)
        {
            SendEndAdEvent = false;
        }

        private void EndAdFailedResolver(GraphQLRequestFailedSignal sig)
        {
            if (SendEndAdEvent)
            {
                if (PendingTransactions.FirstOrDefault() != null)
                {
                    FailedTransactions.Add(PendingTransactions.FirstOrDefault());
                    GraphQLOfflineData.SaveFailedTransactions(FailedTransactions);
                    PendingTransactions.Remove(PendingTransactions.FirstOrDefault());
                }
            }
            if (RecentTransactions.Count > 0)
            {
                if (sig.HasData)
                {
                    PendingTransactions ResolvedFromFailed = RecentTransactions.Find(_ => _.Advertisment.id == sig.GetData<AdvertisementPlay>().id);
                    if (ResolvedFromFailed != null)
                    {
                        RecentTransactions.Remove(ResolvedFromFailed);
                        FailedTransactions.Add(ResolvedFromFailed);
                        GraphQLOfflineData.SaveFailedTransactions(FailedTransactions);
                    }
                }
                else
                {
                    Debug.LogError("No data from signal");
                }
            }
        }
        #endregion

        #region Success Resolvers
        private void LoginSuccessResolver(GraphQLRequestSuccessfulSignal sig)
        {
            Token = sig.GetData<ObscuredString>();
            AdManagerFsm.SendEvent(GraphQLAdsFsmTransitions.LOGIN_SUCCESSFUL);
        }

        private void GetAllAdsSuccessResolver(GraphQLRequestSuccessfulSignal sig)
        {
            AllNewAds = sig.GetData<List<Advertisement>>();
            if (AllOldAds != null)
            {
                //filter
                List<Advertisement> DeleteAds = new List<Advertisement>();
                AllOldAds.ForEach(old_ad => 
                {
                    if (AllNewAds.Any(new_ad => old_ad.id.Equals(new_ad.id)))
                    {
                        //delete
                    }
                });
            }
            AdsToServe = AllNewAds;
            if (CurrAd >= AllNewAds.Count)
            {
                CurrAd = 0;
            }
            AdManagerFsm.SendEvent(GraphQLAdsFsmTransitions.FETCH_AD_DATA_SUCCESSFUL);

        }

        private void PlayAdSuccessResolver(GraphQLRequestSuccessfulSignal sig)
        {
            SendEndAdEvent = true;
            CurrAdPlayTransaction = sig.GetData<AdvertisementPlay>();
        }

        private void GetRandomAdsSuccessResolver(GraphQLRequestSuccessfulSignal sig)
        { 

        }

        private void AdEndSuccessResolver(GraphQLRequestSuccessfulSignal sig)
        {
            PendingTransactions ResolvedFromPending = PendingTransactions.Find(_ => _.Advertisment.id == sig.GetData<AdvertisementEnd>().id);
            PendingTransactions ResolvedFromRecent = RecentTransactions.Find(_ => _.Advertisment.id == sig.GetData<AdvertisementEnd>().id);
            PendingTransactions ResolvedFromFailed = FailedTransactions.Find(_ => _.Advertisment.id == sig.GetData<AdvertisementEnd>().id);
            if (ResolvedFromPending != null)
            {
                PendingTransactions.Remove(ResolvedFromPending);
            }

            if (ResolvedFromRecent != null)
            {
                RecentTransactions.Remove(ResolvedFromRecent);
                GraphQLOfflineData.SaveFailedTransactions(FailedTransactions);
            }
            else if (ResolvedFromFailed != null)
            {
                FailedTransactions.Remove(ResolvedFromFailed);
                GraphQLOfflineData.SaveFailedTransactions(FailedTransactions);
            }
        }
        #endregion

        #region Coroutines and Streams
        private IEnumerator WaitBeforeSendEvent(float seconds, string fsm_event)
        {
            yield return new WaitForSeconds(seconds);
            AdManagerFsm.SendEvent(fsm_event);
        }

        private IEnumerator FlushFailedTransaction()
        {
            if (FailedTransactions.Count > 0)
            {
                PendingTransactions Top = FailedTransactions.FirstOrDefault();
                if (Top != null)
                {
                    FailedTransactions.Remove(Top);
                    this.Publish(new GraphQLEndAdSignal()
                    {
                        token = Token,
                        AdRequest = Top.Advertisment,
                        was_skipped = Top.WasSkipped,
                        timemark = Top.TimeMark
                    });
                    RecentTransactions.Add(Top);
                }
            }
            yield return new WaitForSeconds(GraphQLSetupData.FlushRequestsTimer);
        }
        #endregion

        #region Debug

        [SerializeField, ShowInInspector]
        bool DebugUseUnityAds = false;

        [SerializeField, ShowInInspector]
        bool DebugIsSkipable = false;

        [Button(25)]
        public void ShowCustomAdPopup()
        {
            this.Publish(new PlayAdRequestSignal()
            {
                IsSkippable = DebugIsSkipable
            });
        }


        [Button(25)]
        public void RequestAd()
        {
            this.Publish(new GraphQLPlayAdRequestSignal() { token = Token, ad_id = AllNewAds[CurrAd].id });
            CurrAd = (CurrAd + 1) % AllNewAds.Count;
        }

        [Button(25)]
        public void EndAd()
        {
            this.Publish(new GraphQLEndAdSignal() { token = Token, AdRequest = CurrAdPlayTransaction, was_skipped = false, timemark = 2.4f });
        }

        [SerializeField, ShowInInspector]
        public string DebugCurrFSMState
        {
            get
            {
                if (AdManagerFsm != null)
                {
                    return AdManagerFsm.GetCurrentStateName();
                }
                else
                {
                    return "Uninitialized";
                }
            }
        }
        #endregion

    }

}
