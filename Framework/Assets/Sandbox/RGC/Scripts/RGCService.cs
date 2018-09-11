using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

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

    /// <summary>
    /// A signal that open's the FGC App
    /// </summary>
    public struct OnOpenFGCSignal { }

    public class RGCService : BaseService
    {
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
        private LocalUserData LocalUserData;

        // events
        private readonly string ON_LOGIN_AS_GUEST = "OnLoginAsGuest";
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

            // TEST
            //Fsm.SendEvent(ON_LOGIN_AS_GUEST);
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

            // TEST
            //Fsm.SendEvent(ON_LOGIN_AS_GUEST);
        }

        private void PrepareFSM()
        {
            Fsm = new Fsm("RGCFlow");

            // states
            FsmState idle = Fsm.AddState("Idle");
            FsmState loginAsGuest = Fsm.AddState("LoginAsGuest");
            FsmState loginAsFb = Fsm.AddState("LoginAsFb");

            // transitions
            idle.AddTransition(ON_LOGIN_AS_GUEST, loginAsGuest);
            idle.AddTransition(ON_LOGIN_AS_FB, loginAsFb);

            loginAsGuest.AddTransition(ACTION_DONE, loginAsFb);

            loginAsFb.AddTransition(ACTION_DONE, idle);

            // actions
            idle.AddAction(new FsmDelegateAction(idle, owner => Debug.LogFormat(D.FGC + "RGCService::Idle\n")));

            loginAsGuest.AddAction(new GuestLoginAction(loginAsGuest, ACTION_DONE));
            loginAsGuest.AddAction(new ExitAction(loginAsGuest, owner => this.Publish(new TEST_OnFetchRatesSignal())));

            loginAsFb.AddAction(new FBLoginAction(loginAsFb, ACTION_DONE));
            loginAsFb.AddAction(new ExitAction(loginAsFb,
            owner =>
            {
                if (!IsFGCInstalled)
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
                .Subscribe(_ => Fsm.SendEvent(ON_LOGIN_AS_FB))
                .AddTo(this);

            this.Receive<OnGetSynertix>()
                .Subscribe(
                _ =>
                {
                    if (QuerySystem.Query<bool>(NETService.HasInternet))
                    {
                        this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(RGCConst.POINT_ID) });
                    }
                    else
                    {
                        this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOffline, PopupData = new PopupData(RGCConst.POINT_ID) });
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
                    Assertion.AssertNotNull(_.Id);
                    LocalUser = new LocalData(_.Id, "localdata", "userdata");
                    LocalUserData = LocalUser.LoadFromDisk<LocalUserData>();

                    PREFS.SetString(RGCService.LOCAL_ID, _.Id);
                })
                .AddTo(this);

            this.Receive<OnGuestLoginSignal>()
                .Subscribe(
                _ =>
                {
                    LocalUserData.Token = _.Token;
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
                .Subscribe(_ => Application.OpenURL(FGC_APP_URL))
                .AddTo(this);

            AddButtonHandler(ButtonType.FGC, delegate (ButtonClickedSignal signal)
            {
                Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal LocalUserData:{0}\n", LocalUserData);
                bool isConnectedToFGC = LocalUserData != null;
                if (!isConnectedToFGC)
                {
                    Fsm.SendEvent(ON_LOGIN_AS_GUEST);
                }
                else
                {
                    // TODO: +AS:20180910 Open FGC app here
                    Debug.LogFormat(D.FGC + "RGCService::OnOpenFGCSignal Open FGC App here!\n");
                    this.Publish(new OnOpenFGCSignal());
                }
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

            AddButtonHandler(ButtonType.GetSynertix, delegate (ButtonClickedSignal signal)
            {
                Debug.LogError("sYNERTIX GET");
                this.Publish(new OnGetSynertix());
            });

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