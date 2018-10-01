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
    public class SFXUtil : MonoBehaviour
    {
        public SFX Key;
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume;

        private void Start()
        {
            this.Publish(new OnLoadSFXSignal()
            {
                Key = Key,
                Clip = Clip,
            });

            Rename();
        }

        [Button(ButtonSizes.Medium)]
        public void Rename()
        {
            gameObject.name = string.Format("SFX Player ({0})", Key);
        }
        
        [Button(ButtonSizes.Medium)]
        public void Play()
        {
            this.Publish(new OnResumSFXSignal() { IsResume = true });
            this.Publish(new OnPlaySFXSignal() { SFX = Key, Volume = Volume });
        }

        [Button(ButtonSizes.Medium)]
        public void Stop()
        {
            this.Publish(new OnStopSFXSignal());
        }
    }
}
