using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;

using MiniJSON;

using Common.Utils;

using Framework;

namespace Sandbox.Webview
{
    public class WebviewSandbox : SceneObject
    {
        [SerializeField, ShowInInspector]
        private InputField Input;

        [SerializeField, ShowInInspector]
        private GUISkin Display;

        [SerializeField, ShowInInspector]
        private WebviewScaler WebScaler;

        public void ActivateWebView()
        {
            WebScaler.Position();
        }

        public void DeactivateWebView()
        {
            WebScaler.Hide();
        }

        public void ShowArguments()
        {
        }
    }
}