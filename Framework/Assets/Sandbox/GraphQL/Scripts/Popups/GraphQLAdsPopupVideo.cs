using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Common.Query;

using Framework;

using Button = UnityEngine.UI.Button;

namespace Sandbox.GraphQL
{
    using Sandbox.Audio;
    using Sandbox.Popup;

    public class GraphQLAdsPopupVideo : PopupWindow
    {
        [SerializeField]
        private Image DisableContainerMute; //mute 

        [SerializeField]
        private Image EnableContainerMute; //mute

        [SerializeField]
        private Button MuteButton; //waves

        [SerializeField]
        private Button UnmuteButton;//x

        [SerializeField]
        private Image DisableContainerProgress;

        [SerializeField]
        private Image EnableContainerProgress;

        [SerializeField]
        private Image Progress;

        [SerializeField]
        private Text ProgressText;

        [SerializeField]
        private Button SkipAdButton;

        [SerializeField]
        private Button ExitAdButton;

        [SerializeField]
        private VideoPlayer VideoPlayer;

        [SerializeField]
        private Camera TargetCamera;

        [SerializeField]
        private string DebugAdPath;

        private BoolReactiveProperty IsMute = new BoolReactiveProperty(false); 

        private GraphQLAdData Data;
        private FloatReactiveProperty CurrTime = new FloatReactiveProperty(0f);
        private bool WasSkipped;
        private bool IsApplicationPause = false;
        private IDisposable UpdateStream;
        private float Duration;

        protected override void Awake()
        {
            base.Awake();

            IsMute.ObserveEveryValueChanged(_ => _.Value)
                .Subscribe(_ => 
                {
                    DisableContainerMute.gameObject.SetActive(_);
                    UnmuteButton.gameObject.SetActive(_);
                    EnableContainerMute.gameObject.SetActive(!_);
                    MuteButton.gameObject.SetActive(!_);
                })
                .AddTo(this);

            IsMute.Value = false;
        }

        protected override void Start()
        {
            base.Start();

            CurrTime.Value = 0;
            Progress.fillAmount = 0f;
            WasSkipped = true;
            
            ProgressText.enabled = true;
            DisableContainerMute.gameObject.SetActive(false);
            UnmuteButton.gameObject.SetActive(false);
            EnableContainerMute.gameObject.SetActive(true);
            MuteButton.gameObject.SetActive(true);

            DisableContainerProgress.gameObject.SetActive(true);
            EnableContainerProgress.gameObject.SetActive(false);
            SkipAdButton.gameObject.SetActive(false);
            ExitAdButton.gameObject.SetActive(false);

            VideoPlayer.started += VideoStarted;
            VideoPlayer.prepareCompleted += VideoPrepareCompleted;
            VideoPlayer.loopPointReached += VideoFinished;
            Data = PopupData.GetData<GraphQLAdData>();
            CurrTime.SubscribeToText(ProgressText, _ => FormatProgress(_)).AddTo(this);
            PlayVideoAd(Data.AdPath);

            Screen.orientation = ScreenOrientation.Landscape;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            IsApplicationPause = pauseStatus;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            IsApplicationPause = !hasFocus;
        }

        public void MuteCustomAd()
        {
            IsMute.Value = !IsMute.Value;
            VideoPlayer.GetTargetAudioSource(0).mute = IsMute.Value;
        }

        private void VideoPrepareCompleted(VideoPlayer source)
        {
            Duration = VideoPlayer.frameCount / VideoPlayer.frameRate;
            UpdateStream = this.UpdateAsObservable()
            .Where(_ => !IsApplicationPause)
            .Subscribe(_ =>
            {
                //
                CurrTime.Value = (float) VideoPlayer.time;
                Progress.fillAmount = Math.Min(CurrTime.Value / Duration, 1f);
                if (CurrTime.Value >= Data.Skiptime && Data.IsSkippable)
                {
                    ProgressText.enabled = false;
                    DisableContainerProgress.gameObject.SetActive(false);
                    EnableContainerProgress.gameObject.SetActive(true);
                    SkipAdButton.gameObject.SetActive(true);
                    ExitAdButton.gameObject.SetActive(false);
                }
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Screen.orientation = ScreenOrientation.Portrait;

            this.Publish(new AdFinishedPlayingSignal()
            {
                ad_id = Data.AdId,
                was_skipped = WasSkipped,
                timemark = CurrTime.Value,
                ad_type = Data.AdType
            });

            VideoPlayer.started -= VideoStarted;
            VideoPlayer.prepareCompleted -= VideoPrepareCompleted;
            VideoPlayer.loopPointReached -= VideoFinished;   
        }

        private void VideoFinished(VideoPlayer source)
        {
            Debug.Log("Finished Ad");
            ProgressText.enabled = false;
            Progress.fillAmount = 1f;
            DisableContainerProgress.gameObject.SetActive(false);
            EnableContainerProgress.gameObject.SetActive(true);
            SkipAdButton.gameObject.SetActive(false);
            ExitAdButton.gameObject.SetActive(true);
            WasSkipped = false;
            UpdateStream.Dispose();
        }


        private string FormatProgress(float currtime)
        {
            if (Data.IsSkippable)
            {
                return Math.Ceiling((Data.Skiptime - currtime)).ToString();
            }
            else
            {
                return Math.Ceiling((Duration - currtime)).ToString();
            }

        }

        private void VideoStarted(VideoPlayer source)
        {
            Debug.Log("Start");
        }

        private void PlayVideoAd(string path)
        {
            Debug.LogErrorFormat(D.L("[ADS]") + "GraphQLAdsPopupVideo::PlayVideoAd Path:{0}\n", path);

            Canvas.worldCamera = TargetCamera;
            VideoPlayer.url = path;
            VideoPlayer.Play();
        }
        
        [Button(ButtonSizes.Medium)]
        public void PlayVideoDebug()
        {
            PlayVideoAd(DebugAdPath);
        }

    }
}