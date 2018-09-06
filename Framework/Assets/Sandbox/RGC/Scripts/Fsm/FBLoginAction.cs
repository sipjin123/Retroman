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
    public class FBLoginAction : FsmActionAdapter, IBroker
    {
        private const string LOCAL_ID = "LocalId";

        private string Evt;
        
        private CompositeDisposable Disposables;

        public FBLoginAction(FsmState owner, string evt) : base(owner)
        {
            Evt = evt;
            Disposables = new CompositeDisposable();

            PrepareFsm();
        }

        public override void OnEnter()
        {
            Debug.LogFormat(D.LOG + "FBLoginAction::OnEnter\n");

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
        private readonly string FINISH = "FINISH";

        private void PrepareFsm()
        {
            Debug.LogFormat(D.LOG + "FBLoginAction::PrepareFsm\n");

            Fsm = new Fsm("LoginFsm");

            // stats
            FsmState idle = Fsm.AddState("Idle");
            FsmState loginAsFb = Fsm.AddState("LoginAsFb");
            FsmState loginDone = Fsm.AddState("LoginDone");

            // transitions
            idle.AddTransition(ON_LOGIN_FB, loginAsFb);
            
            loginAsFb.AddTransition(ON_LOGIN_DONE, loginDone);

            loginDone.AddTransition(FINISH, idle);
            
            loginAsFb.AddAction(new FsmDelegateAction(loginAsFb,
                owner =>
                {
                    Debug.LogFormat(D.LOG + "FBLoginAction::{0}\n", owner.GetName());

                    this.Receive<OnFacebookLoginSuccessSignal>()
                        .Subscribe(
                        _ =>
                        {
                            Debug.LogFormat(D.LOG + "FBLoginAction::{0}::Success FbId:{1} FbToken:{2}\n", owner.GetName(), _.Id, _.Token);

                            this.Publish(new OnFBLoginSignal() { Id = _.Id, Token = _.Token });
                            owner.SendEvent(ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Receive<OnFacebookLoginFailedSignal>()
                        .Subscribe(_ =>
                        {
                            Debug.LogFormat(D.LOG + "FBLoginAction::{0}::Fail\n", owner.GetName());
                            owner.SendEvent(this.ON_LOGIN_DONE);
                        })
                        .AddTo(Disposables);

                    this.Publish(new OnFacebookLoginSignal());
                },
                owner => { },
                owner => Disposables.Clear()));

            loginDone.AddAction(new FsmDelegateAction(loginDone,
                owner =>
                {
                    Debug.LogFormat(D.LOG + "FBLoginAction::{0}\n", owner.GetName());

                    owner.SendEvent(FINISH);
                    
                    GetOwner().SendEvent(Evt);
                }));

            // auto start fsm
            Fsm.Start("Idle");
        }
        #endregion
    }
}

