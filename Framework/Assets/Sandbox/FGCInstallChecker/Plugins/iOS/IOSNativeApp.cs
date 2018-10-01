using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Sandbox.RGC
{
#if UNITY_IOS && !UNITY_EDITOR
    public class NativeApp : INativeApp
    {
        [DllImport("__Internal")]
        private static extern bool AppIsInstalled(string package);

        public bool IsInstalled(string package)
        {
            return AppIsInstalled(package);
        }
    }
#endif
}