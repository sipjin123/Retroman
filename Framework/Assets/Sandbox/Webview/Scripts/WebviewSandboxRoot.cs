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
    public class WebviewSandboxRoot : Scene
    {
        [ShowInInspector]
        private InputField Input;

        [ShowInInspector]
        private GUISkin Display;

        [ShowInInspector]
        private WebviewScaler WebScaler;
        
        public void ActivateWebView()
        {
            WebScaler.Position(Input.text);
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