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
    using Sandbox.Services;
    
    public struct OnGraphLoginSignal
    {

    }

    public struct OnGraphQLDone
    {

    }

    public class GraphQLDataFlow : BaseService
    {
        private Fsm Fsm;
        
        #region Fsm Events
        private readonly string CONTINUE = "Continue";
        private readonly string ON_LOGIN = "OnLogin";
        private readonly string ON_CONFIGURE = "OnConfigure";
        private readonly string ON_REQUEST = "OnRequest";
        #endregion

        #region Fsm States
        private readonly string IDLE = "Idle";
        private readonly string LOGIN = "Login";
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

        #region Fsm
        private void PrepareFsm()
        {
            Fsm = new Fsm("GraphDataFlow");

            FsmState idle = Fsm.AddState(IDLE);
            FsmState login = Fsm.AddState(LOGIN);
            FsmState configure = Fsm.AddState(CONFIGURE);
            FsmState requests = Fsm.AddState(REQUESTS);
            FsmState done = Fsm.AddState(DONE);

            idle.AddTransition(ON_LOGIN, login);
            idle.AddTransition(ON_REQUEST, requests);

            login.AddTransition(ON_CONFIGURE, configure);
            configure.AddTransition(CONTINUE, idle);
            requests.AddTransition(CONTINUE, done);
            done.AddTransition(CONTINUE, idle);

            login.AddAction(new FsmDelegateAction(login, delegate (FsmState owner)
            {
                this.Publish(new GraphQLLoginRequestSignal() { UniqueId = Platform.DeviceId });
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
                Fsm.SendEvent(CONTINUE);
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
        #endregion

        #region Signals
        private void PrepareSignals()
        {
            Action<bool, GraphQLRequestType, string> Assert = delegate (bool result, GraphQLRequestType request, string map)
            {
                Assertion.Assert(result, D.ERROR + "GraphDataFlow::PrepareSignals {0} should contain {1} resolver!\n", map, GraphQLRequestType.LOGIN);
            };
            
            /*
            SuccessActionMap[GraphQLRequestType.LOGIN].AddListener(LoginSuccessResolver);
            SuccessActionMap[GraphQLRequestType.CONFIGURE].AddListener(ConfigureSuccessResolver);
            SuccessActionMap[GraphQLRequestType.ANNOUNCEMENTS].AddListener(AnnouncementSuccessResolver);
            //*/
           
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
                .Subscribe(_ => Fsm.SendEvent(ON_LOGIN))
                .AddTo(this);
        }
        #endregion

        #region Success Resolvers
        public void LoginSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            Fsm.SendEvent(ON_CONFIGURE);
        }

        public void ConfigureSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            Fsm.SendEvent(CONTINUE);
        }

        public void AnnouncementSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            Fsm.SendEvent(CONTINUE);
        }
        #endregion

        #region Failed Resolvers
        public void LoginFailResolver(GraphQLRequestFailedSignal result)
        {
        }

        public void ConfigureFailResolver(GraphQLRequestFailedSignal result)
        {
        }

        public void AnnouncementFailResolver(GraphQLRequestFailedSignal result)
        {
        }
        #endregion

        #region Debug
        [Button(25)]
        public void TestLogin()
        {
            this.Publish(new OnGraphLoginSignal());
        }
        #endregion
    }
}