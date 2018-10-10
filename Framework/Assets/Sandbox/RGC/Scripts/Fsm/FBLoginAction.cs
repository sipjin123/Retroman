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
    public class FBLoginAction : FsmActionAdapter, IBroker
    {
        private const string LOCAL_ID = "LocalId";

        private string Evt;

        private string FbId;

        private string FbToken;
        
        private CompositeDisposable Disposables;

        public FBLoginAction(FsmState owner, string evt) : base(owner)
        {
            Evt = evt;
            Disposables = new CompositeDisposable();

            PrepareFsm();
        }

        public override void OnEnter()
        {
            Debug.LogFormat(D.FGC + "FBLoginAction::OnEnter\n");

            Fsm.SendEvent(ON_LOGIN_FB);
        }

        public override void OnUpdate()
        {
            Fsm.Update();
        }

        #region Fsm
        [SerializeField]
        private Fsm Fsm;

        // events
        private readonly string ON_LOGIN_FB = "ON_LOGIN_FB";
        private readonly string ON_LOGIN_DONE = "ON_LOGIN_DONE";
        private readonly string ON_LOGIN_FAILED = "ON_LOGIN_FAILED";
        private readonly string FINISH = "FINISH";

        private void PrepareFsm()
        {
            Debug.LogFormat(D.FGC + "FBLoginAction::PrepareFsm\n");

            Fsm = new Fsm("LoginFsm");

            // stats
            FsmState idle = Fsm.AddState("Idle");
            FsmState loginAsFb = Fsm.AddState("LoginAsFb");
            FsmState loginToFGC = Fsm.AddState("LoginToFGC");
            FsmState loginDone = Fsm.AddState("LoginDone");

            // transitions
            idle.AddTransition(ON_LOGIN_FB, loginAsFb);
            
            loginAsFb.AddTransition(ON_LOGIN_DONE, loginToFGC);
            loginAsFb.AddTransition(ON_LOGIN_FAILED, loginDone);

            loginToFGC.AddTransition(FINISH, loginDone);

            loginDone.AddTransition(FINISH, idle);

            // DEBUG Actions
            Action<FsmState> AddLogAction = state => state.AddAction(new EnterAction(state, owner => Debug.LogFormat(D.FGC + "FBLoginAction::{0}\n", owner.GetName())));
            FsmState[] states = Fsm.States;
            states.ForEach(state => AddLogAction(state));

            // actions
            loginAsFb.AddAction(new FsmDelegateAction(loginAsFb,
                owner =>
                {
                    this.Receive<OnFacebookLoginSuccessSignal>()
                        .Subscribe(
                        _ =>
                        {
                            Debug.LogFormat(D.FGC + "FBLoginAction::{0}::Success FbId:{1} FbToken:{2}\n", owner.GetName(), _.Id, _.Token);

                            this.Publish(new OnFBLoginSignal() { Id = _.Id, Token = _.Token });
                            Fsm.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    // TODO: +AS:09142018 Handle Fb login failed
                    this.Receive<OnFacebookLoginFailedSignal>()
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "FBLoginAction::{0}::Fail\n", owner.GetName());
                            Fsm.SendEvent(ON_LOGIN_FAILED);
                        })
                        .AddTo(Disposables);

                    this.Publish(new OnFacebookLoginSignal());
                },
                owner => { },
                owner => Disposables.Clear()));
            
            loginToFGC.AddAction(new FsmDelegateAction(loginToFGC,
                owner =>
                {
                    this.Receive<GraphQLRequestSuccessfulSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "FBLoginAction::loginToFGC::Success Token:{0}\n", _.GetData<string>());

                            this.Publish(new OnGuestLoginSignal() { Token = _.GetData<string>() });

                            Fsm.SendEvent(FINISH);
                        })
                        .AddTo(Disposables);

                    this.Receive<GraphQLRequestFailedSignal>()
                        .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.FGC + "FBLoginAction::loginToFGC::Fail\n");

                            Fsm.SendEvent(FINISH);
                        })
                        .AddTo(Disposables);
                    
                    this.Publish(new OnGraphLoginSignal() { IsGuest = false });

                    //owner.SendEvent(FINISH);
                }));

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

