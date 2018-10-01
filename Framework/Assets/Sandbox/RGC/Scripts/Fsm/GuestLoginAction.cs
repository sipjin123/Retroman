using System;

using UnityEngine;

using Sirenix.Utilities;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Fsm;
using Common.Fsm.Action;
using Common.Query;

using Framework;

using Sandbox.Facebook;
using Sandbox.GraphQL;
using Sandbox.Network;

namespace Sandbox.RGC
{
    [Serializable]
    public class LocalUserData : IJson
    {
        // user data
        public string Id;
        public string DeviceId;
        public string Token;

        // facebook data
        public string FBId;
        public string FBToken;

        // profile
        public string First;
        public string Middle;
        public string Last;
        public string Birthdate;
        public string Gender;
        public string Address;
        public string City;
        public string Email;

        public int Mobile;

        public bool HasToken()
        {
            return !string.IsNullOrEmpty(Token.Trim());
        }

        public bool HasFBToken()
        {
            return !string.IsNullOrEmpty(FBToken.Trim());
        }
    }

    public class GuestLoginAction : FsmActionAdapter, IBroker
    {
        private string Evt;
        private string Id;
        private CompositeDisposable Disposables;

        public GuestLoginAction(FsmState owner, string evt) : base(owner)
        {
            Evt = evt;
            Disposables = new CompositeDisposable();

            PrepareFsm();
        }

        public override void OnEnter()
        {
            Debug.LogFormat(D.FGC + "GuestLoginAction::OnEnter\n");

            Fsm.SendEvent(ON_LOGIN);
        }

        public override void OnUpdate()
        {
            Fsm.Update();
        }

        private bool HasLocalId()
        {
            return PREFS.TryGet(RGCService.LOCAL_ID, ref Id);
        }
        
        #region Fsm
        [SerializeField]
        private Fsm Fsm;

        // events
        private readonly string ON_LOGIN = "ON_LOGIN";
        private readonly string ON_CREATE_AND_LOGIN_OFFLINE = "ON_CREATE_AND_LOGIN_OFFLINE";
        private readonly string ON_CREATE_AND_LOGIN_AS_GUEST = "ON_CREATE_AND_LOGIN_AS_GUEST";
        private readonly string ON_LOGIN_OFFLINE = "ON_LOGIN_OFFLINE";
        private readonly string ON_LOGIN_QUEST = "ON_LOGIN_QUEST";
        private readonly string ON_LOGIN_DONE = "ON_LOGIN_DONE";
        private readonly string FINISH = "FINISH";

        private void PrepareFsm()
        {
            Debug.LogFormat(D.FGC + "GuestLoginAction::PrepareFsm\n");

            Fsm = new Fsm("LoginFsm");

            // stats
            FsmState idle = Fsm.AddState("Idle");
            FsmState onLogin = Fsm.AddState("OnLogin");
            FsmState createOfflineLoginOffline = Fsm.AddState("CreateOfflineLoginOffline");
            FsmState createOfflineLoginAsGuest = Fsm.AddState("CreateOfflineLoginAsGues");
            FsmState offlineLogin = Fsm.AddState("OfflineLogin");
            FsmState loginAsGuest = Fsm.AddState("LoginAsGuest");
            FsmState loginDone = Fsm.AddState("LoginDone");

            // transitions
            idle.AddTransition(ON_LOGIN, onLogin);

            onLogin.AddTransition(ON_CREATE_AND_LOGIN_OFFLINE, createOfflineLoginOffline);
            onLogin.AddTransition(ON_CREATE_AND_LOGIN_AS_GUEST, createOfflineLoginAsGuest);
            onLogin.AddTransition(ON_LOGIN_OFFLINE, offlineLogin);
            onLogin.AddTransition(ON_LOGIN_QUEST, loginAsGuest);

            createOfflineLoginOffline.AddTransition(ON_LOGIN_OFFLINE, offlineLogin);
            createOfflineLoginAsGuest.AddTransition(ON_LOGIN_QUEST, loginAsGuest);

            offlineLogin.AddTransition(ON_LOGIN_DONE, loginDone);
            loginAsGuest.AddTransition(ON_LOGIN_DONE, loginDone);

            loginDone.AddTransition(FINISH, idle);

            // DEBUG Actions
            Action<FsmState> AddLogAction = state => state.AddAction(new EnterAction(state, owner => Debug.LogFormat(D.FGC + "GuestLoginAction::{0}\n", owner.GetName())));
            FsmState[] states = Fsm.States;
            states.ForEach(state => AddLogAction(state));

            // actions
            onLogin.AddAction(new FsmDelegateAction(onLogin,
               owner =>
               {
                   bool hasNetwork = QuerySystem.Query<bool>(NETService.HasInternet);
                   bool hasLocal = HasLocalId();

                   if (!hasNetwork && !hasLocal)
                   {
                       // TODO: +AS:20180828 create local id
                       Fsm.SendEvent(ON_CREATE_AND_LOGIN_OFFLINE);
                   }
                   else if (!hasNetwork && hasLocal)
                   {
                       // TODO: +AS:20180828 login local id
                       Fsm.SendEvent(ON_LOGIN_OFFLINE);
                   }
                   else if (hasNetwork && !hasLocal)
                   {
                       Fsm.SendEvent(ON_CREATE_AND_LOGIN_AS_GUEST);
                   }
                   else if (hasNetwork && hasLocal)
                   {
                       Fsm.SendEvent(ON_LOGIN_QUEST);
                   }
                   else
                   {
                       Assertion.Assert(false);
                   }
               }));

            createOfflineLoginOffline.AddAction(new FsmDelegateAction(createOfflineLoginOffline,
                owner =>
                {
                    Id = Platform.DeviceId;

                    this.Publish(new OnCreateOfflineUserSignal() { Id = Id });

                    Fsm.SendEvent(ON_LOGIN_OFFLINE);
                }));

            createOfflineLoginAsGuest.AddAction(new FsmDelegateAction(createOfflineLoginAsGuest,
                owner =>
                {
                    Id = Platform.DeviceId;

                    this.Publish(new OnCreateOfflineUserSignal() { Id = Id });

                    Fsm.SendEvent(ON_LOGIN_QUEST);
                }));

            offlineLogin.AddAction(new FsmDelegateAction(offlineLogin,
                owner =>
                {
                    this.Publish(new OnOfflineLoginSignal() { Id = Id });

                    Fsm.SendEvent(ON_LOGIN_DONE);
                }));

            loginAsGuest.AddAction(new FsmDelegateAction(loginAsGuest,
                owner =>
                {
                    this.Receive<GraphQLRequestSuccessfulSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "GuestLoginAction::LoginAsGuest::Success Token:{0}\n", _.GetData<string>());

                            this.Publish(new OnGuestLoginSignal() { Token = _.GetData<string>()});

                            Fsm.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Receive<GraphQLRequestFailedSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "GuestLoginAction::LoginAsGuest::Fail\n");

                            Fsm.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Publish(new OnOfflineLoginSignal() { Id = Id });
                    this.Publish(new OnGraphLoginSignal() { IsGuest = true });
                },
                owner => {},
                owner => Disposables.Clear()));

            loginDone.AddAction(new FsmDelegateAction(loginDone,
                owner =>
                {
                    Fsm.SendEvent(FINISH);

                    GetOwner().SendEvent(Evt);
                }));

            // auto start fsm
            Fsm.Start("Idle");
        }
        #endregion
    }
}