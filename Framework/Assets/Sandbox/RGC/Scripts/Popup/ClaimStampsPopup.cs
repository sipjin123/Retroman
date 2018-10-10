using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;
using Framework.ExtensionMethods;

using Sandbox.Downloader;
using Sandbox.GraphQL;
using Sandbox.Popup;

namespace Sandbox.RGC
{
    using TMPro;

    public class ClaimStampsPopup : PopupWindow, IPopupWindow, UniRx.IProgress<float>
    {
        [SerializeField]
        private Image QRCode;

        [SerializeField]
        private Image Spinner;

        [SerializeField]
        private TextMeshProUGUI Progress;

        [SerializeField]
        private string Url;
        
        protected override void Start()
        {
            //Test("http://13.251.171.185:3002/dev/fgc/qr?uid=687df0c0-c162-11e8-a8da-bfc3d5ae9129");

            base.Start();
            
            Assertion.AssertNotNull(QRCode, D.ERROR + "ConvertOfflinePopup::Awake QRCode text should never be null!\n");

            Url = string.Format("{0}{1}", PopupData.GetData<string>(), QuerySystem.Query<PlayerIDContainer>(PlayerIdRequest.PLAYER_SERVER_DATA).Id);

            DownloadQRCode(Url);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.Publish(new OnFetchCurrenciesSignal());
        }

        private void DownloadQRCode(string url)
        {
            ObservableWWW.GetAndGetBytes(url, null, this)
                .Timeout(TimeSpan.FromSeconds(30))
                .Take(1)
                .Subscribe(_ =>
                {
                    QRCode.sprite = _.ToSprite();
                    Spinner.gameObject.SetActive(false);
                    Progress.gameObject.SetActive(false);
                },
                _ =>
                {
                    QRCode.gameObject.SetActive(false);
                    Spinner.gameObject.SetActive(false);
                    Progress.text = "0".RichTextColorize(Color.red);
                })
                .AddTo(this);
        }

        public void Report(float value)
        {
            Debug.LogFormat("Progress:{0}\n", value);
            Progress.text = string.Format("{0:P0}", value / 1f);
        }
    }
}