using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Audio
{
    public class AudioContainer : SerializedMonoBehaviour
    {
        [SerializeField]
        private bool _HasSFX;
        public bool HasSFX
        {
            get { return _HasSFX; }
        }

        public bool ContainsSFX(SFX sfx)
        {
            return SFX.ContainsKey(sfx);
        }

        [SerializeField]
        private bool _HasBGM;
        public bool HasBGM
        {
            get { return _HasBGM; }
        }

        public bool ContainsBGM(BGM bgm)
        {
            return BGM.ContainsKey(bgm);
        }

        [SerializeField]
        private Dictionary<SFX, AudioClip> _SFX;
        public Dictionary<SFX, AudioClip> SFX
        {
            get { return _SFX; }
        }

        [SerializeField]
        private Dictionary<BGM, AudioClip> _BGM;
        public Dictionary<BGM, AudioClip> BGM
        {
            get { return _BGM; }
        }
    }
}


