using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Fsm;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    using Sandbox.Facebook;
    using Sandbox.Services;
    using Sirenix.Utilities;

    public struct OnGraphLoginSignal
    {
        public bool IsGuest { get; set; }
    }

    public struct OnGraphQLDone
    {

    }

    public class GraphQLDataFlow : BaseService
    {
        [SerializeField]
        private Fsm Fsm;
        
        #region Fsm Events
        private readonly string CONTINUE = "Continue";
        private readonly string ON_LOGIN = "OnLogin";
        private readonly string ON_LOGIN_FB = "OnLoginFb";
        private readonly string ON_CONFIGURE = "OnConfigure";
        private readonly string ON_REQUEST = "OnRequest";
        private readonly string FAILED = "OnRequest";
        #endregion

        #region Fsm States
        private readonly string IDLE = "Idle";
        private readonly string LOGIN = "Login";
        private readonly string LOGIN_FB = "LoginFb";
        private readonly string CONFIGURE = "Configure";
        private readonly string REQUESTS = "Requests";
        private readonly string DONE = "Done";
        #endregion

        #region Action Map
        [SerializeField]
        private Dictionary<GraphQLRequestType, Action<GraphQLRequestSuccessfulSignal>> SuccessActionMap = new Dictionary<GraphQLRequestType, Action<GraphQLRequestSuccessfulSignal>>();
        
        [SerializeField]
        private Dictionary<GraphQLRequestType, Action<GraphQLRequestFailedSignal>> FailedActionMap = new Dictionary<GraphQLRequestType, Action<GraphQLRequestFailedSignal>>();
        #endregion
        
        #region Services
        public override void InitializeService()
        {
            PrepareSignals();
            PrepareFsm();

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            PrepareSignals();
            PrepareFsm();

            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
            foreach(KeyValuePair<GraphQLRequestType, Action<GraphQLRequestSuccessfulSignal>> pair in SuccessActionMap)
            {
                //pair.Value.RemoveAllListeners();
            }

            foreach (KeyValuePair<GraphQLRequestType, Action<GraphQLRequestFailedSignal>> pair in FailedActionMap)
            {
                //pair.Value.RemoveAllListeners();
            }
        }
        #endregion

        [SerializeField]
        private List<string> RequestQueue;

        #region Fsm
        private void PrepareFsm()
        {
            Fsm = new Fsm("GraphDataFlow");

            FsmState idle = Fsm.AddState(IDLE);
            FsmState login = Fsm.AddState(LOGIN);
            FsmState loginFb = Fsm.AddState(LOGIN_FB);
            FsmState configure = Fsm.AddState(CONFIGURE);
            FsmState requests = Fsm.AddState(REQUESTS);
            FsmState done = Fsm.AddState(DONE);

            // Fsm transitions
            idle.AddTransition(ON_LOGIN, login);
            idle.AddTransition(ON_LOGIN_FB, loginFb);
            idle.AddTransition(ON_REQUEST, requests);

            login.AddTransition(ON_CONFIGURE, configure);
            login.AddTransition(FAILED, idle);

            loginFb.AddTransition(ON_CONFIGURE, configure);
            loginFb.AddTransition(FAILED, idle);

            configure.AddTransition(CONTINUE, idle);
            configure.AddTransition(FAILED, idle);

            requests.AddTransition(CONTINUE, done);
            requests.AddTransition(FAILED, idle);

            done.AddTransition(CONTINUE, idle);

            // DEBUG Actions
            Action<FsmState> AddLogAction = state => state.AddAction(new EnterAction(state, owner => Debug.LogFormat(D.GRAPHQL + "GraphQLDataFlow::{0}\n", owner.GetName())));
            FsmState[] states = Fsm.States;
            states.ForEach(state => AddLogAction(state));

            idle.AddAction(new UpdateAction(idle, owner =>
            {
                if (RequestQueue.Count > 0)
                {
                    string evt = RequestQueue.FirstOrDefault();
                    RequestQueue.RemoveAt(0);
                    Fsm.SendEvent(evt);
                }
            }));

            // Actions
            login.AddAction(new FsmDelegateAction(login, delegate (FsmState owner)
            {
                this.Publish(new GraphQLLoginRequestSignal() { UniqueId = Platform.DeviceId });
            }));

            loginFb.AddAction(new FsmDelegateAction(loginFb, delegate (FsmState owner)
            {
                this.Publish(new GraphQLFBLoginRequestSignal() { UniqueId = Platform.DeviceId, FacebookToken = QuerySystem.Query<string>(FBID.UserFacebookToken) });
            }));

            configure.AddAction(new FsmDelegateAction(configure, delegate (FsmState owner)
            {
                this.Publish(new GraphQLConfigureRequestSignal() { Token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN) });
            }));

            // TODO: +AS:06192018 Queue the graphql requests!
            requests.AddAction(new FsmDelegateAction(requests, delegate (FsmState owner)
            {
                // NOTE: +AS:06192018 Test only
                this.Publish(new GraphQLAnnouncementRequestSignal() { Token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), ShowUpcoming = true });
            }));

            done.AddAction(new FsmDelegateAction(done, delegate (FsmState owner)
            {
                SendEvent(CONTINUE);
            }));

            Fsm.Start(IDLE);
            Fsm.CurrState
                .Where(state => state == done)
                .Subscribe(_ => this.Publish(new OnGraphQLDone()))
                .AddTo(this);

            this.UpdateAsObservable()
                .Subscribe(_ => Fsm.Update())
                .AddTo(this);
        }

        private void SendEvent(string evt)
        {
            // Skip
            if (Fsm.HasTransition(evt))
            {
                Fsm.SendEvent(evt);
            }
            // Queue
            else
            {
                RequestQueue = RequestQueue ?? new List<string>();
                RequestQueue.Add(evt);
            }
        }

        #endregion

        #region Signals
        private void PrepareSignals()
        {
            Action<bool, GraphQLRequestType, string> Assert = delegate (bool result, GraphQLRequestType request, string map)
            {
                Assertion.Assert(result, D.ERROR + "GraphDataFlow::PrepareSignals {0} should contain {1} resolver!\n", map, GraphQLRequestType.LOGIN);
            };
            
            Assert(SuccessActionMap.ContainsKey(GraphQLRequestType.LOGIN), GraphQLRequestType.LOGIN, "SuccessActionMap");
            Assert(SuccessActionMap.ContainsKey(GraphQLRequestType.CONFIGURE), GraphQLRequestType.CONFIGURE, "SuccessActionMap");
            Assert(SuccessActionMap.ContainsKey(GraphQLRequestType.ANNOUNCEMENTS), GraphQLRequestType.ANNOUNCEMENTS, "SuccessActionMap");

            Assert(FailedActionMap.ContainsKey(GraphQLRequestType.LOGIN), GraphQLRequestType.LOGIN, "FailedActionMap");
            Assert(FailedActionMap.ContainsKey(GraphQLRequestType.CONFIGURE), GraphQLRequestType.CONFIGURE, "FailedActionMap");
            Assert(FailedActionMap.ContainsKey(GraphQLRequestType.ANNOUNCEMENTS), GraphQLRequestType.ANNOUNCEMENTS, "FailedActionMap");
            
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => SuccessActionMap.ContainsKey(_.Type))
                .Subscribe(_ => SuccessActionMap[_.Type].Invoke(_))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => !SuccessActionMap.ContainsKey(_.Type))
                .Subscribe(_ => Debug.LogWarningFormat(D.WARNING + "GaraphQLDataFlow::GraphQLRequestSuccessfulSignal::{0} has no Success resolver.\n", _.Type))
                .AddTo(this);

            this.Receive<GraphQLRequestFailedSignal>()
                .Where(_ => FailedActionMap.ContainsKey(_.Type))
                .Subscribe(_ => FailedActionMap[_.Type].Invoke(_))
                .AddTo(this);

            this.Receive<GraphQLRequestFailedSignal>()
                .Where(_ => !FailedActionMap.ContainsKey(_.Type))
                .Subscribe(_ => Debug.LogWarningFormat(D.WARNING + "GaraphQLDataFlow::GraphQLRequestFailedSignal::{0} has no Fail resolver.\n", _.Type))
                .AddTo(this);

            this.Receive<OnGraphLoginSignal>()
                .Where(_ => _.IsGuest)
                .Subscribe(_ => SendEvent(ON_LOGIN))
                .AddTo(this);

            this.Receive<OnGraphLoginSignal>()
                .Where(_ =>
                {
                    return !_.IsGuest;
                })
                .Subscribe(_ =>
                {
                    SendEvent(ON_LOGIN_FB);
                })
                .AddTo(this);

            this.Receive<OnGraphLoginSignal>()
                .Subscribe(_ => Debug.LogFormat(D.FGC + "GraphQLDataFlow::OnGraphLoginSignal IsGuest:{0}\n", _.IsGuest))
                .AddTo(this);
        }
        #endregion

        #region Success Resolvers
        public void LoginSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            SendEvent(ON_CONFIGURE);
        }

        public void ConfigureSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            SendEvent(CONTINUE);
        }

        public void AnnouncementSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            SendEvent(CONTINUE);
        }
        #endregion

        #region Failed Resolvers
        public void LoginFailResolver(GraphQLRequestFailedSignal result)
        {
            SendEvent(FAILED);
        }

        public void ConfigureFailResolver(GraphQLRequestFailedSignal result)
        {
            SendEvent(FAILED);
        }

        public void AnnouncementFailResolver(GraphQLRequestFailedSignal result)
        {
            SendEvent(FAILED);
        }
        #endregion

        #region Debug
        [Button(ButtonSizes.Medium)]
        public void TestLogin()
        {
            this.Publish(new OnGraphLoginSignal() { IsGuest = true });
        }
        #endregion
    }
}