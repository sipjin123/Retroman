using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Common;
using Common.Utils;

using CColor = Framework.Color;

namespace Sandbox.Audio
{
    using Framework;
    using Framework.Experimental.ScriptableObjects;

    public struct OnPlaySFXSignal
    {
        public SFX SFX;
        public float Volume;
    }

    public struct OnPlayBGMSignal
    {
        public BGM BGM;
        public float Volume;
        public bool Override;
    }

    public struct OnResumSFXSignal
    {
        public bool IsResume;
    }

    public struct OnResumBGMSignal
    {
        public bool IsResume;
    }

    public struct OnResumAllAudioSignal
    {
        public bool IsResume;
    }

    public struct OnStopSFXSignal
    {
    }

    public struct OnStopBGMSignal
    {
    }

    public struct OnStopAllAudioignal
    {
    }

    public struct OnLoadAudioSignal
    {
        public List<AudioContainer> Container;
    }

    public struct OnClearLoadedAudioSignal
    {
    }

    public class AudioPlayer : SerializedMonoBehaviour
    {
        public const string AUDIO_SOURCE_SINGLE = "Audio_Source (Single)";
        public const string AUDIO_SOURCE_DOUBLE = "Audio_Source (Double)";

        [SerializeField]
        [TabGroup("Configs")]
        private bool IsEnabled = false;

        [SerializeField]
        [TabGroup("Configs")]
        private BGM BGMPlaying = BGM.Invalid;

        [SerializeField]
        [TabGroup("Configs")]
        private SFX SFXPlaying = SFX.Invalid;

        [SerializeField]
        [PropertyRange(0f, 10f)]
        [TabGroup("Configs")]
        private float FadeDuration = 0.25f;

        [SerializeField]
        [PropertyRange(0f, 1f)]
        [TabGroup("Configs")]
        private float _MasterVolume = 1.0f;
        public float MasterVolume
        {
            get { return _MasterVolume; }
        }

        [SerializeField]
        [PropertyRange(0f, 1f)]
        [TabGroup("Configs")]
        private float _SFXVolume = 1.0f;
        public float SFXVolume
        {
            get { return _SFXVolume; }
            private set { _SFXVolume = value; }
        }

        [SerializeField]
        [PropertyRange(0f, 1f)]
        [TabGroup("Configs")]
        private float _BGMVolume = 1.0f;
        public float BGMVolume
        {
            get { return _BGMVolume; }
            private set { _BGMVolume = value; }
        }

        [SerializeField]
        [TabGroup("Configs")]
        private BoolReactiveProperty IsSFXEnabled;
        
        [SerializeField]
        [TabGroup("Configs")]
        private BoolReactiveProperty IsBGMEnabled;

        [SerializeField]
        [TabGroup("Configs")]
        private BoolReactiveProperty IsMuted;

        [SerializeField]
        [TabGroup("New Group", "References")]
        private PoolRequest Pool;

        [SerializeField]
        [TabGroup("New Group", "References")]
        private List<AudioSource> AudioSFX;

        [SerializeField]
        [TabGroup("New Group", "References")]
        private List<DoubleAudioSource> AudioBGM;

        [SerializeField]
        [TabGroup("New Group", "References")]
        private Dictionary<SFX, AudioClip> SFXAudioMap;

        [SerializeField]
        [TabGroup("New Group", "References")]
        private Dictionary<BGM, AudioClip> BGMAudioMap;
        
        protected virtual void Awake()
        {
            IsSFXEnabled
                .Subscribe(val => AudioSFX.ForEach(sfx => sfx.mute = !val))
                .AddTo(this);

            IsBGMEnabled
                .Subscribe(val => AudioBGM.ForEach(bgm => bgm.MuteAudio(!val)))
                .AddTo(this);

            IsMuted
                .Subscribe(val =>
                {
                    AudioSFX.ForEach(sfx => sfx.mute = val);
                    AudioBGM.ForEach(bgm => bgm.MuteAudio(val));
                })
                .AddTo(this);

            this.Receive<OnPlaySFXSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => PlaySFX(_.SFX, _.Volume))
                .AddTo(this);

            this.Receive<OnPlayBGMSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => PlayBgm(_.BGM, _.Volume, _.Override))
                .AddTo(this);

            this.Receive<OnResumSFXSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => ResumeSFX(_.IsResume))
                .AddTo(this);

            this.Receive<OnResumBGMSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => ResumeBGM(_.IsResume))
                .AddTo(this);

            this.Receive<OnResumAllAudioSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => ResumeAllAudio(_.IsResume))
                .AddTo(this);

            this.Receive<OnStopSFXSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => StopSFX())
                .AddTo(this);

            this.Receive<OnStopBGMSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => StopBGM())
                .AddTo(this);

            this.Receive<OnStopAllAudioignal>()
                .Where(_ => IsEnabled)
               .Subscribe(_ => StopAllAudio())
               .AddTo(this);

            this.Receive<OnLoadAudioSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => _.Container.ForEach(c => LoadAudioFromContainer(c)))
                .AddTo(this);

            this.Receive<OnClearLoadedAudioSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(_ => ClearAllAudio())
                .AddTo(this);
        }

        protected virtual void OnEnable()
        {
            Factory.Register<AudioPlayer>(this);
        }

        protected virtual void OnDisable()
        {
            Factory.Clean<AudioPlayer>();
        }

        /// <summary>
        /// NOTE: Temp integration of load and unload audio from container
        /// </summary>
        /// <param name="container"></param>
        private void LoadAudioFromContainer(AudioContainer container)
        {
            // unload sfx
            if (container.HasSFX)
            {
                UnloadAudio<SFX>(ref SFXAudioMap, container.ContainsSFX, UnloadSFX);
                LoadAudio<SFX>(ref SFXAudioMap, container.SFX);
            }

            // unload bgm
            if (container.HasBGM)
            {
                UnloadAudio<BGM>(ref BGMAudioMap, container.ContainsBGM, UnloadBgm);
                LoadAudio<BGM>(ref BGMAudioMap, container.BGM);
            }
        }

        private void LoadAudio<T>(ref Dictionary<T, AudioClip> source, Dictionary<T, AudioClip> toLoad) where T : struct
        {
            foreach (KeyValuePair<T, AudioClip> pair in toLoad)
            {
                if (!source.ContainsKey(pair.Key))
                {
                    source.Add(pair.Key, pair.Value);
                }

                source[pair.Key] = pair.Value;
            }
        }

        private void UnloadAudio<T>(ref Dictionary<T, AudioClip> source, Func<T, bool> contains, Action<T> unload) where T : struct
        {
            T[] keys = source.Keys.ToArray();
            foreach (T t in keys)
            {
                if (!contains(t))
                {
                    unload(t);
                }
            }
        }
        
        private void UnloadSFX(SFX sfx)
        {
            if (SFXAudioMap.ContainsKey(sfx))
            {
                SFXAudioMap[sfx].UnloadAudioData();
                SFXAudioMap.Remove(sfx);
            }
        }

        private void UnloadBgm(BGM bgm)
        {
            if (BGMAudioMap.ContainsKey(bgm))
            {
                BGMAudioMap[bgm].UnloadAudioData();
                BGMAudioMap.Remove(bgm);
            }
        }

        private void ClearAllAudio()
        {
            while (SFXAudioMap.Count > 0)
            {
                KeyValuePair<SFX, AudioClip> iter = SFXAudioMap.First();
                SFXAudioMap[iter.Key].UnloadAudioData();
                SFXAudioMap.Remove(iter.Key);
            }

            while (BGMAudioMap.Count > 0)
            {
                KeyValuePair<BGM, AudioClip> iter = BGMAudioMap.First();
                BGMAudioMap[iter.Key].UnloadAudioData();
                BGMAudioMap.Remove(iter.Key);
            }
        }
        
        #region Pause / Stop / Resume Audio (SFX and BGM)
        private void ResumeSFX(bool resume)
        {
            IsSFXEnabled.Value = resume;
        }

        private void ResumeBGM(bool resume)
        {
            IsBGMEnabled.Value = resume;
        }

        private void ResumeAllAudio(bool resume)
        {
            IsMuted.Value = !resume;
        }

        private void StopSFX()
        {
            AudioSFX.ForEach(sfx => sfx.Stop());
            IsSFXEnabled.Value = false;
        }

        private void StopBGM()
        {
            AudioBGM.ForEach(bgm => bgm.StopAudio());
            IsBGMEnabled.Value = false;
        }

        private void StopAllAudio()
        {
            IsSFXEnabled.Value = false;
            IsBGMEnabled.Value = false;
            IsMuted.Value = true;

            AudioSFX.ForEach(sfx => sfx.Stop());
            AudioBGM.ForEach(bgm => bgm.StopAudio());
        }
        #endregion

        public bool IsSFXplaying()
        {
            return AudioSFX.Exists(sfx => sfx.isPlaying);
        }

        public bool IsBGMPlaying()
        {
            return AudioBGM.Exists(bgm => bgm.isPlaying);
        }
        
        public void PlayBgm(BGM bgm, float volume = 1.0f, bool overridePlay = true)
        {
            if (!IsBGMEnabled.Value || IsMuted.Value)
            {
                return;
            }
            
            Assertion.Assert(BGMAudioMap.ContainsKey(bgm), string.Format(D.ERROR +" {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));
            Assertion.AssertNotNull(BGMAudioMap[bgm], string.Format(D.ERROR + " {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));

            List<DoubleAudioSource> sourceList = ResolveSource<DoubleAudioSource>(ref AudioBGM, AUDIO_SOURCE_DOUBLE, s => s.isPlaying);
            DoubleAudioSource source = null;
          
            if (IsBGMPlaying() && overridePlay)
            {
                source = AudioBGM.Find(_ => _.isPlaying);
            }
            else
            {
                source = sourceList.FirstOrDefault();
            }

            source.CrossFade(BGMAudioMap[bgm], volume, FadeDuration, 0f, true);
            source.gameObject.name = string.Format("{0}_{1}_BGM", AUDIO_SOURCE_DOUBLE, bgm.ToString());
            BGMPlaying = bgm;
            BGMVolume = volume;
        }
        
        public void PlaySFX(SFX sfx, float volume = 1.0f)
        {
            if (!IsSFXEnabled.Value || IsMuted.Value)
            {
                return;
            }
            
            Assertion.Assert(SFXAudioMap.ContainsKey(sfx), string.Format(D.ERROR + " {0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));
            Assertion.AssertNotNull(SFXAudioMap[sfx], string.Format(D.ERROR + " {0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));

            List<AudioSource> sourceList = ResolveSource<AudioSource>(ref AudioSFX, AUDIO_SOURCE_DOUBLE, s => s.isPlaying);
            AudioSource source = sourceList.FirstOrDefault();

            source.clip = SFXAudioMap[sfx];
            source.gameObject.name = string.Format("{0}_{1}_SFX", AUDIO_SOURCE_SINGLE, sfx.ToString());
            source.loop = false;
            source.Play();
            SFXPlaying = sfx;
            SFXVolume = volume;
        }
        
        private List<T> ResolveSource<T>(ref List<T> source, string request, Func<T, bool> isPlaying) where T : Component
        {
            if (source.Count <= 0 || !source.Exists(_ => !isPlaying(_)))
            {
                source.Add(Pool.Request<T>(request));
            }

            return source.FindAll(_ => !isPlaying(_));
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void ToggleSFX()
        {
            ResumeSFX(!IsSFXEnabled.Value);
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void ToggleBGM()
        {
            ResumeBGM(!IsBGMEnabled.Value);
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void ToggleMute()
        {
            ResumeAllAudio(!IsMuted.Value);
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void TestStopSFX()
        {
            StopSFX();
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void TestStopBGM()
        {
            StopBGM();
        }

        [SerializeField]
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Debug")]
        public void TestStopAll()
        {
            StopAllAudio();
        }
    }
}