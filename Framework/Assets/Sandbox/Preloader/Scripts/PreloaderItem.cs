using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using uPromise;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.Preloader
{
    // alias
    using CColor = Framework.Color;

    /// <summary>
    /// This is an interface to be implemented by preloaders that can fade in and fade out.
    /// </summary>
    public interface IPreloader
    {
        ScheduledNotifier<float> Progress { get; }
        
        /// <summary>
        /// Loads/initializes the preloader.
        /// </summary>
        /// <returns>Promise</returns>
        Promise LoadPromise();
        /// <summary>
        /// Fades in the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeInPromise();
        /// <summary>
        /// Fades out the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeOutPromise();
        /// <summary>
        /// Fades out the preloader.
        /// </summary>
        /// <returns></returns>
        Promise WaitProgress();

        void SubscribeProgress();
    };

    public class PreloaderItem : MonoBehaviour, IPreloader
    {
        [SerializeField]
        private Canvas _Canvas;
        public Canvas Canvas
        {
            get { return _Canvas; }
        }

        [SerializeField]
        private CanvasGroup _Group;
        public CanvasGroup Group
        {
            get { return _Group; }
        }

        [SerializeField]
        private Preloaders _Item;
        public Preloaders Item
        {
            get { return _Item; }
        }

        // TODO: +AS:20180528 Move this to Loading preload item
        [SerializeField]
        private Image Fill;

        public string Name
        {
            get { return gameObject.name; }
        }

        private ScheduledNotifier<float> _Progress;
        public ScheduledNotifier<float> Progress
        {
            get { return _Progress; }
            private set { _Progress = value; }
        }

        private void Awake()
        {
            Progress = new ScheduledNotifier<float>();
        }

        #region IPreloader

        /// <summary>
        /// Loads/initializes the preloader.
        /// </summary>
        /// <returns>Promise</returns>
        public Promise LoadPromise()
        {
            Deferred def = new Deferred();
            return def.Promise;
        }

        /// <summary>
        /// Fades in the preloader.
        /// </summary>
        /// <returns></returns>
        public Promise FadeInPromise()
        {
            Deferred def = new Deferred();
            return def.Promise;
        }

        /// <summary>
        /// Fades out the preloader.
        /// </summary>
        /// <returns></returns>
        public Promise FadeOutPromise()
        {
            Deferred def = new Deferred();
            return def.Promise;
        }

        public Promise WaitProgress()
        {
            Deferred def = new Deferred();
            StartCoroutine(WaitProgress(def));
            return def.Promise;
        }

        private IEnumerator WaitProgress(Deferred def)
        {
            while (!(Fill.fillAmount >=1f))
            {
                yield return null;
            }

            yield return null;
            def.Resolve();
        }

        // TODO: +AS:20180528 Move this to Loading preload item
        public void SubscribeProgress()
        {
            Fill.fillAmount = 0f;
            Progress.Subscribe(p => Fill.fillAmount = Mathf.Clamp01(p))
                .AddTo(this);
        }

        #endregion
    }
}