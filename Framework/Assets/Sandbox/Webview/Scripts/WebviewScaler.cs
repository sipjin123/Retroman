using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

namespace Sandbox.Webview
{
    public class WebMargin
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class WebviewScaler : MonoBehaviour
    {
        public const string HTTP = "http://";
        public const string HTTPS = "https://";
        public const string BLANK = "about:blank";
        public const string DEFAULT = "http://www.barangay143.com/";

        [SerializeField, ShowInInspector]
        private Canvas Canvas;

        [SerializeField, ShowInInspector]
        private RectTransform WebviewRect;

        [SerializeField, ShowInInspector]
        private Webview Webview;

        private string Url;
        
        public void Position()
        {
            Position(DEFAULT);
        }
        
        public void Position(string url)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                url = DEFAULT;
            }

            if (!url.StartsWith(HTTP))
            {
                url = string.Format("{0}{1}", HTTPS, url);
            }

            Url = url;

            Debug.LogErrorFormat("WebviewScaler::Position Shoting URL:{0}\n", Url);

            Rect wr = WebviewRect.rect;
            Rect cr = Canvas.GetComponent<RectTransform>().rect;

            float w = wr.width / cr.width;
            float h = wr.height / cr.height;
            Vector2 boxScale = new Vector2(w * Screen.width, h * Screen.height);
            Vector2 boxPosition = Canvas.worldCamera.WorldToScreenPoint(WebviewRect.transform.position);
            boxPosition.x = boxPosition.x - (boxScale.x * 0.5f);
            boxPosition.y = Screen.height - (boxPosition.y + boxScale.y);
            Position(boxPosition, boxScale);
        }

        public void Position(Vector2 viewPos, Vector2 viewScale)
        {
            WebMargin margin = new WebMargin()
            {
                Left = (int)viewPos.x,
                Top = (int)viewPos.y,
                Right = (int)(Screen.width - (viewPos.x + viewScale.x)),
                Bottom = (int)(Screen.height - (viewPos.y + viewScale.y))
            };
            
            Webview.Load(Url, margin);
        }

        public void Hide()
        {
            Webview.Hide();
        }
    }
}