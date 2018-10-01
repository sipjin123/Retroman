using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

namespace Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Sandbox.ButtonSandbox;

    // alias
    using UScene = UnityEngine.SceneManagement.Scene;

    public partial class SceneObject : SerializedMonoBehaviour
    {
        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        protected ISceneData _SceneData = null;
        public ISceneData SceneData
        {
            get { return _SceneData; }
            protected set { _SceneData = value; }
        }

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonClickedSignal>> ButtonClickedMap = new Dictionary<int, Action<ButtonClickedSignal>>();
        
        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonHoveredSignal>> ButtonHoveredMap = new Dictionary<int, Action<ButtonHoveredSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonUnhoveredSignal>> ButtonUnhoveredMap = new Dictionary<int, Action<ButtonUnhoveredSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonPressedSignal>> ButtonPressedMap = new Dictionary<int, Action<ButtonPressedSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonReleasedSignal>> ButtonReleasaedMap = new Dictionary<int, Action<ButtonReleasedSignal>>();

        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        public T GetSceneData<T>() where T : ISceneData
        {
            return (T)SceneData;
        }

        /// <summary>
        /// Returns the name of the GameObject where the presenter is attached.
        /// </summary>
        public string Name
        {
            get { return gameObject.name; }
        }

        protected SystemCanvas SystemCanvas;

        #region Unity Life Cycle
        protected virtual void Awake()
        {
            string sceneRoot = gameObject.scene.name;
            Assertion.Assert(sceneRoot.Equals(Name), D.ERROR + "SceneObject::Awake RootObject:{0} must have the same name with SceneRoot:{1}!\n", Name, sceneRoot);

            SetupSystemCanvas();

            this.Publish(new OnCacheSceneObjectSignal(this));
        }

        protected virtual void Start()
        {
            SetupSystemCanvas();

            this.Receive<ButtonClickedSignal>()
                .Subscribe(sig => OnClickedButton(sig))
                .AddTo(this);

            this.Receive<ButtonHoveredSignal>()
                .Subscribe(sig => OnHoveredButton(sig))
                .AddTo(this);

            this.Receive<ButtonUnhoveredSignal>()
                .Subscribe(sig => OnUnhoveredButton(sig))
                .AddTo(this);

            this.Receive<ButtonPressedSignal>()
                .Subscribe(sig => OnPressedButton(sig))
                .AddTo(this);

            this.Receive<ButtonReleasedSignal>()
                .Subscribe(sig => OnReleasedButton(sig))
                .AddTo(this);
        }
        
        protected virtual void OnDestroy()
        {
            this.Publish(new OnCleanSceneObjectSignal(this));

            ClearButtonHandler<int, ButtonClickedSignal>(ButtonClickedMap);
            ClearButtonHandler<int, ButtonHoveredSignal>(ButtonHoveredMap);
            ClearButtonHandler<int, ButtonUnhoveredSignal>(ButtonUnhoveredMap);
            ClearButtonHandler<int, ButtonPressedSignal>(ButtonPressedMap);
            ClearButtonHandler<int, ButtonReleasedSignal>(ButtonReleasaedMap);
        }

        protected virtual void SetupSystemCanvas()
        {
            SystemCanvas = GetComponent<SystemCanvas>();
            
            if (SystemCanvas != null)
            {
                SystemCanvas.SetupSceneCanvas();
            }
        }
        #endregion

        #region Buttons
        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonClickedSignal> action)
        {
            ButtonClickedMap[button.Value] = (Action<ButtonClickedSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonHoveredSignal> action)
        {
            ButtonHoveredMap[button.Value] = (Action<ButtonHoveredSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonUnhoveredSignal> action)
        {
            ButtonUnhoveredMap[button.Value] = (Action<ButtonUnhoveredSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonPressedSignal> action)
        {
            ButtonPressedMap[button.Value] = (Action<ButtonPressedSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonReleasedSignal> action)
        {
            ButtonReleasaedMap[button.Value] = (Action<ButtonReleasedSignal>)action;
        }
        
        private void OnClickedButton(ButtonClickedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonClickedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnClickedButton Button:{0}\n", button);
                ButtonClickedMap[button](signal);
            }
        }

        private void OnHoveredButton(ButtonHoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonHoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnHoveredButton Button:{0}\n", button);
                ButtonHoveredMap[button](signal);
            }
        }

        private void OnUnhoveredButton(ButtonUnhoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonUnhoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnUnhoveredButton Button:{0}\n", button);
                ButtonUnhoveredMap[button](signal);
            }
        }

        private void OnPressedButton(ButtonPressedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonPressedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnPressedButton Button:{0}\n", button);
                ButtonPressedMap[button](signal);
            }
        }

        private void OnReleasedButton(ButtonReleasedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonReleasaedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnReleasedButton Button:{0}\n", button);
                ButtonReleasaedMap[button](signal);
            }
        }

        private void ClearButtonHandler<K, V>(Dictionary<K, Action<V>> handler)
        {
            if (handler != null)
            {
                handler.Clear();
                handler = null;
            }
        }
        #endregion

        #region Helpers
        public static bool HasScene<T>(string scene) where T : SceneObject
        {
            if (Factory.Get<SceneReference>().Exists(scene))
            {
                return true;
            }

            return IsLoaded(scene);
        }
        
        public static T GetScene<T>(string scene) where T : SceneObject
        {
            if (HasScene<T>(scene))
            {
                SceneEntry entry = Factory.Get<SceneReference>().Find(scene);
                return entry.RootObject.GetComponent<T>();
            }
            
            if (!IsLoaded(scene))
            {
                return null;
            }

            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> rootObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<T> scenes = rootObjects.ToArray<T>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(scenes.Count == 1, D.ERROR + " SceneObject::GetScene invalid scene! Scene:{0} Type:{1}\n", scene, typeof(T).FullName);

            return scenes[0];
        }

        public static bool IsLoaded(EScene scene)
        {
            return IsLoaded(scene.ToString());
        }

        public static bool IsLoaded(string scene)
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            return loadedScene.isLoaded;
        }
        #endregion
    }
}

