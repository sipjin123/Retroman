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

namespace Sandbox.SocketIo
{
    [Serializable]
    public class SocketInfo
    {
        public string socket_id;
        public string channel;

        public byte[] ToPostData()
        {
            string body = JsonUtility.ToJson(this);
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            return bytes;
        }
    }
}