using System;

using UnityEngine;

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

        private bool HasNetwork()
        {
            return QuerySystem.Query<bool>(NETService.HasInternet);
        }

        #region Fsm
        [SerializeField]
        private Fsm Fsm;

        // events
        private readonly string ON_LOGIN = "ON_LOGIN";
        private readonly string ON_CREATE_AND_LOGIN_OFFLINE = "ON_CREATE_AND_LOGIN_OFFLINE";
        private readonly string ON_LOGIN_OFFLINE = "ON_LOGIN_OFFLINE";
        private readonly string ON_LOGIN_QUEST = "ON_LOGIN_QUEST";
        private readonly string ON_LOGIN_DONE = "ON_LOGIN_DONE";

        private void PrepareFsm()
        {
            Debug.LogFormat(D.FGC + "GuestLoginAction::PrepareFsm\n");

            Fsm = new Fsm("LoginFsm");

            // stats
            FsmState idle = Fsm.AddState("Idle");
            FsmState onLogin = Fsm.AddState("OnLogin");
            FsmState createOfflineLogin = Fsm.AddState("CreateOfflineLogin");
            FsmState offlineLogin = Fsm.AddState("OfflineLogin");
            FsmState loginAsGuest = Fsm.AddState("LoginAsGuest");
            FsmState loginDone = Fsm.AddState("LoginDone");

            // transitions
            idle.AddTransition(ON_LOGIN, onLogin);

            onLogin.AddTransition(ON_CREATE_AND_LOGIN_OFFLINE, createOfflineLogin);
            onLogin.AddTransition(ON_LOGIN_OFFLINE, offlineLogin);
            onLogin.AddTransition(ON_LOGIN_QUEST, loginAsGuest);

            createOfflineLogin.AddTransition(ON_LOGIN_QUEST, loginAsGuest);
            offlineLogin.AddTransition(ON_LOGIN_QUEST, loginAsGuest);

            loginAsGuest.AddTransition(ON_LOGIN_DONE, loginDone);

            // actions
            onLogin.AddAction(new FsmDelegateAction(onLogin,
               owner =>
               {
                   Debug.LogFormat(D.FGC + "GuestLoginAction::{0}\n", owner.GetName());

                   bool hasNetwork = HasNetwork();
                   bool hasLocal = HasLocalId();

                   if (!hasNetwork && !hasLocal)
                   {
                       // TODO: +AS:20180828 create local id
                       owner.SendEvent(ON_CREATE_AND_LOGIN_OFFLINE);
                   }
                   else if (!hasNetwork && hasLocal)
                   {
                       // TODO: +AS:20180828 login local id
                       owner.SendEvent(ON_LOGIN_OFFLINE);
                   }
                   else if (hasNetwork)
                   {
                       owner.SendEvent(ON_CREATE_AND_LOGIN_OFFLINE);
                   }
                   else
                   {
                       Assertion.Assert(false);
                   }
               }));

            createOfflineLogin.AddAction(new FsmDelegateAction(createOfflineLogin,
                owner =>
                {
                    Debug.LogFormat(D.FGC + "GuestLoginAction::{0}\n", owner.GetName());

                    Id = Platform.DeviceId;

                    this.Publish(new OnCreateOfflineUserSignal() { Id = Id });
                    
                    owner.SendEvent(ON_LOGIN_QUEST);
                }));

            offlineLogin.AddAction(new FsmDelegateAction(offlineLogin,
                owner =>
                {
                    Debug.LogFormat(D.FGC + "GuestLoginAction::{0} Id:{1}\n", owner.GetName(), Id);

                    this.Publish(new OnOfflineLoginSignal() { Id = Id });

                    owner.SendEvent(ON_LOGIN_QUEST);
                }));

            loginAsGuest.AddAction(new FsmDelegateAction(loginAsGuest,
                owner =>
                {
                    Debug.LogFormat(D.FGC + "GuestLoginAction::{0}\n", owner.GetName());

                    this.Receive<GraphQLRequestSuccessfulSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "GuestLoginAction::LoginAsGuest::Success Token:{0}\n", _.GetData<string>());

                            this.Publish(new OnGuestLoginSignal() { Token = _.GetData<string>()});

                            owner.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Receive<GraphQLRequestFailedSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "GuestLoginAction::LoginAsGuest::Fail\n");

                            owner.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Publish(new OnGraphLoginSignal());
                },
                owner => {},
                owner => Disposables.Clear()));

            loginDone.AddAction(new FsmDelegateAction(loginDone,
                owner =>
                {
                    Debug.LogFormat(D.FGC + "GuestLoginAction::{0}\n", owner.GetName());

                    GetOwner().SendEvent(Evt);
                }));

            // auto start fsm
            Fsm.Start("Idle");
        }
        #endregion
    }
}

