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

namespace Sandbox.SocketIo
{
    using Sandbox.GraphQL;
    using Sandbox.Security;
    using Sandbox.Services;

    public struct OnShotSignal
    {
        public bool Miss;
        public Float3 Source;           // source
        public Float3 Target;           // target
        public Float3 Gravity;          // gravity
        public Float3 Velocity;         // velocity
        public float Duration;          // duration
        public ShotType Shot;           // shot type
    }

    public class GameFlow : BaseService
    {
        private Fsm Fsm;

        [SerializeField, ShowInInspector]
        private string CurrState;

        #region Fsm Events
        private readonly string ON_START_GAME = "OnStartGame";
        private readonly string ON_GAME_LOOP = "OnGameLoop";
        private readonly string ON_END = "OnEnd";
        #endregion

        #region Fsm States
        private readonly string IDLE = "Idle";
        private readonly string START = "Start";
        private readonly string GAME_LOOP = "GameLoop";
        private readonly string END = "End";
        #endregion

        private Timestamp Timestamp;

        #region Services
        public override void InitializeService()
        {
            PrepareFsm();
            PrepareSignals();

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
        }
        #endregion

        #region Fsm
        private void PrepareFsm()
        {
            Fsm = new Fsm("GameFlow");

            FsmState idle = Fsm.AddState(IDLE);
            FsmState start = Fsm.AddState(START);
            FsmState loop = Fsm.AddState(GAME_LOOP);
            FsmState end = Fsm.AddState(END);

            idle.AddTransition(ON_START_GAME, start);

            start.AddTransition(ON_GAME_LOOP, loop);

            loop.AddTransition(ON_END, end);

            end.AddTransition(ON_START_GAME, start);

            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
            }));

            start.AddAction(new FsmDelegateAction(start, delegate (FsmState owner)
            {
                this.Publish(new OnStartGameSignal());
            }));

            loop.AddAction(new FsmDelegateAction(loop, delegate (FsmState owner)
            {
                //this.Publish(new OnCheckLobbyStatus());
            }));

            end.AddAction(new FsmDelegateAction(end, delegate (FsmState owner)
            {
                this.Publish(new OnCloseSessionSignal());
            }));

            Fsm.Start(IDLE);

            Fsm.CurrState
                .Subscribe(_ => CurrState = _.GetName())
                .AddTo(this);
            
            this.Receive<OnShotSignal>()
                .Where(_ => Fsm.CurrState.Value == loop)
                .Where(_ => _.Miss == false)
                .Subscribe(_ =>
                {
                    this.Publish(new OnSendScoreSignal()
                    {
                        Timestamp = Timestamp.Lapsed(),
                        Source = _.Source,          
                        Target = _.Target,          
                        Gravity = _.Gravity,        
                        Velocity = _.Velocity,      
                        Duration = _.Duration,      
                        Shot = _.Shot,              
                    });
                })
                .AddTo(this);

            this.Receive<OnShotSignal>()
                .Where(_ => Fsm.CurrState.Value == loop)
                .Where(_ => _.Miss)
                .Subscribe(_ =>
                {
                    this.Publish(new OnSendMissSignal() { Timestamp = Timestamp.Lapsed() });
                })
                .AddTo(this);
        }
        #endregion

        #region Signals
        private void PrepareSignals()
        {
            this.Receive<OnPlayerJoinedSignal>()
                .Delay(TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.GAME + "OnPlayerJoinedSignal {0}->{1}\n", Fsm.GetCurrentStateName(), ON_START_GAME);
                    Fsm.SendEvent(ON_START_GAME);
                })
                .AddTo(this);

            this.Receive<OnPlayerLeftSignal>()
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.GAME + "OnPlayerLeftSignal {0}->{1}\n", Fsm.GetCurrentStateName(), ON_END);
                    Fsm.SendEvent(ON_END);
                })
                .AddTo(this);

            this.Receive<OnLobbyStatusSignal>()
                .Subscribe(_ => Timestamp.Record(_.Elapsed))
                .AddTo(this);

            this.Receive<OnLobbyStatusChangedSignal>()
                .Where(_ => _.Status == LobbyStatus.IN_GAME)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.GAME + "OnLobbyStatusChangedSignal::IN_GAME {0}->{1}\n", Fsm.GetCurrentStateName(), ON_GAME_LOOP);

                    // Start recording after receiving the IN_GAME status
                    Timestamp = new Timestamp("GameSession");
                    Timestamp.Record();

                    this.Publish(new OnSetInitialTarget()
                    {
                        Timestamp = Timestamp.Lapsed(),
                        Target = new Float3(0.0f, 3.4f, 9.7f),
                    });

                    Fsm.SendEvent(ON_GAME_LOOP);
                })
                .AddTo(this);

            this.Receive<OnLobbyStatusChangedSignal>()
                .Where(_ => _.Status == LobbyStatus.POST_GAME || _.Status == LobbyStatus.CLOSED || _.Status == LobbyStatus.TERMINATED)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.GAME + "OnLobbyStatusChangedSignal::{0} {1}->{2}\n", _.Status, Fsm.GetCurrentStateName(), ON_GAME_LOOP);
                    Fsm.SendEvent(ON_END);
                })
                .AddTo(this);
        }
        #endregion

        #region Fire signals
        [Button(25)]
        public void SendScore()
        {
            List<ShotType> shots = new List<ShotType>() { ShotType.Normal, ShotType.Perfect };

            // emit "message": + <timestamp> <data>
            // emit "message": + 14076.95 3.9,1.5,4.0,0.0,3.4,9.7,0.0,-9.8,0.0,-3.3,7.5,4.7,1.2
            this.Publish(new OnSendScoreSignal()
            {
                Timestamp = Timestamp.Lapsed(),
                Source = new Float3(3.9f, 1.5f, 4.0f),          // source
                Target = new Float3(0.0f, 3.4f, 9.7f),          // target
                Gravity = new Float3(0.0f, -9.8f, 0.0f),        // gravity
                Velocity = new Float3(-3.3f, 7.5f, 4.7f),       // velocity
                Duration = 1.2f,                                // duration
                Shot = shots.Random(),                          // shot type
            });
        }

        [Button(25)]
        public void SendMiss()
        {
            this.Publish(new OnSendMissSignal() { Timestamp = Timestamp.Lapsed() });
        }

        [Button(25)]
        public void CheckStatus()
        {
            this.Publish(new OnCheckLobbyStatus());
        }
        #endregion
    }
}