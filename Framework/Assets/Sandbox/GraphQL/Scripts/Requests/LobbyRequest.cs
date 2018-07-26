using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;
using Common.Query;

using Framework;

#region Lobby and Socket Data
namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.SocketIo;
    
    [Serializable]
    public class LobbyJoinReconnect : IJson
    {
        public string id;
    }

    [Serializable]
    public class LobbyInfo : IJson
    {
        public string id;
        public string base_url;
        public string port;
        public string status;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class LobbyActiveInstance : IJson
    {
        public bool has_instance;
        public LobbyInfo lobby;
    }

    public enum LobbyError
    {
        RTS001, // Player already in lobby
        RTS002, // Can not join lobby
        RTS003, // Incorrect password for lobby
        RTS004, // Player not added to lobby
        RTS005, // Lobby is closed
        RTS006, // Lobby is full
        RTS007, // Lobby not found
        RTS008, // Lobby can not be opened by player
        RTS009, // Lobby can not be started
        RTS010, // Lobby is already instantiated
        RTS011, // Registry entry not found
        RTS012, // A token is required for this registry entry
        RTS013, // Player does not own this registry entry
        RTS014, // Lobby instance does not exist
        RTS015, // Player does not host lobby
    }
}
#endregion

#region Lobby and Socket Signals
namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.SocketIo;

    public struct OnLobbyActiveInstanceSignal
    {
        public ObscuredString Token;
    }

    public struct OnCloseLobbySessionSignal
    {
        public ObscuredString Token;
        public ObscuredString LobbyId;
    }

    public struct OnLobbyOpenSignal
    {
        public ObscuredString Token;
        public ObscuredString Slug;
        public ObscuredString Name;
    }

    public struct OnLobbyJoinSignal
    {
        public ObscuredString Token;
        public ObscuredString LobbyId;
        public ObscuredString SocketId;
    }

    public struct OnLobbyReconnectSignal
    {
        public ObscuredString Token;
        public ObscuredString LobbyId;
        public ObscuredString SocketId;
    }

    public struct OnConnectToLobbySignal
    {
        public LobbyInfo Lobby;
    }

    public struct OnStartGameSignal
    {
    }

    public struct OnSetInitialTarget
    {
        public ObscuredInt Timestamp;
        public ObscuredFloat3 Target;
    }

    public struct OnSendScoreSignal
    {
        public ObscuredInt Timestamp;
        public ObscuredFloat3 Source;
        public ObscuredFloat3 Target;
        public ObscuredFloat3 Gravity;
        public ObscuredFloat3 Velocity;
        public ObscuredFloat Duration;
        public ShotType Shot;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", Source.GetDecrypted(), Target.GetDecrypted(), Gravity.GetDecrypted(), Velocity.GetDecrypted(), Duration.GetDecrypted(), Shot.ToInt());
        }
    }

    public struct OnSendMissSignal
    {
        public ObscuredInt Timestamp;
    }

    public struct OnEndGameSignal
    {

    }

    public struct OnCloseSessionSignal
    {

    }

    public struct OnCheckLobbyStatus
    {

    }
}
#endregion

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.SocketIo;
    
    public class LobbyRequest : UnitRequest
    {
        [SerializeField, ShowInInspector]
        private LobbyActiveInstance ActiveLobby;

        [SerializeField, ShowInInspector]
        private List<LobbyInfo> Lobbies;
        
        private List<ObscuredString> JoinedLobbies;
        
        protected ObscuredString Token;
        
        private Timestamp Timestamp;

        private List<ShotType> Shots;

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            ActiveLobby = null;
            Lobbies = new List<LobbyInfo>();
            JoinedLobbies = new List<ObscuredString>();
            Timestamp = new Timestamp("StartGame");
            Shots = new List<ShotType>() { ShotType.Normal, ShotType.Perfect };

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => Token = _.GetData<ObscuredString>())
                .AddTo(this);

            this.Receive<OnLobbyActiveInstanceSignal>()
                .Subscribe(_ => LobbyActiveInstance(_.Token.GetDecrypted()))
                .AddTo(this);

            this.Receive<OnLobbyOpenSignal>()
                .Subscribe(_ => LobbyOpen(_.Token.GetDecrypted(), _.Slug, _.Name))
                .AddTo(this);

            this.Receive<OnLobbyJoinSignal>()
                .Subscribe(_ => JoinGameSessionLobby(_.Token.GetDecrypted(), _.LobbyId, _.SocketId))
                .AddTo(this);

            this.Receive<OnLobbyReconnectSignal>()
                .Subscribe(_ => ReconnectGameSessionLobby(_.Token.GetDecrypted(), _.LobbyId, _.SocketId))
                .AddTo(this);

            this.Receive<OnCloseLobbySessionSignal>()
                .Subscribe(_ => CloseLobbySession(_.Token.GetDecrypted(), _.LobbyId))
                .AddTo(this);
            
            this.Receive<OnLobbyStatusChangedSignal>()
                .Where(_ => _.Status == LobbyStatus.IN_GAME)
                .Subscribe(_ =>
                {
                    // Start recording after receiving the IN_GAME status
                    Timestamp.Record();
                })
                .AddTo(this);
        }

        #region Requests
        //http://192.168.22.29/game-backend/knowledge-base/wikis/GNT-integration-guide#check-if-player-has-an-active-session
        public void LobbyActiveInstance(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("lobby_activeInstance");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("has_instance");
            ret.Add("lobby", new Return("id", "base_url", "port", "status"));

            ProcessRequest(GraphInfo, builder.ToString(), OnLobbyActiveInstance);
        }

        public void LobbyOpen(string token, string slug, string name)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_open");
            func.AddString("token", token);
            func.AddString("slug", slug);
            func.AddString("name", name);
            Return ret = builder.CreateReturn("id", "base_url", "port", "status");

            ProcessRequest(GraphInfo, builder.ToString(), OnLobbyOpen);
        }

        public void JoinGameSessionLobby(string token, string lobbyId, string socketId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_join");
            func.AddString("token", token);
            func.AddString("id", lobbyId);
            func.AddString("socket_id", socketId);
            Return ret = builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), OnJoinGameSessionLobby);
        }
        
        public void ReconnectGameSessionLobby(string token, string lobbyId, string socketId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_reconnect");
            func.AddString("token", token);
            func.AddString("id", lobbyId);
            func.AddString("socket_id", socketId);
            Return ret = builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), OnReconnectToLobby);
        }

        public void CloseLobbySession(string token, string lobbyId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_close");
            func.AddString("token", token);
            func.AddString("id", lobbyId);

            ProcessRequest(GraphInfo, builder.ToString(), OnCloseLobbySession);
        }
        #endregion

        #region Parsers
        public void OnLobbyActiveInstance(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_ACTIVE_INSTANCE });
            }
            else
            {
                ActiveLobby = result.Result.data.lobby_activeInstance;

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_ACTIVE_INSTANCE, Data = ActiveLobby });
                
                /* +AS:20180625 TODO: Remove this snippet after the LobbyFlow setup.
                // Do OpenLobby action if there is no active lobby
                if (ActiveLobby.has_instance)
                {
                    LobbyInfo lobby = ActiveLobby.lobby;
                    LobbyStatus status = lobby.status.ToEnum<LobbyStatus>();
                    Lobbies.Add(lobby);
                    
                    if (status == LobbyStatus.OPEN)
                    {
                        this.Publish(new OnConnectToLobbySignal() { Lobby = lobby });
                    }
                    else
                    {
                        // TODO: +AS:20180625 Reconnect here
                        Debug.LogErrorFormat("TODO: Do reconnect to server there! LobbyStatus:{0}\n", status);
                        this.Publish(new OnConnectToLobbySignal() { Lobby = lobby });
                    }
                }
                else
                {
                    this.Publish(new OnLobbyOpenSignal() { Token = Token, Slug = "session", Name = "Session" });
                }
                //*/
            }
        }

        // {"data":{"lobby_open":null},"errors":[{"message":"RTS007: Lobby not found","locations":[{"line":1,"column":10}],"path":["lobby_open"]}]}
        // {"data":{"lobby_open":null},"errors":[{"message":"RTS001: Player already in lobby","locations":[{"line":1,"column":10}],"path":["lobby_open"]}]}
        public void OnLobbyOpen(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_OPEN });

                // Sample catch all errors
                CatchError(result.Result.errors, delegate (string message, string request)
                {
                    Debug.LogErrorFormat(D.ERROR + "Lobby::OnLobbyOpen Request:{0} Message:{1}\n", request, message);
                });

                // Sample catch error with filters
                CatchError(LobbyError.RTS001, result.Result.errors, delegate (string message, string request)
                {
                    Debug.LogErrorFormat(D.ERROR + "Lobby::OnLobbyOpen Request:{0} Message:{1}\n", request, message);
                });
            }
            else
            {
                LobbyInfo lobby = result.Result.data.lobby_open;
                Lobbies.Add(lobby);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_OPEN, Data = lobby });

                Debug.LogFormat(D.LOBBY + "Registering new Lobby! LobbyId:{0}\n", lobby.id);

                /* +AS:20180625 TODO: Remove this snippet after the LobbyFlow setup.
                this.Publish(new OnConnectToLobbySignal() { Lobby = lobby });
                //*/
            }
        }

        public void OnCloseLobbySession(GraphResult result)
        {
            this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_CLOSE });
        }

        // {"data":{"lobby_join":{"id":"ddead560-7484-11e8-8610-e7f5777e7209"}}}
        // {"data":{"lobby_join":{"id":"b202fa60-753b-11e8-9bf2-6bce97719484"}}}
        public void OnJoinGameSessionLobby(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_JOIN });
            }
            else
            {
                Assertion.AssertNotNull(JoinedLobbies, "JoinedLobbies");
                Assertion.AssertNotNull(result, "result");
                Assertion.AssertNotNull(result.Result, "result.Result");
                Assertion.AssertNotNull(result.Result.data, "result.Result.data");
                Assertion.AssertNotNull(result.Result.data.lobby_join, "result.Result.data.lobby_join");
                Assertion.AssertNotNull(result.Result.data.lobby_join.id, "result.Result.data.lobby_join.id");
                JoinedLobbies.Add(result.Result.data.lobby_join.id);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_JOIN, Data = JoinedLobbies.LastOrDefault().GetDecrypted() });
            }
        }

        // {"data":{"lobby_reconnect":{"id":"34307fa0-785f-11e8-b46b-1f787b84fa26"}}}
        public void OnReconnectToLobby(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_RECONNECT });
            }
            else
            {
                Assertion.AssertNotNull(JoinedLobbies, "JoinedLobbies");
                Assertion.AssertNotNull(result, "result");
                Assertion.AssertNotNull(result.Result, "result.Result");
                Assertion.AssertNotNull(result.Result.data, "result.Result.data");
                Assertion.AssertNotNull(result.Result.data.lobby_reconnect, "result.Result.data.lobby_reconnect");
                Assertion.AssertNotNull(result.Result.data.lobby_reconnect.id, "result.Result.data.lobby_reconnect.id");
                JoinedLobbies.Add(result.Result.data.lobby_reconnect.id);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_RECONNECT, Data = JoinedLobbies.LastOrDefault().GetDecrypted() });
            }
        }
        #endregion

        #region Debug
        [Button(25)]
        public void TestLobbyActiveInstance()
        {
            this.Publish(new OnLobbyActiveInstanceSignal() { Token = Token });
        }

        [Button(25)]
        public void TestLobbyOpen()
        {
            this.Publish(new OnLobbyOpenSignal() { Token = Token, Slug = "session", Name = "Session" });
        }

        [Button(25)]
        public void TestLobbyJoin()
        {
            /* Socket message
            {
                "type": "player_joined",
                "data": {
                    "player_id": "430ef9c0-22b5-11e8-9cff-d94e89226117",
                    "message": "430ef9c0-22b5-11e8-9cff-d94e89226117 joined"
                }
            }
            //*/

            LobbyInfo lobby = Lobbies.LastOrDefault();
            IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
            request.AddParameter(SocketConnections.ID, lobby.id);
            string socketId = QuerySystem.Complete<string>();
            this.Publish(new OnLobbyJoinSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
        }

        [Button(25)]
        public void TestReconnectToLobby()
        {
            /* Socket message
            {
                "type": "player_joined",
                "data": {
                    "player_id": "430ef9c0-22b5-11e8-9cff-d94e89226117",
                    "message": "430ef9c0-22b5-11e8-9cff-d94e89226117 joined"
                }
            }
            //*/

            LobbyInfo lobby = Lobbies.LastOrDefault();
            IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
            request.AddParameter(SocketConnections.ID, lobby.id);
            string socketId = QuerySystem.Complete<string>();
            this.Publish(new OnLobbyReconnectSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
        }

        [Button(25)]
        public void TestStartGame()
        {
            // emit "message": /start
            this.Publish(new OnStartGameSignal());
        }

        [Button(25)]
        public void TestLobbyStatus()
        {
            // emit "message": /start
            this.Publish(new OnCheckLobbyStatus());
        }

        [Button(25)]
        public void TestSetInitialTarget()
        {
            this.Publish(new OnSetInitialTarget()
            {
                Timestamp = Timestamp.Lapsed(),
                Target = new Float3(0.0f, 3.4f, 9.7f),    // target
            });
        }
        
        [Button(25)]
        public void TestSendScore()
        {
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
                Shot = Shots.Random(),                          // shot type
            });
        }

        [Button(25)]
        public void TestSendMiss()
        {
            // emit "message": - <timestamp>
            // emit "message": - 14076.95
            this.Publish(new OnSendMissSignal()
            {
                Timestamp = Timestamp.Lapsed()
            });
        }

        [Button(25)]
        public void TestEndGame()
        {
            // emit "message": /end
            this.Publish(new OnEndGameSignal());
        }

        [Button(25)]
        public void TestCloseSession()
        {
            // emit "message": /leave
            this.Publish(new OnCloseSessionSignal());
        }

        [Button(25)]
        public void TestCloseLobbySession()
        {
            LobbyInfo lobby = Lobbies.LastOrDefault();
            this.Publish(new OnCloseLobbySessionSignal() { Token = Token, LobbyId = lobby.id });
        }

        // http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:5000/_lobbies
        // http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:5000/_status
        // https://chrome.google.com/webstore/detail/json-viewer/gbmdgpbipfallnflgajpaliibnhdgobh
        #endregion
    }
}
