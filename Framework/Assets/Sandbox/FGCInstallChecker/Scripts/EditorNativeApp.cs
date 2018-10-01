using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;

using Framework;

namespace Sandbox.RGC
{
#if UNITY_EDITOR
    public class NativeApp : INativeApp
    {
        public bool IsInstalled(string package)
        {
            return false;
        }
    }
#endif
}