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
    public class BGMUtil : MonoBehaviour
    {
        public BGM Key;
        public AudioClip Clip;
        public bool Override = true;

        [Range(0f, 1f)]
        public float Volume;

        private void Start()
        {
            this.Publish(new OnLoadBGMSignal()
            {
                Key = Key,
                Clip = Clip,
            });

            Rename();
        }
        
        [Button(ButtonSizes.Medium)]
        public void Rename()
        {
            gameObject.name = string.Format("BGM Player ({0})", Key);
        }

        [Button(ButtonSizes.Medium)]
        public void Play()
        {
            this.Publish(new OnResumBGMSignal() { IsResume = true });
            this.Publish(new OnPlayBGMSignal() { BGM = Key, Volume = Volume, Override = Override});
        }

        [Button(ButtonSizes.Medium)]
        public void Stop()
        {
            this.Publish(new OnStopBGMSignal());
        }
    }
}
