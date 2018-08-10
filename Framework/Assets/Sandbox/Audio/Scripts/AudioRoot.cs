using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Common;
using Common.Query;

using Framework;

namespace Sandbox.Audio
{
    /// <summary>
    /// This handles playing of common sound effects like button hovers and clicks.
    /// </summary>
    public class AudioRoot : Scene
    {
        /// <summary>
        /// The audio player to be used in playing clips.
        /// </summary>
        [SerializeField]
        private AudioPlayer AudioPlayer;

        protected override void Awake()
        {
            base.Awake();
            
            Assertion.AssertNotNull(AudioPlayer);

            QuerySystem.RegisterResolver(QueryIds.MusicIsMute, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(AudioPlayer.IsMuted);
            });

            /*
            if (Prefs.GetInt(Const.MUSIC_MUTE, 0) == 1)
            {
                AudioPlayer.MuteAllAudio(true);
            }
            else
            {
                AudioPlayer.MuteAllAudio(false);
            }*/
        }

        protected override void Start()
        {
            base.Start();
            /*
            // play hover sound effect when a button is hovered
            this.Receive<ButtonHoveredSignal>()
                .Subscribe(_ => AudioPlayer.PlaySFX(SFX.Sfx001))
                .AddTo(this);

            // play click sound effect when a button is clicked
            this.Receive<ButtonClickedSignal>()
                .Subscribe(_ => AudioPlayer.PlaySFX(SFX.Sfx002))
                .AddTo(this);*/
        }
    }
}
