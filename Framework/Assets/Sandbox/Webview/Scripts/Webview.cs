using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;

using MiniJSON;

using Common.Utils;

using Framework;

namespace Sandbox.Webview
{
    public class Webview : MonoBehaviour
    {
        public string Url;

        private WebViewObject WebViewObject;

        private WebMargin Margin;
        
        public void Load(string url, WebMargin margin)
        {
            Url = url;
            Margin = margin;
            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            WebViewObject = gameObject.AddComponent<WebViewObject>();
            WebViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                },
                err: (msg) =>
                {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                },
                ld: (msg) =>
                {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if !UNITY_ANDROID
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                WebViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                WebViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#endif
                    WebViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                },
                //ua: "custom user agent string",
                enableWKWebView: true);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        WebViewObject.bitmapRefreshCycle = 1;
#endif

            Show(Margin);

#if !UNITY_WEBPLAYER
            if (Url.StartsWith("http"))
            {
                WebViewObject.LoadURL(Url.Replace(" ", "%20"));
            }
            else
            {
                var exts = new string[]{
                    ".jpg",
                    ".js",
                    ".html"  // should be last
                };

                foreach (var ext in exts)
                {
                    var url = Url.Replace(".html", ext);
                    var src = Path.Combine(Application.streamingAssetsPath, url);
                    var dst = Path.Combine(Application.persistentDataPath, url);
                    byte[] result = null;

                    if (src.Contains("://"))
                    {  
                        // for Android
                        var www = new WWW(src);
                        yield return www;
                        result = www.bytes;
                    }
                    else
                    {
                        result = File.ReadAllBytes(src);
                    }

                    File.WriteAllBytes(dst, result);

                    if (ext == ".html")
                    {
                        WebViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                        break;
                    }
                }
            }
#else
        if (Url.StartsWith("http")) 
        {
            WebViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            WebViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
        WebViewObject.EvaluateJS(
            "parent.$(function() {" +
            "   window.Unity = {" +
            "       call:function(msg) {" +
            "           parent.unityWebView.sendMessage('WebViewObject', msg)" +
            "       }" +
            "   };" +
            "});");
#endif
        }

        public void Show(WebMargin margin)
        {
            WebViewObject.SetMargins(margin.Left, margin.Top, margin.Right, margin.Bottom);
            WebViewObject.SetVisibility(true);
        }

        public void Hide()
        {
            WebViewObject.SetVisibility(false);
        }
    }
}