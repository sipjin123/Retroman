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
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.GraphQL;
    using Sandbox.Services;

    public struct OnInitLobbySignal
    {

    }

    public struct OnConnectedToLobby
    {

    }

    public struct OnDisconnectedToLobby
    {

    }
    
    public class LobbyFlow : BaseService
    {
        private Fsm Fsm;

        [SerializeField, ShowInInspector]
        private string CurrState;

        #region Fsm Events
        private readonly string CONTINUE = "Continue";
        private readonly string ON_INIT = "OnInit";
        private readonly string ON_CONNECT = "OnConnect";
        private readonly string ON_DISCONNECT = "OnDisconnect";
        #endregion

        #region Fsm States
        private readonly string IDLE = "Idle";
        private readonly string INIT = "Init";
        private readonly string CONNECT = "Connect";
        private readonly string CONNECTED = "Connected";
        private readonly string DISCONNECT = "Disconnected";
        private readonly string DONE = "Done";
        #endregion
        
        private ObscuredString Token;
        private LobbyActiveInstance ActiveLobby;
        private LobbyInfo Lobby;
        
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
            Fsm = new Fsm("LobbyFlow");

            FsmState idle = Fsm.AddState(IDLE);
            FsmState init = Fsm.AddState(INIT);
            FsmState connect = Fsm.AddState(CONNECT);
            FsmState connected = Fsm.AddState(CONNECTED);
            FsmState disconnect = Fsm.AddState(DISCONNECT);

            idle.AddTransition(ON_INIT, init);

            init.AddTransition(ON_CONNECT, connect);

            connect.AddTransition(ON_INIT, init);
            connect.AddTransition(CONTINUE, connected);
            connect.AddTransition(ON_DISCONNECT, disconnect);

            connected.AddTransition(ON_DISCONNECT, disconnect);

            disconnect.AddTransition(ON_INIT, init);
            
            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                
            }));

            init.AddAction(new FsmDelegateAction(init, delegate (FsmState owner)
            {
                this.Publish(new OnLobbyActiveInstanceSignal() { Token = Token });
            }));

            connect.AddAction(new FsmDelegateAction(connect, delegate (FsmState owner)
            {
                // Do OpenLobby action if there is no active lobby
                if (ActiveLobby.has_instance)
                {
                    Lobby = ActiveLobby.lobby;
                    LobbyStatus status = Lobby.status.ToEnum<LobbyStatus>();

                    switch (status)
                    {
                        case LobbyStatus.OPEN:
                            Debug.LogFormat(D.LOBBY + "LobbyFlow::Connect ConnectingToLobby:{0}\n", Lobby.ToString());
                            this.Publish(new OnConnectToLobbySignal() { Lobby = Lobby });
                            break;
                        default:
                            Debug.LogErrorFormat(D.LOBBY + "TODO: Close the session and reconnect to server! LobbyStatus:{0}\n", status);
                            this.Publish(new OnCloseLobbySessionSignal() { Token = Token, LobbyId = Lobby.id });
                            break;
                    }
                }
                else
                {
                    this.Publish(new OnLobbyOpenSignal() { Token = Token, Slug = "session", Name = "Session" });
                }
            }));

            connected.AddAction(new FsmDelegateAction(connected, delegate (FsmState owner)
            {
                this.Publish(new OnConnectedToLobby());
            }));

            disconnect.AddAction(new FsmDelegateAction(disconnect, delegate (FsmState owner)
            {
                this.Publish(new OnDisconnectedToLobby());
            }));

            Fsm.Start(IDLE);

            Fsm.CurrState
                .Subscribe(_ => CurrState = _.GetName())
                .AddTo(this);
        }
        #endregion

        #region Signals
        private void PrepareSignals()
        {
            this.Receive<OnInitLobbySignal>()
                .Where(_ => string.IsNullOrEmpty(Token.GetDecrypted()))
                .Subscribe(_ => Assertion.Assert(false, "Token should never be empty or null!"))
                .AddTo(this);

            this.Receive<OnInitLobbySignal>()
                .Where(_ => !string.IsNullOrEmpty(Token.GetDecrypted()))
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.LOBBY + "OnInitLobbySignal {0}->{1}\n", Fsm.GetCurrentStateName(), ON_INIT);
                    Fsm.SendEvent(ON_INIT);
                })
                .AddTo(this);

            this.Receive<OnDisconnectedToSocketsSignal>()
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.LOBBY + "OnDisconnectedToSocketsSignal {0}->{1}\n", Fsm.GetCurrentStateName(), ON_DISCONNECT);
                    Fsm.SendEvent(ON_DISCONNECT);
                })
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ =>
                {
                    Token = _.GetData<ObscuredString>();

                    // Uncomment for auto init
                    //this.Publish(new OnInitLobbySignal());
                })
                .AddTo(this);

            // Receiver of OnLobbyActiveInstanceSignal
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOBBY_ACTIVE_INSTANCE)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.LOBBY + "GraphQLRequestSuccessfulSignal::{0} {1}->{2}\n", _.Type, Fsm.GetCurrentStateName(), ON_CONNECT);
                    ActiveLobby = _.GetData<LobbyActiveInstance>();
                    Fsm.SendEvent(ON_CONNECT);
                })
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOBBY_OPEN)
                .Subscribe(_ =>
                {
                    Lobby = _.GetData<LobbyInfo>();

                    Debug.LogFormat(D.LOBBY + "GraphQLRequestSuccessfulSignal::{0} {1}-> ConnectingToLobby:{2}\n", _.Type, Fsm.GetCurrentStateName(), Lobby.ToString());
                    
                    this.Publish(new OnConnectToLobbySignal() { Lobby = Lobby });
                })
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOBBY_JOIN || _.Type == GraphQLRequestType.LOBBY_RECONNECT)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.LOBBY + "GraphQLRequestSuccessfulSignal::{0} {1}->{2}\n", _.Type, Fsm.GetCurrentStateName(), CONTINUE);
                    Fsm.SendEvent(CONTINUE);
                })
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOBBY_CLOSE)
                .Subscribe(_ => this.Publish(new OnInitLobbySignal()))
                .AddTo(this);

            this.Receive<OnConnectedToSocketsSignal>()
                .Where(_ => ActiveLobby != null)
                .Where(_ => ActiveLobby.has_instance)
                .Where(_ => ActiveLobby.lobby.id.Equals(_.LobbyId))
                .Subscribe(_ =>
                {
                    LobbyInfo lobby = ActiveLobby.lobby;
                    IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
                    request.AddParameter(SocketConnections.ID, lobby.id);
                    string socketId = QuerySystem.Complete<string>();
                    this.Publish(new OnLobbyReconnectSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
                })
                .AddTo(this);

            this.Receive<OnConnectedToSocketsSignal>()
                .Where(_ => ActiveLobby != null)
                .Where(_ => !ActiveLobby.has_instance)
                .Where(_ => Lobby != null)
                .Where(_ => Lobby.id.Equals(_.LobbyId))
                .Subscribe(_ =>
                {
                    LobbyInfo lobby = Lobby;
                    IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
                    request.AddParameter(SocketConnections.ID, lobby.id);
                    string socketId = QuerySystem.Complete<string>();
                    this.Publish(new OnLobbyJoinSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
                })
                .AddTo(this);
            
            this.Receive<OnLobbyStatusChangedSignal>()
                .Where(_ => _.Status == LobbyStatus.POST_GAME || _.Status == LobbyStatus.CLOSED || _.Status == LobbyStatus.TERMINATED)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.LOBBY + "OnLobbyStatusChangedSignal::{0} {1}->{2}\n", _.Status, Fsm.GetCurrentStateName(), ON_DISCONNECT);
                    Fsm.SendEvent(ON_DISCONNECT);
                })
                .AddTo(this);
            
        }
        #endregion

        [Button(25)]
        public void Init()
        {
            this.Publish(new OnInitLobbySignal());
        }
        
        [Button(25)]
        public void End()
        {
            this.Publish(new OnCloseSessionSignal());
        }
    }
}