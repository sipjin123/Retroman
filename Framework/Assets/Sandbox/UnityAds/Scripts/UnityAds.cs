//#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using Common;
using Common.Signal;
using Common.Query;

using UniRx;

using Framework;

namespace Sandbox.UnityAds
{
    using Sandbox.Services;

    /// <summary>
    /// Handles display of unity ads.
    /// </summary>
	public class UnityAds : MonoBehaviour, IService
    {
        public enum AdReward
        {
            FreeCoins,
            ShopCoins,
            ContinuePlaying,
            NoReward
        }

        bool rewardSuccessful = false;
        bool fromAd = false;

        private BoolReactiveProperty _IsAdPlaying = new BoolReactiveProperty(false);
        public BoolReactiveProperty IsAdPlaying
        {
            get { return _IsAdPlaying; }
        }

        /// <summary>
        /// Holder for subscriptions to be disposed when the service is terminated.
        /// </summary>
        private CompositeDisposable TerminationDisposables = new CompositeDisposable();

        private AdReward RewardType;

        #region IService implementation
        [SerializeField, ShowInInspector]
        private bool _IsServiceRequired;
        public bool IsServiceRequired
        {
            get
            {
                return _IsServiceRequired;
            }
        }

        public string ServiceName { get { return name; } }

        private ReactiveProperty<ServiceState> _CurrentServiceState = new ReactiveProperty<ServiceState>(ServiceState.Uninitialized);
        public ReactiveProperty<ServiceState> CurrentServiceState
        {
            get
            {
                return _CurrentServiceState;
            }
        }

        BoolReactiveProperty isConnected = new BoolReactiveProperty(false);


        public void InitializeService()
        {
            isConnected = QuerySystem.Query<BoolReactiveProperty>(QueryIds.IsConnected);
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            this.Receive<ShowUnityAdsSignal>()
                .Subscribe(sig =>
                {
                    if (sig.isRewardedVideo)
                    {
                        RewardType = sig.rewardType;
                        ShowRewardedAd();

                    }
                    else
                    {
                        OnShowUnityAds(sig.Region);
                    }
                })
                .AddTo(TerminationDisposables);

            this.Receive<CheckUnityAdsAvailabiltySignal>()
               .Subscribe(_ =>
               {
#if UNITY_ADS
                    //this.Publish(new InternetCheckSignal());
                    bool hideButton = !Advertisement.IsReady("rewardedVideo") || Application.internetReachability == NetworkReachability.NotReachable;
                   this.Publish(new HideWatchAdsButtonSignal() { HideButton = hideButton });
#else
                    Debug.Log("Unity Ads Not Available.");
                    this.Publish(new HideWatchAdsButtonSignal() { HideButton = true });
#endif

                })
               .AddTo(TerminationDisposables);

            QuerySystem.RegisterResolver(QueryIds.IsUnityAdsPlaying, delegate (IQueryRequest req, IMutableQueryResult result)
            {
                _IsAdPlaying.Value = false;
#if UNITY_ADS
                _IsAdPlaying.Value = Advertisement.isShowing;

#endif
                result.Set(IsAdPlaying);
            });
#endif
            Initialize();
            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public void TerminateService()
        {
            // dispose all subscriptions and clear list
            QuerySystem.RemoveResolver(QueryIds.IsUnityAdsPlaying);
            TerminationDisposables.Clear();
        }
        #endregion

#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        private void Initialize()
        {
            string adUnitId = string.Empty;

            // mobile
            if (Platform.IsMobileAndroid())
            {
                adUnitId = Const.UNITY_GAME_ID_ANDROID;
            }
            else if (Platform.IsMobileIOS())
            {
                adUnitId = Const.UNITY_GAME_ID_IOS;
            }
            // editor
            else
            {
                // editor
#if UNITY_ANDROID
                adUnitId = Const.UNITY_GAME_ID_ANDROID;
#elif UNITY_IOS
				adUnitId = Const.UNITY_GAME_ID_IOS;
#else
				Assertion.Assert(false, "Unsupported Platform");
#endif
            }

#if UNITY_ADS
            if (!Advertisement.isInitialized)
            {
                Advertisement.Initialize(adUnitId);
            }
#endif
        }

        private void OnShowUnityAds(string region)
        {
#if UNITY_ADS
            if (!string.IsNullOrEmpty(region))
            {
                if (Advertisement.IsReady(region))
                {
                    ShowOptions options = new ShowOptions();
                    options.resultCallback = this.HandleUnrewardedAds;
                    Advertisement.Show(region, options);
                }
                else
                {
                    this.Publish(new HideWatchAdsButtonSignal() { HideButton = true });
                    //this.Publish(new UnityAdFailureSignal() { Type = AdReward.NoReward });
                }
            }
            else
            {
                if (Advertisement.IsReady())
                {
                    var options = new ShowOptions { resultCallback = HandleUnrewardedAds };
                    Advertisement.Show();
                }
            }
#endif
        }

        void ShowRewardedAd()
        {


#if UNITY_ADS
            const string RewardedPlacementId = "rewardedVideo";
            if (!Advertisement.IsReady(RewardedPlacementId))
            {
                this.Publish(new HideWatchAdsButtonSignal() { HideButton = true });
                //this.Publish(new UnityAdFailureSignal() { Type = RewardType });
                Debug.Log(string.Format("Ads not ready for placement '{0}'", RewardedPlacementId));
                return;
            }
            fromAd = true;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show(RewardedPlacementId, options);
#endif
        }

        public IEnumerator InitializeServiceSequentially()
        {
            throw new NotImplementedException();
        }

        /*
		private void HandleResult(ShowResult result) {
			Debug.LogFormat("UnityAds::HandleResult Result:{0}\n", result);
		}
        //*/

        /*void OnApplicationPause(bool value)
        {
            if (!value)
            {
                if (fromAd)
                {
                    if (rewardSuccessful)
                    {
                        this.Publish(new UnityAdSuccessSignal() { rewardType = RewardType });
                    }
                    else
                    {
                        this.Publish(new UnityAdFailureSignal());
                    }
                    fromAd = false;
                }
            }
        }*/

#if UNITY_ADS
        private void HandleShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    rewardSuccessful = true;
                    this.Publish(new UnityAdSuccessSignal() { rewardType = RewardType });
                    break;
                case ShowResult.Skipped:
                    rewardSuccessful = false;
                    Debug.Log("The ad was skipped before reaching the end.");
                    this.Publish(new UnityAdFailureSignal() { Type = RewardType });
                    break;
                case ShowResult.Failed:
                    rewardSuccessful = false;
                    Debug.LogError("The ad failed to be shown.");
                    this.Publish(new UnityAdFailureSignal() { Type = RewardType });
                    break;
            }
        }

        private void HandleUnrewardedAds(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    this.Publish(new UnityAdSuccessSignal() { rewardType = AdReward.NoReward });
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    this.Publish(new UnityAdFailureSignal() { Type = AdReward.NoReward });
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    this.Publish(new UnityAdFailureSignal() { Type = AdReward.NoReward });
                    break;
            }
        }
#endif

#endif
    }
}
