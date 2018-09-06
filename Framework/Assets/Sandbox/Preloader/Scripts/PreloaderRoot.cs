using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

namespace Sandbox.Preloader
{
    using Framework;

    // alias
    using UColor = UnityEngine.Color;
    using URandom = UnityEngine.Random;
    using UScene = UnityEngine.SceneManagement.Scene;
    using CColor = Framework.Color;
    using CSScene = Framework.Scene;

    /// <summary>
    /// Enum of loading screen IDs.
    /// </summary>
    [Flags]
    public enum Preloaders
    {
        Invalid = 0x0,

        Preloader001 = 0x1 << 0,
        Preloader002 = 0x1 << 1,
        Preloader003 = 0x1 << 2,
        Preloader004 = 0x1 << 3,

        Max = 0x1 << 4,
    };
    
    /// <summary>
    /// Scene loading transision helper class.
    /// Touch input blocker.
    /// To use, sequence the following promises:
    /// 1) LoadLoadingScreen()
    /// 2) FadeInLoadingScreen()
    /// 3) ... load additive scenes to be loaded
    /// 4) FadeOutLoadingScreen().
    /// </summary>
    public class PreloaderRoot : Scene
    {
        public const float IN_DURATION = 0.5f;
        public const float OUT_DURATION = 0.5f;
        public const float FIXED_DELTA = 0.01656668f;

        public static readonly Vector2 TARGET_RESOLUTION = new Vector2(1536.0f, 2048.0f);

        [SerializeField]
        [TabGroup("New Group", "Preloader")]
        private int PlaneDistance = 20;

        /// <summary>
        /// True while the loading screen is being loaded or active.
        /// </summary>
        [SerializeField]
        [TabGroup("New Group", "Preloader")]
        protected bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            protected set { _IsLoading = value; }
        }

        /// <summary>
        /// This swallows all the touch/mouse input when enabled
        /// </summary>
        [SerializeField]
        [TabGroup("New Group", "Preloader")]
        protected GameObject _Blocker;
        public GameObject Blocker
        {
            get { return _Blocker; }
        }

        /// <summary>
        /// This parents loaded loading screens.
        /// </summary>
        [SerializeField]
        [TabGroup("New Group", "Preloader")]
        protected Transform _Container;
        public Transform Container
        {
            get { return _Container; }
        }

        /// <summary>
        /// The currently loaded loading screen.
        /// </summary>
        [SerializeField]
        [TabGroup("New Group", "Preloader")]
        protected List<PreloaderItem> _PreloaderItems;
        public List<PreloaderItem> PreloaderItems
        {
            get { return _PreloaderItems; }
        }

        #region Unity Life Cycle

        protected override void Awake()
        {
            base.Awake();

            // force set scene type and depth
            SceneType = EScene.Preloader;

            Assertion.Assert(Blocker, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Blocker:{0} is null!\n", Blocker));
            Assertion.Assert(Container, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Container:{0} is null!\n", Container));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        #endregion
        
        #region Preloader Promise

        /// <summary>
        /// Loads the preloader by enabling the blocker and loading the loading screen to be displayed.
        /// </summary>
        /// <returns></returns>
        public Promise LoadLoadingScreenPromise()
        {
            Preloaders images = Preloaders.Preloader001 | Preloaders.Preloader002 | Preloaders.Preloader003 | Preloaders.Preloader004;
            return LoadLoadingScreenPromise(images);
        }

        public Promise LoadLoadingScreenPromise(Preloaders images)
        {
            if (IsLoading)
            {
                return EndFramePromise();
            }

            var matches = Enum.GetValues(typeof(Preloaders))
                            .Cast<Preloaders>()
                            .Where(i => images.Has(i))
                            .ToList();

            matches.Remove(Preloaders.Invalid);
            matches.Remove(Preloaders.Max);

            var match = matches.Random();
            
            IsLoading = true;
            Blocker.SetActive(true);

            // load scene with loading screen
            AsyncOperation operation = SceneManager.LoadSceneAsync(match.ToString(), LoadSceneMode.Additive);
            
            Deferred deferred = new Deferred();
            StartCoroutine(LoadLoadingScreen(deferred, match, operation));
            return deferred.Promise;
        }

        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeInLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeInLoadingScreen(deferred));
            return deferred.Promise;
        }

        /// <summary>
        /// Fades out the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeOutLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeOutLoadingScreen(deferred));
            return deferred.Promise;
        }

        public Promise WaitNotifier()
        {
            return PreloaderItems.FirstOrDefault().WaitProgress();
        }

        public ScheduledNotifier<float> GetNotifier()
        {
            return PreloaderItems.FirstOrDefault().Progress;
        }
        
        #endregion

        #region Coroutines

        /// <summary>
        /// Loads a random loading screen.
        /// </summary>
        /// <param name="deffered"></param>
        /// <returns></returns>
        protected virtual IEnumerator LoadLoadingScreen(Deferred deffered, Preloaders image, AsyncOperation operation)
        {
            yield return null;

            // TODO: +AS:180404
            //  - Fix canvas sorting with SystemCanvas (Add SystemCanvas Support)
            //  - Disabled temporarily
            //*
            string imageScene = image.ToString();

            // load scene with loading screen
            yield return operation;
            
            // get objects in the scene
            UScene loadedScene = SceneManager.GetSceneByName(imageScene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<PreloaderItem> items = objects.ToArray<PreloaderItem>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(items.Count == 1);

            // get the first and only object (which should be the loading screen image)
            PreloaderItem preloader = items.FirstOrDefault();
            Assertion.AssertNotNull(preloader);

            PreloaderItems.Add(preloader);

            // fix object parenting setup
            Transform root = Container;
            preloader.transform.SetParent(root);
            preloader.transform.SetAsFirstSibling();
            preloader.gameObject.SetActive(true);

            CanvasSetup setup = new CanvasSetup();
            setup.RenderMode = RenderMode.ScreenSpaceCamera;
            setup.PlaneDistance = PlaneDistance;
            setup.SceneDepth = ESceneDepth.Overlay;
            setup.Canvas = preloader.Canvas;

            SystemCanvas.AddCanvas(setup);
            
            SetupSceneCanvas();

            //SceneManager.UnloadScene(imageScene);
            yield return SceneManager.UnloadSceneAsync(imageScene);

            deffered.Resolve();
            //*/
        }

        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        protected virtual IEnumerator FadeInLoadingScreen(Deferred deferred)
        {
            yield return null;

            float timer = 0.0f;
            Deferred def = deferred;

            while (timer <= IN_DURATION)
            {
                float scale = Mathf.Clamp((timer += FIXED_DELTA), 0.0f, IN_DURATION) / IN_DURATION;
                PreloaderItems.ForEach(p => p.Group.alpha = scale);
                yield return null;
            }

            PreloaderItems.ForEach(p => p.Group.alpha = 1f);
            yield return null;

            def.Resolve();
        }

        /// <summary>
        /// Fades out the loading screen and disables the blocker.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        protected virtual IEnumerator FadeOutLoadingScreen(Deferred deferred)
        {
            yield return null;

            float timer = 0.0f;
            Deferred def = deferred;

            while (timer <= OUT_DURATION)
            {
                float scale = 1.0f - Mathf.Clamp((timer += FIXED_DELTA), 0.0f, OUT_DURATION) / OUT_DURATION;
                PreloaderItems.ForEach(p => p.Group.alpha = scale);
                yield return null;
            }

            PreloaderItems.ForEach(p => p.Group.alpha = 0f);
            yield return null;
            
            PreloaderItem preloader = PreloaderItems.FirstOrDefault();
            PreloaderItems.Remove(preloader);
            SystemCanvas.RemoveCanvas(preloader.Canvas);

            GameObject.Destroy(preloader.gameObject);
            
            IsLoading = false;
            Blocker.gameObject.SetActive(false);

            def.Resolve();
        }
        
        #endregion
    }

}
 