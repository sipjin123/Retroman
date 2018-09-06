using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Video;

using uPromise;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Framework
{
    public struct OnSplashDoneSignal
    {

    }
    
    public class SplashMovieRoot : SceneObject
    {
        [SerializeField]
        private bool Done = false;

        [SerializeField]
        private VideoPlayer Player;
        
        #region Unity Life Cycle
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            Player.targetCamera = SystemCanvas.Camera;
            Player.Play();

            StartCoroutine(PlaySplash());
        }

        protected override IEnumerator Wait(Deferred def)
        {
            YieldInstruction endFrame = new WaitForEndOfFrame();

#if ENABLE_SPLASH_VIDEO
            while (!Done)
            {
                yield return endFrame;
            }
#endif

            // resolve
            yield return endFrame;
            def.Resolve();
        }
        #endregion

        private IEnumerator PlaySplash()
        {
            YieldInstruction endFrame = new WaitForEndOfFrame();

#if ENABLE_SPLASH_VIDEO
            Player.Play();
            
            yield return new WaitForSeconds(4.0f);
            
            while (Player.isPlaying)
            {
                yield return endFrame;
            }
#endif

            Player.enabled = false;
            Done = true;

            yield return endFrame;
            this.Publish(new OnSplashDoneSignal());
        }
    }
}

