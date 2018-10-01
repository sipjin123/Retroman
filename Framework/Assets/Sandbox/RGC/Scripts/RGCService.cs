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
    
    public struct OnConnectToFGCApp { }

    public struct OnGetSynertix { }

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

    public class RGCService : BaseService
    {
        public static readonly string SHOW_CONNECT_POPUP = "ShowConnectPopup";
        public static readonly string IS_INSTALLED = "IsInstalled";
        public static readonly string LOCAL_ID = "LocalId";

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
            Assertion.AssertNotNull(NativeApp);

            SetupReceivers();
            SetupResolvers();
            PrepareFSM();

            IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);

            CurrentServiceState.Value = ServiceState.Initialized;
            
            // Auto login
            Fsm.SendEvent(ON_LOGIN_AS_GUEST);
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            Assertion.AssertNotNull(NativeApp);

            SetupReceivers();
            SetupResolvers();
            PrepareFSM();
            
            IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);

            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
            
            Fsm.SendEvent(ON_LOGIN_AS_GUEST);
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
            idle.AddAction(new EnterAction(idle,
                owner =>
                {
                    if (PREFS.GetBool(SHOW_CONNECT_POPUP, true))
                    {
                        this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
                    }
                }));

            loginAsGuest.AddAction(new GuestLoginAction(loginAsGuest, ACTION_DONE));
            loginAsGuest.AddAction(new ExitAction(loginAsGuest, 
                owner =>
                {
                    bool hasToken = LocalUserData.HasToken();
                    bool hasNetwork = QuerySystem.Query<bool>(NETService.HasInternet);
                    if (hasToken && hasNetwork)
                    {
                        this.Publish(new OnFetchCurrenciesSignal());
                    }
                    else
                    {
                        // Error on logging in Facebook
                    }
                }));

            loginAsGuestAndFB.AddAction(new GuestLoginAction(loginAsGuestAndFB, ON_LOGIN_AS_FB));
            loginAsGuestAndFB.AddAction(new ExitAction(loginAsGuestAndFB, owner => this.Publish(new OnFetchCurrenciesSignal())));

            loginAsFb.AddAction(new FBLoginAction(loginAsFb, ACTION_DONE));
            loginAsFb.AddAction(new ExitAction(loginAsFb,
            owner =>
            {
                bool hasFBLogin = QuerySystem.Query<bool>(FBID.HasLoggedInUser);
                if (!hasFBLogin)
                {
                    Debug.LogFormat(D.ERROR + "RGCService::loginAsFb::ExitAction Error logging in Fb.\n");
                }
                else if (!IsFGCInstalled)
                {
                    this.Publish(new OnShowPopupSignal() { Popup = PopupType.DownloadFGC });
                }
                else
                {
                    this.Publish(new OnShowPopupSignal() { Popup = PopupType.Profile });
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
            this.Receive<OnConnectToFGCApp>()
                .Subscribe(_ => OnClickedFGCButton())
                .AddTo(this);

            this.Receive<OnGetSynertix>()
                .Subscribe(
                _ =>
                {
                    GameResultInfo data = GameResultFactory.CreateDefault();

                    if (QuerySystem.Query<bool>(NETService.HasInternet))
                    {
                        this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(data) });
                    }
                    else
                    {
                        this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOffline, PopupData = new PopupData(data) });
                    }
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

            this.Receive<OnPollSignal>()
                .Subscribe(_ =>
                {
                    IsFGCInstalled = NativeApp.IsInstalled(FGC_BUNDLE);

                    //Debug.LogFormat(D.FGC + "RGCService::SetupResolvers IsFGCInitialized:{0}\n", IsFGCInstalled);
                })
                .AddTo(this);

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

            AddButtonHandler(ButtonType.ConvertSynertix, delegate(ButtonClickedSignal signal)
            {
                this.Publish(new OnCloseActivePopup());
            });

            AddButtonHandler(ButtonType.ConnectToFGC, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
            });

            AddButtonHandler(ButtonType.GetSynerytix, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnGetSynertix());
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
                    Fsm.SendEvent(this.ON_LOGIN_AS_GUEST_AND_FB);
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
                this.Publish(new OnOpenFGCSignal());
                break;
            } while (true);
        }
        
        private void SetupResolvers()
        {
            QuerySystem.RegisterResolver(IS_INSTALLED, RegisterFGCFlag);
        }

        private void RegisterFGCFlag(IQueryRequest request, IMutableQueryResult result)
        {
            result.Set(IsFGCInstalled);
        }
    }
}