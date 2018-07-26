using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using MiniJSON;

using Sirenix.OdinInspector;

using Common;
using Common.Query;

using Framework;

using Sandbox.GraphQL;
using Sandbox.Services;

// Socket messages 
namespace Sandbox.SocketIo
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public enum LobbyStatus
    {
        OPEN,
        MATCHMAKING,
        IN_GAME,
        POST_GAME,
        CLOSED,
        TERMINATED,
    }

    public enum ShotType
    {
        Normal = 0,
        Perfect = 1,
    }

    public enum MessageType
    {
        status,
        miss,
        score,
        initial_target,
        lobby_status_changed,
        player_joined,
        player_reconnected,
        player_left,
        system_message,
    }

    public struct OnPlayerJoinedSignal { }

    public struct OnPlayerLeftSignal { }

    public struct OnUpdateServerScoreSignal
    {
        public ObscuredInt Score;
    }

    public struct OnUpdateServerLivesSignal
    {
        public ObscuredInt Lives;
    }

    public struct OnConnectedToSocketsSignal
    {
        public ObscuredString LobbyId;
        public ObscuredString SocketId;
    }

    public struct OnDisconnectedToSocketsSignal
    {

    }

    public struct OnLobbyStatusChangedSignal
    {
        public LobbyStatus Status;
    }

    public struct OnLobbyStatusSignal
    {
        public LobbyStatus Status;
        public ObscuredInt Score;
        public ObscuredInt Lives;
        public ObscuredInt Elapsed;
    }

    public struct OnReceivedEventTriggerSignal
    {
        public ObscuredString Data;
    }
}

namespace Sandbox.SocketIo
{
    [Serializable]
    public class Status
    {
        public static readonly string TYPE = "status";

        [Serializable]
        public class Data
        {
            public List<float> initial_target;
            public int last_command;
            public int score;
            public int lives_left;
            public int perfect_streak;
            public int elapsed;
            public string hot_start;
            public string hot_end;
            public string hot_multiplier;
            public string status;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class Miss
    {
        public static readonly string TYPE = "miss";

        [Serializable]
        public class Data
        {
            public int timestamp;
            public int lives_left;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class Score
    {
        public static readonly string TYPE = "score";

        [Serializable]
        public class Data
        {
            public int timestamp;
            public int score;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class InitialTarget
    {
        public static readonly string TYPE = "initial_target";

        [Serializable]
        public class Data
        {
            public int timestamp;
            public List<float> initial_target;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class LobbyStatusChanged
    {
        public static readonly string TYPE = "lobby_status_changed";

        [Serializable]
        public class Data
        {
            public string status;   // LobbyStatus
            public string message;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class PlayerJoined
    {
        public static readonly string TYPE = "player_joined";

        [Serializable]
        public class Data
        {
            public string player_id;
            public string message;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class SystemMessage
    {
        public static readonly string TYPE = "system_message";

        [Serializable]
        public class Data
        {
            public string message;
        }

        public string type;
        public Data data;
    }

    [Serializable]
    public class RegisterResulData
    {
        public RegisterWebhook webhooks;
    }

    [Serializable]
    public class RegisterWebhook
    {
        public RegisterData register;
        public PlayersList players;
    }

    [Serializable]
    public class PlayersList 
    {
        public PlayersBody body;
    }

    [Serializable]
    public class PlayersBody
    {
        public List<PlayersBodyData> data;
    }

    [Serializable]
    public class PlayersBodyData
    {
        public string id;
        public string announcement_id;
        public string code;
    }

    [Serializable]
    public class RegisterData
    {
        public RegisterBody body;
    }

    [Serializable]
    public class RegisterBody
    {
        public bool success;
        public string message;
    }

    [Serializable]
    public class SocketMessage
    {
        public string type = string.Empty;
        public RegisterResulData result;
        public RegisterPayloadResult payload;
    }

    [Serializable]
    public class PayloadBody
    {
        public string announcement_id;
    }

    [Serializable]
    public class RegisterPayloadResult
    {
        public string message;
        public string body;
    }
}

namespace Sandbox.SocketIo
{
    using CodeStage.AntiCheat.ObscuredTypes;

    using Sandbox.Security;

    [Serializable]
    public class SocketEntry : IJson
    {
        public string Id;
        public string Connection;
        public SocketIo Socket;
    }
    
    public class SocketConnections : BaseService
    {
        // Socket Commands
        public static readonly string START_GAME_COMMAND = "/start";               // emit "message": /start
        public static readonly string INITIAL_TARGET_COMMAND = "target {0} {1}";   // emin "message": target 14076 x,y,z
        public static readonly string SEND_SCORE_COMMAND = "+ {0} {1}";            // emit "message": + 14076 3.9,1.5,4.0,0.0,3.4,9.7,0.0,-9.8,0.0,-3.3,7.5,4.7,1.2
        public static readonly string SEND_MISS_COMMAND = "- {0}";                 // emit "message": - 14076.95
        public static readonly string END_GAME_COMMAND = "/end";                   // emit "message": /end
        public static readonly string LEAVE_GAME_COMMAND = "/leave";               // emit "message": /leave
        public static readonly string STATUS_COMMAND = "/status";                  // emit "message": /status

        // Query Ids
        public static readonly string ID = "Id";

        // Query Params
        public static readonly string SOCKET_ID_QUERY = "SocketIdQuery";

        // End Points
        public static readonly string PUBSUB = "pubsub_endpoint";
        public static readonly string PUBSUB_PUBLIC = "pubsub_public_channel";
        public static readonly string PUBSUB_PRIVATE = "pubsub_private_channel";

        [SerializeField, ShowInInspector]
        private List<SocketEntry> Connections;

        private Dictionary<MessageType, Action<string>> SocketHandlers;

        private ObscuredBool CheatDetected = false;

        #region Services
        public override void InitializeService()
        {
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.CONFIGURE)
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // +AS:06192018 [TEST AUTOCONNECT] Connect to pubsub socket
                    List<Config> configs = _.GetData<List<Config>>();
                    
                    Connect(configs.Find(c => c.key.Equals(PUBSUB)));
                    //Connect(configs.Find(c => c.key.Equals(PUBSUB_PUBLIC)));
                    //Connect(configs.Find(c => c.key.Equals(PUBSUB_PRIVATE)));
                })
                .AddTo(this);
            
            this.Receive<OnConnectToLobbySignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    LobbyInfo lobby = _.Lobby;
                    Connect(lobby.id, string.Format("{0}:{1}", lobby.base_url, lobby.port));
                })
                .AddTo(this);
            
            this.Receive<OnStartGameSignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": /start
                    EmitMessage(START_GAME_COMMAND);
                })
                .AddTo(this);

            this.Receive<OnSetInitialTarget>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emin message 14076 x,y,z
                    string message = string.Format(INITIAL_TARGET_COMMAND, _.Timestamp.GetDecrypted(), _.Target.GetDecrypted().ToString());
                    EmitMessage(message);
                })
                .AddTo(this);

            this.Receive<OnSendScoreSignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": + 14076 3.9,1.5,4.0,0.0,3.4,9.7,0.0,-9.8,0.0,-3.3,7.5,4.7,1.2
                    string message = string.Format(SEND_SCORE_COMMAND, _.Timestamp.GetDecrypted(), _.ToString());
                    EmitMessage(message);
                })
                .AddTo(this);

            this.Receive<OnSendMissSignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": - 14076.95
                    string message = string.Format(SEND_MISS_COMMAND, _.Timestamp.GetDecrypted());
                    EmitMessage(message);
                })
                .AddTo(this);
            
            this.Receive<OnEndGameSignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": /end
                    EmitMessage(END_GAME_COMMAND);
                })
                .AddTo(this);

            this.Receive<OnCloseSessionSignal>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": /leave
                    EmitMessage(LEAVE_GAME_COMMAND);
                })
                .AddTo(this);

            this.Receive<OnCheckLobbyStatus>()
                .Where(_ => !CheatDetected.GetDecrypted())
                .Subscribe(_ =>
                {
                    // emit "message": /status
                    EmitMessage(STATUS_COMMAND);
                })
                .AddTo(this);

            this.Receive<OnCheaterDetectedSignal>()
                .Subscribe(_ =>
                {
                    Debug.LogErrorFormat(D.WARNING + "Cheater Detected!");

                    EmitMessage(END_GAME_COMMAND);
                    EmitMessage(LEAVE_GAME_COMMAND);

                    CheatDetected = true;
                })
                .AddTo(this);

            QuerySystem.RegisterResolver(SOCKET_ID_QUERY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(ID));
                string connectionId = request.GetParameter<string>(ID);

                SocketIo socket = Connections.Find(c => c.Id.Equals(connectionId)).Socket;
                Assertion.AssertNotNull(socket);
                
                Assertion.AssertNotNull(socket);
                result.Set(socket.SocketId);
            });

            SocketHandlers = new Dictionary<MessageType, Action<string>>();
            SocketHandlers.Add(MessageType.player_joined, HandlePlayerJoined);
            SocketHandlers.Add(MessageType.player_reconnected, HandlePlayerJoined);
            SocketHandlers.Add(MessageType.player_left, HandlePlayerLeft);
            SocketHandlers.Add(MessageType.score, HandleScore);
            SocketHandlers.Add(MessageType.miss, HandleMiss);
            SocketHandlers.Add(MessageType.status, HandleStatus);
            SocketHandlers.Add(MessageType.lobby_status_changed, HandleLobbyStatusChanged);

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
            QuerySystem.RemoveResolver(SOCKET_ID_QUERY);
        }
        #endregion

        private void Connect(Config config)
        {
            Debug.LogFormat(D.SOCKETS + "SocketConnections::Connect Channel:{0} EndPoint:{1}\n", config.key, config.value);

            Channel channel = new Channel();
            channel.EndPoint = config.value;
            channel.Subscribe = config.value + "/subscribe";

            SocketIo socket = new SocketIo();
            socket.Broker
                .Receive<OnSocketSignal>()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    string result = _.GetInfo<object>().ToString();
                    OnHandleSocketEvents(_);
                })
                .AddTo(this);

            Connections = Connections ?? new List<SocketEntry>();
            Connections.Add(new SocketEntry()
            {
                Id = config.key,
                Connection = channel.EndPoint,
                Socket = socket,
            });

            socket.Initialize(channel);
        }

        private void Connect(string id, string connection)
        {
            SocketIo socket = new SocketIo();
            socket.Broker
                .Receive<OnSocketSignal>()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    string result = _.GetInfo<object>().ToString();
                    OnHandleSocketEvents(_);
                })
                .AddTo(this);

            Connections = Connections ?? new List<SocketEntry>();
            Connections.Add(new SocketEntry()
            {
                Id = id,
                Connection = connection,
                Socket = socket,
            });

            socket.Initialize(connection);
        }

        private void OnHandleSocketEvents(OnSocketSignal signal)
        {
            Debug.LogFormat(D.SOCKETS + "SocketConnection::OnHandleSocketEvents Evt:{0} SubEvt:{1} Result:{2}\n", signal.Event.ToString(), signal.SubEvent, signal.Info);

            switch (signal.Event)
            {
                case SocketSignal.ON_CONNECTED:
                    SocketEntry entry = Connections.Find(c => c.Socket.SocketId.Equals(signal.GetInfo<string>()));
                    this.Publish(new OnConnectedToSocketsSignal() { LobbyId = entry.Id, SocketId = entry.Socket.SocketId });

                    GraphQLResultDataHandler data = QuerySystem.Query<GraphQLResultDataHandler>(GraphQLResultDataHandler.RESULT_DATA);
                    GraphConfigs config = (GraphConfigs)data.GetGraphQLData(GraphQLRequestType.CONFIGURE);
                    
                    // auto subscribe
                    entry.Socket.Subscribe();
                    entry.Socket.Subscribe(config.PubChannel);
                    entry.Socket.Subscribe(config.PrivateChannel);
                    break;
                case SocketSignal.ON_RECEIVED_MESSAGE:
                    string rawMessage = signal.Info.ToString();
                    SocketMessage message = JsonUtility.FromJson<SocketMessage>(rawMessage);

                    // event trigger message assumption
                    if (!string.IsNullOrEmpty(message.type))
                    {
                        MessageType type = message.type.ToEnum<MessageType>();
                        if (SocketHandlers.ContainsKey(type))
                        {
                            SocketHandlers[type](rawMessage);
                        }
                        else
                        {
                            Debug.LogErrorFormat(D.WARNING + "SocketConnections::OnHandleSocketEvents SockedHandlers has no resolver for MessageType:{0}\n", type);
                        }
                    }
                    else
                    {
                        this.Publish(new OnReceivedEventTriggerSignal() { Data = rawMessage });
                    }

                    break;
                case SocketSignal.ON_SUBSCRIBED:
                    break;
                case SocketSignal.ON_DISCONNECTED:
                    //<color=white>[SOCKETS]</color> SocketConnection::OnHandleSocketEvents Evt:ON_DISCONNECTED SubEvt:disconnect Result:io server disconnect
                    this.Publish(new OnDisconnectedToSocketsSignal());
                    break;
            }
        }

        private void EmitMessage(string message)
        {
            Debug.LogFormat(D.SOCKETS + "SocketConnection::EmitMessage Message:{0}\n", message);

            // Assuming the gameplay connection is always the last entry in the list
            SocketEntry connection = Connections.LastOrDefault();
            connection.Socket.Emit(message);
        }

        /*
        {
            "type": "system_message",
            "data": {
                "message": "/start"
            }
        }
        {
            "type": "system_message",
            "data": {
                "message": "Command is available for lobby host only"
            }
        }
        //*/
        private void HandleSystem(string message)
        {
            SystemMessage system = JsonUtility.FromJson<SystemMessage>(message);
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandleSystem Message:{0}\n", system.data.message);
        }

        /*
        {
            "type": "initial_target",
            "data": {
                "timestamp": 8201,
                "initial_target": [
                    0,
                    3.4,
                    9.7
                ]
            }
        }
        //*/
        private void HandleInitialTarget(string message)
        {
            InitialTarget target = JsonUtility.FromJson<InitialTarget>(message);
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandleInitialTarget Timestamp:{0} Target:{1}\n", target.data.timestamp, target.data.initial_target);
        }

        /*
        {
            "type": "player_joined",
            "data": {
                "player_id": "430ef9c0-22b5-11e8-9cff-d94e89226117",
                "message": "430ef9c0-22b5-11e8-9cff-d94e89226117 joined"
            }
        }
        //*/
        private void HandlePlayerJoined(string message)
        {
            PlayerJoined playerJoined = JsonUtility.FromJson<PlayerJoined>(message);
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandlePlayerJoined PlayerId:{0}\n", playerJoined.data.player_id);
            this.Publish(new OnPlayerJoinedSignal());
        }

        private void HandlePlayerLeft(string message)
        {
            this.Publish(new OnPlayerLeftSignal());
        }

        /*
        {
            "type": "lobby_status_changed",
            "data": {
                "status": "IN_GAME",
                "message": "Game started"
            }
        }
        {
            "type": "lobby_status_changed",
            "data": {
                "status": "TERMINATED",
                "message": "Lobby terminated"
            }
        }
        {
            "type": "lobby_status_changed",
            "data": {
                "status": "POST_GAME",
                "message": "Game ended"
            }
        }
        //*/
        private void HandleLobbyStatusChanged(string message)
        {
            LobbyStatusChanged statusChanged = JsonUtility.FromJson<LobbyStatusChanged>(message);
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandleLobbyStatusChanged Status:{0} Message:{1}\n", statusChanged.data.status, statusChanged.data.message);

            LobbyStatus lobbyStatus = statusChanged.data.status.ToEnum<LobbyStatus>();

            this.Publish(new OnLobbyStatusChangedSignal() { Status = lobbyStatus });
        }

        /*
        {
            "type": "score",
            "data": {
                "timestamp": 60962,
                "score": 1
            }
        }
        //*/
        private void HandleScore(string message)
        {
            Score score = JsonUtility.FromJson<Score>(message);
            this.Publish(new OnUpdateServerScoreSignal() { Score = score.data.score });
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandleScore Timestamp:{0} Score:{1}\n", score.data.timestamp, score.data.score);
        }

        /*
        {
            "type": "miss",
            "data": {
                "timestamp": 88149,
                "lives_left": 2
            }
        }
        //*/
        private void HandleMiss(string message)
        {
            Miss miss = JsonUtility.FromJson<Miss>(message);
            this.Publish(new OnUpdateServerLivesSignal() { Lives = miss.data.lives_left });
            Debug.LogFormat(D.SOCKETS + "SockedConnections::HandleMiss Timestamp:{0} Lives:{1}\n", miss.data.timestamp, miss.data.lives_left);
        }

        /*
        {
            "type": "status",
            "data": {
                "initial_target": [
                    0,
                    3.4,
                    9.7
                ],
            "last_command": 136589,
            "score": 4,
            "lives_left": 1,
            "perfect_streak": 0,
            "hot_start": "2018-06-25T04:00:10Z",
            "hot_end": "2018-06-25T07:00:10Z",
            "hot_multiplier": "2",
            "status": "IN_GAME",
            "elapsed": 151651
            }
        }
        //*/
        private void HandleStatus(string message)
        {
            Status status = JsonUtility.FromJson<Status>(message);
            LobbyStatus lobbyStatus = status.data.status.ToEnum<LobbyStatus>();

            /*
            public List<float> initial_target;
            public int last_command;
            public int score;
            public int lives_left;
            public int perfect_streak;
            public int elapsed;
            public string hot_start;
            public string hot_end;
            public string hot_multiplier;
            public string status;
            //*/

            this.Publish(new OnLobbyStatusSignal()
            {
                Status = lobbyStatus,
                Score = status.data.score,
                Lives = status.data.lives_left,
                Elapsed = status.data.elapsed,
            });
        }
    }
}
 