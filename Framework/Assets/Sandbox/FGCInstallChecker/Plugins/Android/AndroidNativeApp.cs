using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.RGC
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public class NativeApp : INativeApp
    {
        public bool IsInstalled(string package)
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = null;

            //if the app is installed, no errors. Else, doesn't get past next line
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", package);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(D.ERROR + "NativeApp::IsInstalled Package:{0} Exception:{1}\n", package, ex.Message);
            }

            bool installed = launchIntent != null;

            Debug.LogFormat(D.LOG + "NativeApp::IsInstalled Package:{0} IsInstalled:{1}\n", package, installed);

            return installed;
        }
    }
#endif
}