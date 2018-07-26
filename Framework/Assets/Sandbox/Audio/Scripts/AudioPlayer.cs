using System;
using System.Collections;
using System.Collections.Generic;

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

    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private BGM BGMPlaying = BGM.Invalid;

        [SerializeField]
        private List<AudioSource> AudioSFX;

        [SerializeField]
        private AudioSource BGMSource;

        [SerializeField]
        private DoubleAudioSource DBGMSource;

        [Range(0f, 5f)]
        [SerializeField]
        private float _MasterVolume = 1.0f;
        public float MasterVolume
        {
            get { return _MasterVolume; }
        }

        [SerializeField]
        private bool IsSFXEnabled = true;

        [SerializeField]
        private bool IsBGMEnabled = true;

        [SerializeField]
        private Dictionary<SFX, AudioClip> SFXAudio;

        [SerializeField]
        private Dictionary<BGM, AudioClip> BGMAudio;
        
        [SerializeField]
        private bool _IsMuted = false;
        public bool IsMuted
        {
            get { return _IsMuted; }
            set { _IsMuted = value; }
        }

        private AudioSource Ambience = null;

        protected virtual void Awake()
        {
            Assertion.AssertNotNull(AudioSFX);
            Assertion.AssertNotNull(BGMSource);
        }

        protected virtual void OnEnable()
        {
            Factory.Register<AudioPlayer>(this);
        }

        protected virtual void OnDisable()
        {
            Factory.Clean<AudioPlayer>();
        }
        
        private void UpdateSFX(bool value)
        {
            IsSFXEnabled = value;
        }

        private void UpdateBGM(bool value)
        {
            IsBGMEnabled = value;
            BGMSource.mute = !value;
        }
        
        public bool IsSFXplaying(int num)
        {
            return AudioSFX[num].isPlaying;
        }

        public bool IsBGMPlaying()
        {
            return BGMSource.isPlaying;
        }

        public bool IsBGMPlaying(BGM bgm)
        {
            if (BGMPlaying != bgm)
            {
                return false;
            }

            return IsBGMPlaying();
        }
        
        public void PlayBgm(BGM bgm, float volume = 1.0f)
        {
            if (!IsBGMEnabled)
            {
                return;
            }
            
            Assertion.Assert(BGMAudio.ContainsKey(bgm), string.Format(D.ERROR +" {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));
            Assertion.AssertNotNull(BGMAudio[bgm], string.Format(D.ERROR + " {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));

            if (BGMSource.isPlaying)
            {
                this.StartCoroutine(this.FadeOut(BGMSource, delegate {
                    BGMSource.clip = BGMAudio[bgm];
                    BGMSource.volume = volume;
                    BGMSource.Play();
                    BGMPlaying = bgm;
                }, 0.75f));
            }
            else
            {
                BGMSource.clip = BGMAudio[bgm];
                BGMSource.volume = volume;
                BGMSource.Play();
                BGMPlaying = bgm;
            }
        }

        public void PauseBGM()
        {
            BGMSource.Pause();
        }

        public void ResumeBGM()
        {
            BGMSource.UnPause();
        }

        public void StopBGM()
        {
            BGMSource.Stop();
            BGMPlaying = BGM.Invalid;
        }

        public void sPlaySFX(SFX sfx, float volume = 1.0f)
        {
            if (!IsSFXEnabled)
            {
                return;
            }
            
            Assertion.Assert(SFXAudio.ContainsKey(sfx), string.Format(D.ERROR + " {0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));
            Assertion.AssertNotNull(SFXAudio[sfx], string.Format(D.ERROR + " {0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));

            foreach (AudioSource audio in AudioSFX)
            {
                if (!audio.isPlaying)
                {
                    audio.clip = SFXAudio[sfx];
                    audio.volume = volume;
                    audio.Play();
                    break;
                }
            }
        }

        public void PlaySFXAsAmbience(SFX sfx, float volume = 1.0f)
        {
            if (!IsSFXEnabled)
            {
                return;
            }
            
            Assertion.Assert(SFXAudio.ContainsKey(sfx), string.Format(D.ERROR + " {0} AudioPlayer::PlaySFXAsAmbience Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));
            Assertion.AssertNotNull(SFXAudio[sfx], string.Format(D.ERROR + " {0} AudioPlayer::PlaySFXAsAmbience Invalid Key! Bgm:{1} Volume:{2}\n", string.Empty, sfx, volume));

            foreach (AudioSource audio in AudioSFX)
            {
                if (!audio.isPlaying)
                {
                    audio.clip = SFXAudio[sfx];
                    audio.volume = volume;
                    audio.Play();
                    Ambience = audio;
                    break;
                }
            }
        }

        public void StopCurrentAmbience()
        {
            if (Ambience != null && Ambience.isPlaying)
            {
                Ambience.Stop();
            }
        }

        public void PlaySFXFadeBgm(SFX sfx, float volume = 1.0f)
        {
            /*
            Assertion.Assert(AUDIO_DURATION.ContainsKey(sfx), "AudioPlayer::PlaySFXFadeBgm Invalid sfx for audio duration. sfx:" + sfx + "\n");
            this.StartCoroutine(this.FadeOut(this.audioBgm, delegate {
                this.PlaySFX(sfx, volume);
            }));

            this.StartCoroutine(this.FadeIn(this.audioBgm, null, AUDIO_DURATION[sfx]));
            //*/
        }

        public void MuteAllAudio(bool val)
        {
            DBGMSource.MuteAudio(val);
            IsMuted = val;
            BGMSource.mute = val;

            foreach (AudioSource audio in AudioSFX)
            {
                audio.mute = val;
            }
        }

        public void BlendAudioBGM(BGM bgm, float volume = 0.75f)
        {
            if (!IsBGMEnabled)
            {
                return;
            }

            Assertion.Assert(BGMAudio.ContainsKey(bgm), string.Format(D.ERROR + " {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));
            Assertion.AssertNotNull(BGMAudio[bgm], string.Format(D.ERROR + " {0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", string.Empty, bgm, volume));

            DBGMSource.CrossFade(BGMAudio[bgm], volume, 0.15f, 0.2f);
            BGMPlaying = bgm;
        }

        public IEnumerator FadeIn(AudioSource audio, Action action = null, float delay = 0.0f)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

            float duration = 0.15f;
            float timer = 0.0f;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                audio.volume = (timer / duration);
                yield return null;
            }

            timer = duration;
            audio.volume = (timer / duration);
            yield return null;

            if (action != null)
            {
                action();
            }
        }

        public IEnumerator FadeOut(AudioSource audio, Action action = null, float duration = 0.15f)
        {
            float timer = 0.0f;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                audio.volume = Mathf.Clamp01(1.0f - (timer / duration));
                yield return null;
            }

            timer = duration;
            audio.volume = Mathf.Clamp01(1.0f - (timer / duration));
            yield return null;

            if (action != null)
            {
                action();
            }
        }
    }
}