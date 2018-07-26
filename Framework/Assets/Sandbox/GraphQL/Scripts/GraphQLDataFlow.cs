﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;

using Framework;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

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
        [SerializeField, ShowInInspector]
        private Dictionary<GraphQLRequestType, UnityEvent<GraphQLRequestSuccessfulSignal>> SuccessActionMap = new Dictionary<GraphQLRequestType, UnityEvent<GraphQLRequestSuccessfulSignal>>();
        
        [SerializeField, ShowInInspector]
        private Dictionary<GraphQLRequestType, UnityEvent<GraphQLRequestFailedSignal>> FailedActionMap = new Dictionary<GraphQLRequestType, UnityEvent<GraphQLRequestFailedSignal>>();
        #endregion
        
        private ObscuredString Token;

        #region Services
        public override void InitializeService()
        {
            PrepareSignals();
            PrepareFsm();

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
            foreach(KeyValuePair<GraphQLRequestType, UnityEvent<GraphQLRequestSuccessfulSignal>> pair in SuccessActionMap)
            {
                pair.Value.RemoveAllListeners();
            }

            foreach (KeyValuePair<GraphQLRequestType, UnityEvent<GraphQLRequestFailedSignal>> pair in FailedActionMap)
            {
                pair.Value.RemoveAllListeners();
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
                this.Publish(new GraphQLLoginRequestSignal() { unique_id = Platform.DeviceId });
            }));

            configure.AddAction(new FsmDelegateAction(configure, delegate (FsmState owner)
            {
                this.Publish(new GraphQLConfigureRequestSignal() { Token = Token });
            }));

            // +AS:06192018 TODO: Queue the graphql requests!
            requests.AddAction(new FsmDelegateAction(requests, delegate (FsmState owner)
            {
                // [TEST]
                this.Publish(new GraphQLAnnouncementRequestSignal() { Token = Token, ShowUpcoming = true });
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

            /*
            SuccessActionMap[GraphQLRequestType.LOGIN].AddListener(LoginSuccessResolver);
            SuccessActionMap[GraphQLRequestType.CONFIGURE].AddListener(ConfigureSuccessResolver);
            SuccessActionMap[GraphQLRequestType.ANNOUNCEMENTS].AddListener(AnnouncementSuccessResolver);
            //*/
            
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
            Token = result.GetData<ObscuredString>();
            Fsm.SendEvent(ON_CONFIGURE);
        }
        #endregion

        #region Failed Resolvers
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