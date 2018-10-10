using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common.Query;

namespace Sandbox.GraphQL
{
    using Framework;

    using Sandbox.Popup;

    using Button = UnityEngine.UI.Button;

    public class GraphQLAdsPopupImage : PopupWindow
    {
        [SerializeField]
        private Image DisableContainer;

        [SerializeField]
        private Image EnableContainer;

        [SerializeField]
        private Image Progress;

        [SerializeField]
        private Text ProgressText;

        [SerializeField]
        private Button SkipAdButton;

        [SerializeField]
        private Button ExitAdButton;

        [SerializeField]
        private Image AdImage;

        private GraphQLAdData Data;
        private FloatReactiveProperty CurrTime = new FloatReactiveProperty(0f);
        private bool WasSkipped;
        private bool IsApplicationPause = false;
        private bool CameFromPause = false;
        private float TimePaused;
        private float PickedDeltaTime;
        private IDisposable UpdateStream;
        
        protected override void Start()
        {
            base.Start();

            IsApplicationPause = false;
            CameFromPause = false;
            CurrTime.Value = 0;
            Progress.fillAmount = 0f;
            WasSkipped = true;
            Data = PopupData.GetData<GraphQLAdData>();
            PlayImageAd(Data.AdPath);
            ProgressText.enabled = true;
            DisableContainer.gameObject.SetActive(true);
            EnableContainer.gameObject.SetActive(false);
            SkipAdButton.gameObject.SetActive(false);
            ExitAdButton.gameObject.SetActive(false);

            CurrTime.ObserveEveryValueChanged(_ => _.Value)
                .SubscribeToText(ProgressText, _ => FormatProgress(_))
                .AddTo(this);

            UpdateStream = this.UpdateAsObservable()
                .Where(_ => !IsApplicationPause)
                .Subscribe(_ =>
                {
                    if (!CameFromPause)
                    {
                        CurrTime.Value += Time.fixedDeltaTime;
                    }
                    else
                    {
                        CameFromPause = false;
                    }

                    Progress.fillAmount = Math.Min(CurrTime.Value / Data.EndTime, 1f);
                    if (CurrTime.Value >= Data.Skiptime && Data.IsSkippable)
                    {
                        ProgressText.enabled = false;
                        DisableContainer.gameObject.SetActive(false);
                        EnableContainer.gameObject.SetActive(true);
                        SkipAdButton.gameObject.SetActive(true);
                        ExitAdButton.gameObject.SetActive(false);
                    }

                    if (CurrTime.Value >= Data.EndTime)
                    {
                        ProgressText.enabled = false;
                        DisableContainer.gameObject.SetActive(false);
                        EnableContainer.gameObject.SetActive(true);
                        SkipAdButton.gameObject.SetActive(false);
                        ExitAdButton.gameObject.SetActive(true);
                        WasSkipped = false;
                        UpdateStream.Dispose();
                    }
                })
                .AddTo(this);

            Screen.orientation = ScreenOrientation.Landscape;
        }


        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                CameFromPause = true;
                TimePaused = Time.time;
            }

            IsApplicationPause = pauseStatus;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (UpdateStream != null)
            {
                UpdateStream.Dispose();
            }

            this.Publish(new AdFinishedPlayingSignal()
            {
                ad_id = Data.AdId,
                was_skipped = WasSkipped,
                timemark = CurrTime.Value,
                ad_type = Data.AdType
            });

            Screen.orientation = ScreenOrientation.Portrait;
        }

        private string FormatProgress(float currtime)
        {
            if (Data.IsSkippable)
            {
                return Math.Ceiling((Data.Skiptime - currtime)).ToString();
            }
            else
            {
                return Math.Ceiling((Data.EndTime - currtime)).ToString();
            }
        }

        private void PlayImageAd(string path)
        {
            Debug.LogErrorFormat(D.L("[ADS]") + "GraphQLAdsPopupImage::PlayImageAd Path:{0}\n", path);
            AdImage.sprite = path.ToSprite();
        }
    }
}