using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.SocketIo
{
    [Serializable]
    public class SocketConnectionData
    {
        public string EndPoint = "http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:4321";
        public string Subscribe = "http://ec2-54-169-135-194.ap-southeast-1.compute.amazonaws.com:4321/subscribe";
        public string SocketId = string.Empty;
        public string Channel = "gmc:system-prdgm";
        public string Event = "message";
    }
}