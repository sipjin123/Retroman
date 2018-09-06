using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using uPromise;

using Common;
using Common.Query;

using Framework;

namespace Sandbox.Audio
{
    using FScene = Framework.Scene;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This handles playing of common sound effects like button hovers and clicks.
    /// </summary>
    public class AudioRoot : FScene
    {
        protected override void Start()
        {
            base.Start();

            // play hover sound effect when a button is hovered
            this.Receive<ButtonHoveredSignal>()
                .Subscribe(_ => this.Publish(new OnPlaySFXSignal() { SFX = SFX.Sfx001, Volume = 1f }))
                .AddTo(this);

            // play click sound effect when a button is clicked
            this.Receive<ButtonClickedSignal>()
                .Subscribe(_ => this.Publish(new OnPlaySFXSignal() { SFX = SFX.Sfx002, Volume = 1f }))
                .AddTo(this);
        }

        public Promise LoadAudioContainer(string sceneName)
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadAudio(def, sceneName));
            return def.Promise;
        }

        protected IEnumerator LoadAudio(Deferred def, string sceneName)
        {
            yield return null;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return operation;

            Transform root = transform;
            UScene loadedScene = SceneManager.GetSceneByName(sceneName);
            List<GameObject> rawObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<AudioContainer> objects = rawObjects.ToArray<AudioContainer>();

            this.Publish(new OnLoadAudioSignal() { Container = objects });
            
            operation = SceneManager.UnloadSceneAsync(sceneName);
            yield return operation;

            def.Resolve();
        }

        [Button(ButtonSizes.Medium)]
        public void PlaySFX01()
        {
            this.Publish(new OnPlaySFXSignal() { SFX = SFX.Sfx001, Volume = 1f });
        }

        [Button(ButtonSizes.Medium)]
        public void PlaySFX02()
        {
            this.Publish(new OnPlaySFXSignal() { SFX = SFX.Sfx002, Volume = 1f });
        }

        [Button(ButtonSizes.Medium)]
        public void BGM01()
        {
            this.Publish(new OnPlayBGMSignal() { BGM = BGM.Bgm001, Volume = 1f, Override = true });
        }

        [Button(ButtonSizes.Medium)]
        public void BGM02()
        {
            this.Publish(new OnPlayBGMSignal() { BGM = BGM.Bgm002, Volume = 1f, Override = true });
        }

        [Button(ButtonSizes.Medium)]
        public void ResumeSFX()
        {
            this.Publish(new OnResumSFXSignal() { IsResume = true });
        }

        [Button(ButtonSizes.Medium)]
        public void ResumeBGM()
        {
            this.Publish(new OnResumBGMSignal() { IsResume = true });
        }

        [Button(ButtonSizes.Medium)]
        public void ResumeAll()
        {
            this.Publish(new OnResumAllAudioSignal() { IsResume = true });
        }

        [Button(ButtonSizes.Medium)]
        public void StopSFX()
        {
            this.Publish(new OnStopSFXSignal());
        }

        [Button(ButtonSizes.Medium)]
        public void StopBGM()
        {
            this.Publish(new OnStopBGMSignal());
        }

        [Button(ButtonSizes.Medium)]
        public void StopAll()
        {
            this.Publish(new OnStopAllAudioignal());
        }

        [Button(ButtonSizes.Medium)]
        public void LoadAudioContainerSet1()
        {
            LoadAudioContainer("AudioContainer001");
        }

        [Button(ButtonSizes.Medium)]
        public void LoadAudioContainerSet2()
        {
            LoadAudioContainer("AudioContainer002");
        }

        [Button(ButtonSizes.Medium)]
        public void LoadAudioContainerSet1N2()
        {
            LoadAudioContainer("AudioContainer003");
        }

        [Button(ButtonSizes.Medium)]
        public void UnloadAudio()
        {
            this.Publish(new OnClearLoadedAudioSignal());
        }
    }
}
