using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Quobject.SocketIoClientDotNet.Client;

using MiniJSON;

using Sirenix.OdinInspector;

namespace Sandbox.Sockets
{
    using MiniJSON;

    using UniRx;
    using UniRx.InternalUtil;
    using UniRx.Operators;
    using UniRx.Toolkit;
    using UniRx.Triggers;

    public class ChatData
    {
        public string id;
        public string msg;
    };

    public class SocketInfo
    {
        public string socket_id;
        public string channel;
    }

    public class SocketIOTest : MonoBehaviour
    {
        public string EndPoint = "http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:4321";
        public string Subscribe = "http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:4321/subscribe";
        public string Message = "message";
        public string SocketId = string.Empty;
        public string Channel = "gmc:system-prdgm";
        public SocketInfo Info;

        public InputField uiInput = null;
        public Button uiSend = null;
        public Button uiOpen = null;
        public Button uiClose = null;
        public Text uiChatLog = null;

        protected Socket Socket = null;
        protected List<string> chatLog = new List<string>();
        
        private void Start()
        {
            //Initialize();
        }

        [Button(25)]
        public void Initialize()
        {
            InitializeSocketIO();
            
            Socket.On(Message, (data) =>
            {
                string str = string.Format("Data received! Message: Data:{0}", data);

                UpdateLog(str);
            });

            uiSend.onClick.AddListener(() =>
            {
                SendChat(uiInput.text);
                uiInput.text = "";
                uiInput.ActivateInputField();
            });

            uiOpen.onClick.AddListener(Open);
            uiClose.onClick.AddListener(Close);

            UpdateLog("Socket Initialized");
        }
        
        private void InitializeSocketIO()
        {
            //*
            Socket = IO.Socket(EndPoint);

            Socket.On(Socket.EVENT_CONNECT, () =>
            {
                SocketId = Socket.Io().EngineSocket.Id;
                UpdateLog("Socket.IO connected. SocketId:" + SocketId);
            });
            
            Socket.On(Socket.EVENT_RECONNECT_ATTEMPT, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_ATTEMPT Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_RECONNECT_FAILED, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_FAILED Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_RECONNECT_ERROR, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_RECONNECT_ERROR Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_RECONNECT, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_RECONNECT Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_RECONNECTING, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_RECONNECTING Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_CONNECT_ERROR Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_MESSAGE Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_ERROR, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_ERROR Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_DISCONNECT, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_DISCONNECT Data:{0}", data);
                UpdateLog(str);
            });

            Socket.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                string str = string.Format("SocketIOScript::On::EVENT_CONNECT_TIMEOUT Data:{0}", data);
                UpdateLog(str);
            });
            
            Socket.On(Message, (data) =>
            {
                string str = string.Format("Data received! Message: Data:{0}", data);

                UpdateLog(str);
            });

            Socket.On(Subscribe, (data) =>
            {
                string str = string.Format("Data received! Subscribe: Data:{0}", data);

                UpdateLog(str);
            });
            //*/
        }

        [Button(25)]
        public void SubscribeToChannel()
        {
            Info = new SocketInfo();
            Info.socket_id = SocketId;
            Info.channel = Channel;

            string body = JsonUtility.ToJson(Info);
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            
            UpdateLog(string.Format("Subscribing to:{0} with Post Headers. Body:{1}", Subscribe, body));

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json");

            ObservableWWW.Post(Subscribe, bytes, header)
                .Take(1)
                .Subscribe(_ =>
                {
                    UpdateLog(string.Format("GraphInfo::Request Post (Post Headers) Result:{0}\n", _));
                    Info = JsonUtility.FromJson<SocketInfo>(_);
                },
                _ =>
                {
                    UpdateLog(string.Format("GraphInfo::Request Post Error (Post Headers) Result:{0}\n", _.Message));
                },
                () =>
                {
                    UpdateLog(string.Format("GraphInfo::Request Post Completed (Post Headers) SocketId:{0} Channel:{1}\n", Info.socket_id, Info.channel));
                });
        }

        private IEnumerator Process(WWW www, Action<WWW> handle)
        {
            yield return www;
            handle(www);
        }

        private void Update()
        {
            lock (chatLog)
            {
                if (chatLog.Count > 0)
                {
                    string str = uiChatLog.text;
                    foreach (var s in chatLog)
                    {
                        str = str + "\n" + s;
                    }
                    uiChatLog.text = str;
                    chatLog.Clear();
                }
            }
        }
        
        [Button(25)]
        public void Open()
        {
            Socket.Open();
            Socket.Connect();
        }

        [Button(25)]
        public void Close()
        {
            UpdateLog("Socket Close");
            Socket.Close();
        }

        private void SendChat(string str)
        {
            if (Socket != null)
            {
                UpdateLog(string.Format("Sending {0}...", str));

                Socket.Emit(Message, str);
                
                Socket.Emit(Subscribe, str);
            }
        }

        private void UpdateLog(string log)
        {
            Debug.LogErrorFormat("{0}\n", log);

            lock (chatLog)
            {
                chatLog.Add(log);
            }
        }
    }
}