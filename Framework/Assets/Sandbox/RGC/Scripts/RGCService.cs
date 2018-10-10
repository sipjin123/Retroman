using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UniRx;
using UniRx.Triggers;

using uPromise;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Utils;

// Alias
using Currency = Sandbox.GraphQL.WalletRequest.CurrencyUpdate<Sandbox.GraphQL.WalletRequest.ScoreCurrency>;

namespace Sandbox.RGC
{
    using Framework;

    using Sandbox.Audio;
    using Sandbox.ButtonSandbox;
    using Sandbox.Facebook;
    using Sandbox.GraphQL;
    using Sandbox.Network;
    using Sandbox.Popup;
    using Sandbox.Preloader;
    using Sandbox.Services;

    public enum AccountType
    {
        Guest,
        Facebook,
    }
    
    public struct OnConnectToFGCApp { }

    public struct OnGetSynertix { }

    public struct OnClaimStamps { }

    public struct OnCreateOfflineUserSignal
    {
        public string Id;
    }

    public struct OnOfflineLoginSignal
    {
        public string Id;
    }

    public struct OnGuestLoginSignal
    {
        public string Token;
    }

    public struct OnFBLoginSignal
    {
        public string Id;
        public string Token;
    }

    public struct OnToggleFGCPopup
    {
        public bool Show;
    }

    /// <summary>
    /// A signal that open's the FGC App
    /// </summary>
    public struct OnOpenFGCSignal { }

    public struct OnShowFGCPopupSignal { }

    public struct OnAutoConnectToFGC
    {
        public AccountType Type;
    }

    public struct OnTestSendScore
    {
        public int Score;
    }

    public class RGCService : BaseService
    {
        public static readonly string SHOW_CONNECT_POPUP = "ShowConnectPopup";
        public static readonly string IS_INSTALLED = "IsInstalled";
        public static readonly string LOCAL_ID = "LocalId";
        public static readonly string HAS_FB_TOKEN = "HasFbToken";

        // TODO: +AS:20180830 Backend setup of FGC configs. (must support platform specific configs)
        public readonly string FGC_BUNDLE = "com.synergy88digital.framework";
        public readonly string FGC_APP_URL = "https://unity3d.com/";

        [ShowInInspector]
        private Fsm Fsm;

        [ShowInInspector]
        private bool IsFGCInstalled;

        [SerializeField]
        private AppInstallChecker NativeApp;
        
        private LocalData LocalUser;

        [SerializeField]
        private LocalUserData LocalUserData;

        // events
        private readonly string ON_LOGIN_AS_GUEST = "OnLoginAsGuest";
        private readonly string ON_LOGIN_AS_GUEST_AND_FB = "OnLoginAsGuestAndFB";
        private readonly string ON_LOGIN_AS_FB = "OnLoginAsFb";
        private readonly string ACTION_DONE = "ActionDone";

        public override void InitializeService()
        {
#if ENABLE_FGC
            Assertion.AssertNotNull(NativeApp);

            SetupReceivers();
            SetupResolvers();
            PrepareFSM();

            IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);
            
            AutoLogin();
#else
            CurrentServiceState.Value = ServiceState.Initialized;
#endif
        }

        public override IEnumerator InitializeServiceSequentially()
        {
#if ENABLE_FGC
            Assertion.AssertNotNull(NativeApp);

            SetupReceivers();
            SetupResolvers();
            PrepareFSM();
            
            IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);

            yield return null;
            
            AutoLogin();
#else
            yield return null;
            CurrentServiceState.Value = ServiceState.Initialized;
#endif
        }

        private void PrepareFSM()
        {
            Fsm = new Fsm("RGCFlow");

            // states
            FsmState idle = Fsm.AddState("Idle");
            FsmState loginAsGuest = Fsm.AddState("LoginAsGuest");
            FsmState loginAsGuestAndFB = Fsm.AddState("LoginAsGuestAndFB");
            FsmState loginAsFb = Fsm.AddState("LoginAsFb");

            // transitions
            idle.AddTransition(ON_LOGIN_AS_GUEST, loginAsGuest);
            idle.AddTransition(ON_LOGIN_AS_GUEST_AND_FB, loginAsGuestAndFB);
            idle.AddTransition(ON_LOGIN_AS_FB, loginAsFb);

            loginAsGuest.AddTransition(ACTION_DONE, idle);

            loginAsGuestAndFB.AddTransition(ON_LOGIN_AS_FB, loginAsFb);

            loginAsFb.AddTransition(ACTION_DONE, idle);

            // DEBUG Actions
            Action<FsmState> AddLogAction = state => state.AddAction(new EnterAction(state, owner => Debug.LogFormat(D.FGC + "RGCService::{0}\n", owner.GetName())));
            FsmState[] states = Fsm.States;
            states.ForEach(state => AddLogAction(state));

            // actions
            loginAsGuest.AddAction(new GuestLoginAction(loginAsGuest, ACTION_DONE));
            loginAsGuest.AddAction(new ExitAction(loginAsGuest, 
                owner =>
                {
                    bool hasToken = LocalUserData.HasToken();
                    bool hasNetwork = QuerySystem.Query<bool>(NETService.HasInternet);

                    // Fetch wallet -> Fetch Currencies -> Set ServiceState to Initialized.
                    if (hasToken && hasNetwork)
                    {
                        // Fetch wallet and currencies
                        this.Publish(new OnFetchCurrenciesSignal());

                        CurrentServiceState.Value = ServiceState.Initialized;
                    }
                    // Offline Login -> Set ServiceState to Initialized.
                    else
                    {
                        // Error on logging as guest
                        CurrentServiceState.Value = ServiceState.Initialized;
                    }
                }));

            loginAsGuestAndFB.AddAction(new GuestLoginAction(loginAsGuestAndFB, ON_LOGIN_AS_FB));
            //loginAsGuestAndFB.AddAction(new ExitAction(loginAsGuestAndFB, owner => this.Publish(new OnFetchCurrenciesSignal())));

            loginAsFb.AddAction(new FBLoginAction(loginAsFb, ACTION_DONE));
            loginAsFb.AddAction(new EnterAction(loginAsFb, owner => this.Publish(new OnShowPopupSignal() { Popup = PopupType.Spinner })));
            loginAsFb.AddAction(new ExitAction(loginAsFb,
            owner =>
            {
                // Show initial popup
                bool hasFBLogin = QuerySystem.Query<bool>(FBID.HasLoggedInUser);

                // Fetch wallet -> Fetch currencies -> CloseActivePopup (Spinner)
                if (hasFBLogin)
                {
                    // Fetch wallet and currencies
                    string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
                    string currencySlug = RGCConst.SCORE_SLUG;
                    string eventSlug = RGCConst.GAME_END;
                    Builder builder;
                    Function func;
                    Return ret;
                    OnHandleGraphRequestSignal signal;

                    // Fetch currencies
                    Action fetchCurrencies = () =>
                    {
                        Debug.LogFormat(D.FGC + "RGCService::FBLoginExit OnFechCurrencies.\n");

                        builder = Builder.Query();
                        func = builder.CreateFunction("wallet");
                        func.AddString("token", token);
                        func.AddString("slug", currencySlug);
                        ret = builder.CreateReturn("amount", "updated_at");
                        ret.Add("currency", new Return("id", "slug", "exchange_rate"));

                        signal = new OnHandleGraphRequestSignal();
                        signal.Builder = builder;
                        signal.Parser = result =>
                        {
                            this.Publish(new OnUpdateFGCCurrency() { Result = result });
                            this.Publish(new OnCloseActivePopup());
                        };

                        this.Publish(signal);
                    };

                    // Fetch wallet
                    Action fetchWallet = () =>
                    {
                        Debug.LogFormat(D.FGC + "RGCService::FBLoginExit OnFetchWallet.\n");

                        builder = Builder.Query();
                        builder
                            .CreateFunction("fgc_wallet")
                            .AddString("token", token);
                        builder.CreateReturn("amount", "updated_at");

                        signal = new OnHandleGraphRequestSignal();
                        signal.Builder = builder;
                        signal.Parser = result =>
                        {
                            this.Publish(new OnUpdateFGCWallet() { Result = result });
                            fetchCurrencies();
                        };

                        this.Publish(signal);
                    };

                    fetchWallet();
                }
                // No Active FB Account -> CloseActivePopup (Spinner)
                else
                {
                    this.Publish(new OnCloseActivePopup());
                }

                if (!hasFBLogin)
                {
                    Debug.LogFormat(D.ERROR + "RGCService::loginAsFb::ExitAction Error logging in Fb.\n");
                }
                else if (!IsFGCInstalled)
                {
                    //this.Publish(new OnShowPopupSignal() { Popup = PopupType.DownloadFGC });
                }
                else
                {
                    //this.Publish(new OnShowPopupSignal() { Popup = PopupType.Profile });
                }
            }));

            // auto start fsm
            Fsm.Start("Idle");

            // update loop
            this.UpdateAsObservable()
                .Subscribe(_ => Fsm.Update())
                .AddTo(this);
        }

        private void SetupReceivers()
        {
#if IS_ESGS_BUILD
            this.Receive<OnShowFGCPopupSignal>()
                .Subscribe(_ => ShowFGCPopup())
                .AddTo(this);
#endif

            this.Receive<OnAutoConnectToFGC>()
                .Subscribe(_ =>
                {
                    if (_.Type == AccountType.Guest)
                    {
                        Debug.LogFormat(D.FGC + "RGCService::OnAutoConnectToFGC Guest\n");
                        Fsm.SendEvent(ON_LOGIN_AS_GUEST);
                    }
                    else
                    {
                        Debug.LogFormat(D.FGC + "RGCService::OnAutoConnectToFGC Facebook HasFB:{0}\n", QuerySystem.Query<bool>(FBID.HasLoggedInUser));
                        bool hasFBUser = QuerySystem.Query<bool>(FBID.HasLoggedInUser);
                        Fsm.SendEvent(ON_LOGIN_AS_GUEST_AND_FB);
                    }
                })
                .AddTo(this);

            this.Receive<OnConnectToFGCApp>()
                .Subscribe(_ => OnClickedFGCButton())
                .AddTo(this);

            this.Receive<OnGetSynertix>()
                .Subscribe(
                _ =>
                {
                    bool hasFbToken = LocalUserData.HasFBToken();
                    bool hasFbLogin = QuerySystem.Query<bool>(FBID.HasLoggedInUser);

                    if (hasFbToken && hasFbLogin)
                    {
                        ShowConvertPopup();
                    }
                })
                .AddTo(this);

            this.Receive<OnClaimStamps>()
                .Subscribe(_ =>
                {
                    this.Publish(new OnShowPopupSignal()
                    {
                        Popup = PopupType.ClaimStamps,
                        PopupData = new PopupData(QuerySystem.Query<GraphConfigs>(ConfigurationRequest.GRAPH_CONFIGS).QRUrl)
                    });
                })
                .AddTo(this);
           
            this.Receive<OnCreateOfflineUserSignal>()
                .Subscribe(
                _ =>
                {
                    LocalUserData = new LocalUserData();
                    LocalUserData.Id = _.Id;
                    LocalUserData.DeviceId = _.Id;

                    LocalUser = new LocalData(_.Id, "localdata", "userdata");
                    LocalUser.ReplaceToDisk(LocalUserData);

                    PREFS.SetString(LOCAL_ID, _.Id);
                })
                .AddTo(this);

            this.Receive<OnOfflineLoginSignal>()
                .Subscribe(
                _ =>
                {
                    Assertion.AssertNotEmpty(_.Id);

                    LocalUser = new LocalData(_.Id, "localdata", "userdata");
                    LocalUserData = LocalUser.LoadFromDisk<LocalUserData>();

                    PREFS.SetString(RGCService.LOCAL_ID, _.Id);
                })
                .AddTo(this);

            this.Receive<OnGuestLoginSignal>()
                .Subscribe(_ =>
                {
                    Assertion.AssertNotEmpty(_.Token);
                    
                    LocalUserData.Token = _.Token;
                    LocalUser = new LocalData(LocalUserData.Id, "localdata", "userdata");
                    LocalUser.ReplaceToDisk(LocalUserData);
                })
                .AddTo(this);

            this.Receive<OnFBLoginSignal>()
                .Subscribe(
                _ =>
                {
                    LocalUserData.FBId = _.Id;
                    LocalUserData.FBToken = _.Token;
                    LocalUser.ReplaceToDisk(LocalUserData);
                })
                .AddTo(this);

            /*
            this.Receive<OnPollSignal>()
                .Subscribe(_ =>
                {
                    IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);

                    //Debug.LogFormat(D.FGC + "RGCService::SetupResolvers IsFGCInitialized:{0}\n", IsFGCInstalled);
                })
                .AddTo(this);
            //*/

            this.Receive<OnOpenFGCSignal>()
                .Subscribe(_ =>
                {
                    bool hasFGCApp = IsFGCInstalled;
                    bool hasNetwork = QuerySystem.Query<bool>(NETService.HasInternet);
                    if ((hasNetwork && hasFGCApp) || (!hasNetwork && hasFGCApp))
                    {
                        // TODO: +AS:09132018 Open the FGC App here
                        Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal TODO! Open FGC App here!\n");
                        Application.OpenURL(FGC_APP_URL);
                    }
                    else if (hasNetwork && !hasFGCApp)
                    {
                        // TODO: +AS:09132018 Show the FGC store link here!
                        Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal TODO! Show the FGC store link here!\n");
                    }
                    else
                    {
                        // TODO: +AS:09132018 Show no internet connection popup here!
                        Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal TODO! Show no internet connection popup here!\n");
                    }
                })
                .AddTo(this);

            this.Receive<OnToggleFGCPopup>()
                .Subscribe(_ => PREFS.SetBool(SHOW_CONNECT_POPUP, _.Show))
                .AddTo(this);

            this.Receive<OnTestSendScore>()
                .Subscribe(_ => TestSendScore(_.Score))
                .AddTo(this);

            AddButtonHandler(ButtonType.FGC, delegate (ButtonClickedSignal signal)
            {
                Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal LocalUserData:{0}\n", LocalUserData);
                OnClickedFGCButton();
            });
            
            AddButtonHandler(ButtonType.DownloadFGC, (ButtonClickedSignal signal) =>
            {
                this.Publish(new OnCloseActivePopup());
                this.Publish(new OnOpenFGCSignal());
            });

            AddButtonHandler(ButtonType.Facebook, (ButtonClickedSignal signal) =>
            {
                this.Publish(new OnCloseActivePopup());
                Fsm.SendEvent(this.ON_LOGIN_AS_FB);
            });
            
            AddButtonHandler(ButtonType.ConnectToFGC, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
            });

            AddButtonHandler(ButtonType.GetSynerytix, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnGetSynertix());
            });

            AddButtonHandler(ButtonType.ClaimStamps, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnClaimStamps());
            });
        }

        /*/
        Login Scenarios
            > Started Offline
                1.
                    > Login as Offline
                    - Clicked FGC Button
                        If has FGC app
                            > Open FGC
                        else
                            > Shows no Internet Connection popup
                2.
                    > Same scenario in 1.
                    > Enabled internat connection
                    - Clicked FGC Button
                        > Login as guest and fb
                            If has FGC app
                                > Open FGC
                            else
                                > Shows download FGC popup
            > Started Online
                1.
                    > Auto login as guest
                    - Clicked FGC Button
                        > Fb login
                            if FAIL
                                > Handle stuff here (not yet defined in flow)
                                > More or less, do 'Started Offline action if has no internet'
                            else
                                If has FGC app
                                    > Open FGC
                                else
                                    > Shows download FGC popup
        //*/
        private void OnClickedFGCButton()
        {
            do
            {
                bool hasNetwork = QuerySystem.Query<bool>(NETService.HasInternet);

                // is started offline? do guest login then facebook
                bool isOffline = !LocalUserData.HasToken();
                if (isOffline && hasNetwork)
                {
                    Fsm.SendEvent(ON_LOGIN_AS_GUEST_AND_FB);
                    break;
                }

                // if online and has loggedin as guest? do fb login
                bool isGuest = !LocalUserData.HasFBToken() || !QuerySystem.Query<bool>(FBID.HasLoggedInUser);
                if (isGuest && hasNetwork)
                {
                    Fsm.SendEvent(ON_LOGIN_AS_FB);
                    break;
                }
                
                // Connected to GMC with FB
                //this.Publish(new OnOpenFGCSignal());
                break;
            } while (true);
        }
        
        private void SetupResolvers()
        {
            QuerySystem.RegisterResolver(IS_INSTALLED, RegisterFGCFlag);
            QuerySystem.RegisterResolver(HAS_FB_TOKEN, RegisterFbTokenFlag);
            
        }

        private void RegisterFGCFlag(IQueryRequest request, IMutableQueryResult result)
        {
            result.Set(IsFGCInstalled);
        }

        private void RegisterFbTokenFlag(IQueryRequest request, IMutableQueryResult result)
        {
            result.Set(LocalUserData.HasFBToken());
        }

        private void ShowFGCPopup()
        {
            if (PREFS.GetBool(SHOW_CONNECT_POPUP, true))
            {
                this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
            }
        }

        private void AutoLogin()
        {
#if !ENABLE_AUTOMATION
            // TODO: +AS:09262018 Revise fb auto login
            bool hasFBUser = QuerySystem.Query<bool>(FBID.HasLoggedInUser);

            Debug.LogFormat(D.FGC + "RGCService::AutoLoginFb HasFB:{0}\n", hasFBUser);
            Fsm.SendEvent(ON_LOGIN_AS_GUEST);
#else
            CurrentServiceState.Value = ServiceState.Initialized;
#endif
        }

        private void ShowConvertPopup()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);

            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();
            GameResultInfo data = GameResultFactory.Create(currency.CurrencyInfo.CurrencySlug, currency.Amount, currency.CurrencyInfo.Rate);
            
            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(data) });
        }

        private void TestSendScore(int score)
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            string currencySlug = RGCConst.SCORE_SLUG;
            string eventSlug = RGCConst.GAME_END;
            Builder builder;
            Function func;
            Return ret;
            Payload load;
            OnHandleGraphRequestSignal signal;

            // Fetch currencies
            Action fetchCurrencies = () =>
            {
                Debug.LogFormat(D.FGC + "TitleRoot::OnClickedSend OnFechCurrencies.\n");

                builder = Builder.Query();
                func = builder.CreateFunction("wallet");
                func.AddString("token", token);
                func.AddString("slug", currencySlug);
                ret = builder.CreateReturn("amount", "updated_at");
                ret.Add("currency", new Return("id", "slug", "exchange_rate"));

                signal = new OnHandleGraphRequestSignal();
                signal.Builder = builder;
                signal.Parser = result =>
                {
                    this.Publish(new OnUpdateFGCCurrency() { Result = result });
                    this.Publish(new OnCloseActivePopup());
                };

                this.Publish(signal);
            };

            // Fetch wallet
            Action fetchWallet = () =>
            {
                Debug.LogFormat(D.FGC + "TitleRoot::OnClickedSend OnFetchWallet.\n");

                builder = Builder.Query();
                builder
                    .CreateFunction("fgc_wallet")
                    .AddString("token", token);
                builder.CreateReturn("amount", "updated_at");

                signal = new OnHandleGraphRequestSignal();
                signal.Builder = builder;
                signal.Parser = result =>
                {
                    this.Publish(new OnUpdateFGCWallet() { Result = result });
                    fetchCurrencies();
                };

                this.Publish(signal);
            };

            // Submit score request
            Debug.LogFormat(D.FGC + "TitleRoot::OnClickedSend OnSubmitScore.\n");
            Currency payload = new Currency() { currencies = new WalletRequest.ScoreCurrency(score) };

            builder = Builder.Mutation();
            builder.CreateReturn("id");

            load = new Payload();
            load.AddString("message", "Events Event");
            load.AddJsonString("body", payload.ToJson());

            func = builder.CreateFunction("event_trigger");
            func.AddString("token", token);
            func.AddString("slug", eventSlug);
            func.Add("payload", load.ToString());

            signal = new OnHandleGraphRequestSignal();
            signal.Builder = builder;
            signal.Parser = result => fetchWallet();

            this.Publish(new OnShowPopupSignal() { Popup = PopupType.Spinner });
            this.Publish(signal);
        }
    }
}