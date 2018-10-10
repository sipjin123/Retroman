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

// Alias
using JProp = Newtonsoft.Json.JsonPropertyAttribute;

#region Lobby and Socket Data
namespace Sandbox.GraphQL
{
    [Serializable]
    public class LobbyJoinReconnect : IJson
    {
        [JProp("id")] public string Id;
    }

    [Serializable]
    public class LobbyInfo : IJson
    {
        [JProp("id")] public string Id;
        [JProp("base_url")] public string Url;
        [JProp("port")] public string Port;
        [JProp("status")] public string Status;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class LobbyActiveInstance : IJson
    {
        [JProp("has_instance")] public bool HasInstance;
        [JProp("lobby")] public LobbyInfo Lobby;
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
    public struct OnLobbyActiveInstanceSignal : IRequestSignal
    {
        public string Token;
    }

    public struct OnCloseLobbySessionSignal : IRequestSignal
    {
        public string Token;
        public string LobbyId;
    }

    public struct OnLobbyOpenSignal : IRequestSignal
    {
        public string Token;
        public string Slug;
        public string Name;
    }

    public struct OnLobbyJoinSignal : IRequestSignal
    {
        public string Token;
        public string LobbyId;
        public string SocketId;
    }

    public struct OnLobbyReconnectSignal : IRequestSignal
    {
        public string Token;
        public string LobbyId;
        public string SocketId;
    }

    public struct OnConnectToLobbySignal : IRequestSignal
    {
        public LobbyInfo Lobby;
    }

    public struct OnStartGameSignal : IRequestSignal
    {
    }

    public struct OnSetInitialTargetSignal : IRequestSignal
    {
        public int Timestamp;
        public Vector3 Target;
    }

    [Serializable]
    public struct OnSendScoreSignal : IRequestSignal, IJson
    {
        public int Timestamp;
        public Vector3 Source;
        public Vector3 Target;
        public Vector3 Gravity;
        public Vector3 Velocity;
        public float Duration;
        //public ShotType Shot;

        public override string ToString()
        {
            //return string.Format("{0},{1},{2},{3},{4},{5}", Source, Target, Gravity, Velocity, Duration, Shot.ToInt());
            return string.Empty;
        }
    }

    public struct OnSendMissSignal : IRequestSignal
    {
        public int Timestamp;
    }

    public struct OnEndGameSignal : IRequestSignal
    {

    }

    public struct OnCloseSessionSignal : IRequestSignal
    {

    }

    public struct OnCheckLobbyStatus : IRequestSignal
    {

    }
}
#endregion

namespace Sandbox.GraphQL
{
    public class LobbyRequest : UnitRequest
    {
        [SerializeField, ShowInInspector]
        private LobbyActiveInstance ActiveLobby;

        [SerializeField, ShowInInspector]
        private List<LobbyInfo> Lobbies;
        
        private List<string> JoinedLobbies;
        
        protected string Token;
        
        private Timestamp Timestamp;

        //private List<ShotType> Shots;

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            ActiveLobby = null;
            Lobbies = new List<LobbyInfo>();
            JoinedLobbies = new List<string>();
            Timestamp = new Timestamp("StartGame");
            //Shots = new List<ShotType>() { ShotType.Normal, ShotType.Perfect };

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => Token = _.GetData<string>())
                .AddTo(this);

            this.Receive<OnLobbyActiveInstanceSignal>()
                .Subscribe(_ => LobbyActiveInstance(_.Token))
                .AddTo(this);

            this.Receive<OnLobbyOpenSignal>()
                .Subscribe(_ => LobbyOpen(_.Token, _.Slug, _.Name))
                .AddTo(this);

            this.Receive<OnLobbyJoinSignal>()
                .Subscribe(_ => JoinGameSessionLobby(_.Token, _.LobbyId, _.SocketId))
                .AddTo(this);

            this.Receive<OnLobbyReconnectSignal>()
                .Subscribe(_ => ReconnectGameSessionLobby(_.Token, _.LobbyId, _.SocketId))
                .AddTo(this);

            this.Receive<OnCloseLobbySessionSignal>()
                .Subscribe(_ => CloseLobbySession(_.Token, _.LobbyId))
                .AddTo(this);
            
            /*
            this.Receive<OnLobbyStatusChangedSignal>()
                .Where(_ => _.Status == LobbyStatus.IN_GAME)
                .Subscribe(_ =>
                {
                    // Start recording after receiving the IN_GAME status
                    Timestamp.Record();
                })
                .AddTo(this);
            //*/
        }

        #region Requests
        private void LobbyActiveInstance(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("lobby_activeInstance");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("has_instance");
            ret.Add("lobby", new Return("id", "base_url", "port", "status"));

            ProcessRequest(GraphInfo, builder.ToString(), OnLobbyActiveInstance);
        }

        private void LobbyOpen(string token, string slug, string name)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_open");
            func.AddString("token", token);
            func.AddString("slug", slug);
            func.AddString("name", name);
            Return ret = builder.CreateReturn("id", "base_url", "port", "status");

            ProcessRequest(GraphInfo, builder.ToString(), OnLobbyOpen);
        }

        private void JoinGameSessionLobby(string token, string lobbyId, string socketId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_join");
            func.AddString("token", token);
            func.AddString("id", lobbyId);
            func.AddString("socket_id", socketId);
            Return ret = builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), OnJoinGameSessionLobby);
        }

        private void ReconnectGameSessionLobby(string token, string lobbyId, string socketId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_reconnect");
            func.AddString("token", token);
            func.AddString("id", lobbyId);
            func.AddString("socket_id", socketId);
            Return ret = builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), OnReconnectToLobby);
        }

        private void CloseLobbySession(string token, string lobbyId)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("lobby_close");
            func.AddString("token", token);
            func.AddString("id", lobbyId);

            ProcessRequest(GraphInfo, builder.ToString(), OnCloseLobbySession);
        }
        #endregion

        #region Parsers
        private void OnLobbyActiveInstance(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_ACTIVE_INSTANCE });
            }
            else
            {
                ActiveLobby = result.Result.Data.ActiveLobby;

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_ACTIVE_INSTANCE, Data = ActiveLobby });

                /* TODO: +AS:20180625 Remove this snippet after the LobbyFlow setup.
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

        private void OnLobbyOpen(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOBBY_OPEN });

                // Sample catch all errors
                CatchError(result.Result.Errors, delegate (string message, string request)
                {
                    Debug.LogErrorFormat(D.ERROR + "Lobby::OnLobbyOpen Request:{0} Message:{1}\n", request, message);
                });

                // Sample catch error with filters
                CatchError(LobbyError.RTS001, result.Result.Errors, delegate (string message, string request)
                {
                    Debug.LogErrorFormat(D.ERROR + "Lobby::OnLobbyOpen Request:{0} Message:{1}\n", request, message);
                });
            }
            else
            {
                LobbyInfo lobby = result.Result.Data.OpenedLobby;
                Lobbies.Add(lobby);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_OPEN, Data = lobby });

                Debug.LogFormat(D.LOBBY + "Registering new Lobby! LobbyId:{0}\n", lobby.Id);

                /* TODO: +AS:20180625 Remove this snippet after the LobbyFlow setup.
                this.Publish(new OnConnectToLobbySignal() { Lobby = lobby });
                //*/
            }
        }

        private void OnCloseLobbySession(GraphResult result)
        {
            this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_CLOSE });
        }
        
        private void OnJoinGameSessionLobby(GraphResult result)
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
                Assertion.AssertNotNull(result.Result.Data, "result.Result.data");
                Assertion.AssertNotNull(result.Result.Data.JoinedLobby, "result.Result.data.lobby_join");
                Assertion.AssertNotNull(result.Result.Data.JoinedLobby.Id, "result.Result.data.lobby_join.id");
                JoinedLobbies.Add(result.Result.Data.JoinedLobby.Id);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_JOIN, Data = JoinedLobbies.LastOrDefault() });
            }
        }
        
        private void OnReconnectToLobby(GraphResult result)
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
                Assertion.AssertNotNull(result.Result.Data, "result.Result.data");
                Assertion.AssertNotNull(result.Result.Data.ReconnectedLobby, "result.Result.data.lobby_reconnect");
                Assertion.AssertNotNull(result.Result.Data.ReconnectedLobby.Id, "result.Result.data.lobby_reconnect.id");
                JoinedLobbies.Add(result.Result.Data.ReconnectedLobby.Id);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOBBY_RECONNECT, Data = JoinedLobbies.LastOrDefault() });
            }
        }
        #endregion

        #region Debug
        [Button(ButtonSizes.Medium)]
        public void TestLobbyActiveInstance()
        {
            this.Publish(new OnLobbyActiveInstanceSignal() { Token = Token });
        }

        [Button(ButtonSizes.Medium)]
        public void TestLobbyOpen()
        {
            this.Publish(new OnLobbyOpenSignal() { Token = Token, Slug = "session", Name = "Session" });
        }

        [Button(ButtonSizes.Medium)]
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

            //LobbyInfo lobby = Lobbies.LastOrDefault();
            //IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
            //request.AddParameter(SocketConnections.ID, lobby.id);
            //string socketId = QuerySystem.Complete<string>();
            //this.Publish(new OnLobbyJoinSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
        }

        [Button(ButtonSizes.Medium)]
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

            //LobbyInfo lobby = Lobbies.LastOrDefault();
            //IQueryRequest request = QuerySystem.Start(SocketConnections.SOCKET_ID_QUERY);
            //request.AddParameter(SocketConnections.ID, lobby.id);
            //string socketId = QuerySystem.Complete<string>();
            //this.Publish(new OnLobbyReconnectSignal() { Token = Token, LobbyId = lobby.id, SocketId = socketId });
        }

        [Button(ButtonSizes.Medium)]
        public void TestStartGame()
        {
            // emit "message": /start
            this.Publish(new OnStartGameSignal());
        }

        [Button(ButtonSizes.Medium)]
        public void TestLobbyStatus()
        {
            // emit "message": /start
            this.Publish(new OnCheckLobbyStatus());
        }

        [Button(ButtonSizes.Medium)]
        public void TestSetInitialTarget()
        {
            this.Publish(new OnSetInitialTargetSignal()
            {
                Timestamp = Timestamp.Lapsed(),
                //Target = new Float3(0.0f, 3.4f, 9.7f),    // target
            });
        }
        
        [Button(ButtonSizes.Medium)]
        public void TestSendScore()
        {
            // emit "message": + <timestamp> <data>
            // emit "message": + 14076.95 3.9,1.5,4.0,0.0,3.4,9.7,0.0,-9.8,0.0,-3.3,7.5,4.7,1.2
            //this.Publish(new OnSendScoreSignal()
            //{
            //    Timestamp = Timestamp.Lapsed(),
            //    Source = new Float3(3.9f, 1.5f, 4.0f),          // source
            //    Target = new Float3(0.0f, 3.4f, 9.7f),          // target
            //    Gravity = new Float3(0.0f, -9.8f, 0.0f),        // gravity
            //    Velocity = new Float3(-3.3f, 7.5f, 4.7f),       // velocity
            //    Duration = 1.2f,                                // duration
            //    Shot = Shots.Random(),                          // shot type
            //});
        }

        [Button(ButtonSizes.Medium)]
        public void TestSendMiss()
        {
            // emit "message": - <timestamp>
            // emit "message": - 14076.95
            this.Publish(new OnSendMissSignal()
            {
                Timestamp = Timestamp.Lapsed()
            });
        }

        [Button(ButtonSizes.Medium)]
        public void TestEndGame()
        {
            // emit "message": /end
            this.Publish(new OnEndGameSignal());
        }

        [Button(ButtonSizes.Medium)]
        public void TestCloseSession()
        {
            // emit "message": /leave
            this.Publish(new OnCloseSessionSignal());
        }

        [Button(ButtonSizes.Medium)]
        public void TestCloseLobbySession()
        {
            LobbyInfo lobby = Lobbies.LastOrDefault();
            this.Publish(new OnCloseLobbySessionSignal() { Token = Token, LobbyId = lobby.Id });
        }

        // http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:5000/_lobbies
        // http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:5000/_status
        // https://chrome.google.com/webstore/detail/json-viewer/gbmdgpbipfallnflgajpaliibnhdgobh
        #endregion
    }
}
