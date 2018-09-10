using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using  Sirenix.OdinInspector;

using  UniRx;
using  UniRx.Triggers;

namespace Sandbox.RGC
{
    public interface INativeApp
    {
        bool IsInstalled(string package);
    }

    public class AppInstallChecker : MonoBehaviour, INativeApp
    {
        private INativeApp NativeApp;

        private void Awake()
        {
            NativeApp = new NativeApp();
        }

        public bool IsInstalled(string package)
        {
            return NativeApp.IsInstalled(package);
        }
    }
}