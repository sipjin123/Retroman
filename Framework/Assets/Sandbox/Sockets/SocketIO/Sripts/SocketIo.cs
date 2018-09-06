using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Quobject.SocketIoClientDotNet.Client;

using UniRx;
using UniRx.Triggers;

using MiniJSON;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.SocketIo
{
    using MiniJSON;

    using UniRx;
    using UniRx.InternalUtil;
    using UniRx.Operators;
    using UniRx.Toolkit;
    using UniRx.Triggers;
    
    public enum SocketSignal
    {
        ON_CONNECTED,
        ON_DISCONNECTED,
        ON_SUBSCRIBED,
        ON_RECEIVED_MESSAGE,
        ON_CUSTOM_EVENT,
    }

    public class OnSocketSignal
    {
        public SocketSignal Event;
        public string SubEvent;
        private object _Info;
        public object Info
        {
            get { return _Info; }
        }

        public void SetInfo(object info)
        {
            _Info = info;
        }

        public T GetInfo<T>()
        {
            return (T)_Info;
        }
    }

    public struct Channel
    {
        public string EndPoint;
        public string Subscribe;
    }

    public struct OnAddChannel
    {
        public string ChannelKey;
        public Channel Channel;
    }

    public interface ISocketIo
    {
        void Initialize(Channel channel);
        void Connect();
        void Disconnect();
    }
    
    public class SocketIo : MonoBehaviour, ISocketIo
    {
        private readonly string HEADER_TYPE_KEY = "Content-Type";
        private readonly string HEADER_TYPE_VAL = "application/json";

        [HideInInspector]
        public MessageBroker Broker { get; private set; }

        [HideInInspector]
        public string SocketId { get { return Connection.SocketId; } }

        [SerializeField, ShowInInspector]
        private SocketSignal SocketEvent;

        [SerializeField, ShowInInspector]
        private string SocketSubEvent;

        [SerializeField, ShowInInspector]
        private SocketConnectionData Connection;
        
        private SocketInfo Info;
        
        private Socket Socket = null;

        public SocketIo()
        {
            Connection = new SocketConnectionData();
            Broker = new MessageBroker();
        }
        
        public void Initialize(Channel channel)
        {
            InitializeSocket(channel.EndPoint, channel.Subscribe);
        }

        public void Initialize(string baseUrlAndPort)
        {
            InitializeSocket(baseUrlAndPort, string.Empty);
        }

        [Button(25)]
        public void Connect()
        {
            Socket.Connect();
        }

        [Button(25)]
        public void Disconnect()
        {
            Socket.Disconnect();
        }

        [Button(25)]
        public void Subscribe()
        {
            Subscribe(Connection.Channel);
        }

        public void Emit(string message, params object[] args)
        {
            Socket.Send(message);
            //Socket.Emit(message, args);
        }

        private void InitializeSocket(string endPoint, string subscribe)
        {
            Debug.LogFormat(D.SOCKETS + "SocketIo::InitializeSocket EndPoint:{0} Subscribe:{1}\n", endPoint, subscribe);

            Connection.EndPoint = endPoint;
            Connection.Subscribe = subscribe;

            Socket = IO.Socket(Connection.EndPoint);
            
            Broker.Receive<OnSocketSignal>()
                .Subscribe(_ =>
                {
                    SocketEvent = _.Event;
                    SocketSubEvent = _.SubEvent;
                });

            #region Main socket events
            Socket.On(Socket.EVENT_CONNECT, () =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_CONNECT SocketId:{0}", Socket.Io().EngineSocket.Id);
                //Debug.Log(str);

                Connection.SocketId = Socket.Io().EngineSocket.Id;

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CONNECTED;
                signal.SubEvent = Socket.EVENT_CONNECT;
                signal.SetInfo(Connection.SocketId);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_DISCONNECT, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_DISCONNECT Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_DISCONNECTED;
                signal.SubEvent = Socket.EVENT_DISCONNECT;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });
            #endregion

            #region Other socket events
            /*
            Socket.On(Connection.Event, (data) =>
            {
                string str = string.Format("Data received! Message: Data:{0}", data);
                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_RECEIVED_MESSAGE;
                signal.SubEvent = Connection.Event;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });
            //*/
            
            Socket.On(Socket.EVENT_RECONNECT_ATTEMPT, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_ATTEMPT Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_RECONNECT_ATTEMPT;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_RECONNECT_FAILED, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_FAILED Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_RECONNECT_FAILED;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_RECONNECT_ERROR, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_ERROR Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_RECONNECT_ERROR;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_RECONNECT, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_RECONNECT Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_RECONNECT;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_RECONNECTING, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_RECONNECTING Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_RECONNECTING;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_CONNECT_ERROR Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_CONNECT_ERROR;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_MESSAGE Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_RECEIVED_MESSAGE;
                signal.SubEvent = Socket.EVENT_MESSAGE;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });

            Socket.On(Socket.EVENT_ERROR, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_ERROR Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_ERROR;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });
            
            Socket.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                //string str = string.Format("SocketIOScript::On::EVENT_CONNECT_TIMEOUT Data:{0}", data);
                //Debug.Log(str);

                OnSocketSignal signal = new OnSocketSignal();
                signal.Event = SocketSignal.ON_CUSTOM_EVENT;
                signal.SubEvent = Socket.EVENT_CONNECT_TIMEOUT;
                signal.SetInfo(data);
                Broker.Publish(signal);
            });
            #endregion
        }

        public void Subscribe(string channel)
        {
            if (string.IsNullOrEmpty(channel))
            {
                Debug.LogErrorFormat(D.WARNING + "SocketIo::Subscribe SocketId:{0} can't subscribe to null channel.\n", Connection.SocketId);
                return;
            }

            if (string.IsNullOrEmpty(Connection.Subscribe))
            {
                Debug.LogErrorFormat(D.WARNING + "SocketIo::Subscribe SocketId:{0} can't subscribe to null end point.\n", Connection.SocketId);
                return;
            }

            Info = new SocketInfo();
            Info.socket_id = Connection.SocketId;
            Info.channel = channel;

            string body = JsonUtility.ToJson(Info);
            byte[] bytes = Encoding.UTF8.GetBytes(body);

            Debug.LogFormat(D.SOCKETS + "SocketIo::Subscribe Subscribing to:{0} with Post Headers. Body:{1}\n", Connection.Subscribe, body);

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add(HEADER_TYPE_KEY, HEADER_TYPE_VAL);

            ObservableWWW.Post(Connection.Subscribe, bytes, header)
                .Take(1)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.SOCKETS + "SocketIo::Subscribe GraphInfo::Request Post (Post Headers) Result:{0}\n", _);
                    Info = JsonUtility.FromJson<SocketInfo>(_);
                },
                _ =>
                {
                    Debug.LogFormat(D.SOCKETS + "SocketIo::Subscribe GraphInfo::Request Post Error (Post Headers) Result:{0}\n", _.Message);
                },
                () =>
                {
                    Debug.LogFormat(D.SOCKETS + "SocketIo::Subscribe GraphInfo::Request Post Completed (Post Headers) SocketId:{0} Channel:{1}\n", Info.socket_id, Info.channel);

                    OnSocketSignal signal = new OnSocketSignal();
                    signal.Event = SocketSignal.ON_SUBSCRIBED;
                    signal.SubEvent = string.Empty;
                    signal.SetInfo((object)Info);
                    Broker.Publish(signal);
                });
        }

        public override string ToString()
        {
            return SocketId;
        }
    }
}