using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;

using uPromise;

using UniRx;

using Common;
using Common.Query;
using Common.Signal;

using Common.Utils;

namespace Framework
{
    using Sandbox.ButtonSandbox;

    using UScene = UnityEngine.SceneManagement.Scene;
    
    public interface IScene
    {
        ISceneData SceneData { get; set; }
        T GetSceneData<T>() where T : ISceneData;
    }

    [Serializable]
    public class SceneEntry
    {
        public string Scene;
        public GameObject RootObject;
    }

    /// <summary>
    /// This is the base MVP Presenter class to be extended by each scene root.
    /// </summary>
    public partial class Scene : SerializedMonoBehaviour
    {
        /// <summary>
        /// String Dropdown representation of EScene enum in Editor
        /// </summary>
        [SerializeField]
        [TabGroup("Scene")]
        [LabelText("SceneType")]
        [ValueDropdown("Scenes")]
        [OnValueChanged("UpdateSceneTypeString")]
        private string SelectedScene;

        /// <summary>
        /// The type/ID of the scene this root is for.
        /// This should match the scene's name.
        /// </summary>
        private EScene _SceneType;
        public EScene SceneType
        {
            get { return _SceneType; }
            protected set { _SceneType = value; }
        }
        
        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        private ISceneData _SceneData = null;
        public ISceneData SceneData
        {
            get { return _SceneData; }
            protected set { _SceneData = value; }
        }

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

        /// <summary>
        /// Persistent scenes indicates that they are exempted from UnloadScenes.
        /// Developers must manually unload the scene
        /// </summary>
        [SerializeField]
        [TabGroup("Scene")]
        private bool _IsPersistent = false;
        public bool IsPersistent
        {
            get { return _IsPersistent; }
            private set { _IsPersistent = value; }
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
        
        [SerializeField]
        [TabGroup("Scene")]
        protected List<ConcreteInstaller> Installers = new List<ConcreteInstaller>();

        /// <summary>
        /// Holder for subscriptions to be disposed when this Scene is disabled.
        /// </summary>
        protected CompositeDisposable OnDisableDisposables = new CompositeDisposable();

        protected SystemCanvas SystemCanvas;

        #region Unity Life Cycle

        protected virtual void Awake()
        {
            // Update Scene Type & Depth from Editor
            SelectedScene = CachedScene;
            SceneType = CachedScene.ToEnum<EScene>();

            // Update canvas settings
            SetupSceneCanvas();
            
            // Cache the Root scene object
            //Factory.Get<SceneReference>().Add(this);
            this.Publish(new OnCacheSceneSignal(this));
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            // Update Scene Type & Depth from Editor
            SelectedScene = CachedScene;
            SceneType = CachedScene.ToEnum<EScene>();

            this.Receive<ButtonClickedSignal>()
                .Subscribe(sig => OnClickedButton(sig))
                .AddTo(OnDisableDisposables);

            this.Receive<ButtonHoveredSignal>()
                .Subscribe(sig => OnHoveredButton(sig))
                .AddTo(OnDisableDisposables);

            this.Receive<ButtonUnhoveredSignal>()
                .Subscribe(sig => OnUnhoveredButton(sig))
                .AddTo(OnDisableDisposables);

            this.Receive<ButtonPressedSignal>()
                .Subscribe(sig => OnPressedButton(sig))
                .AddTo(OnDisableDisposables);

            this.Receive<ButtonReleasedSignal>()
                .Subscribe(sig => OnReleasedButton(sig))
                .AddTo(OnDisableDisposables);
        }

        protected virtual void OnDisable()
        {
            // dispose all subscriptions and clear list
            OnDisableDisposables.Clear();
        }

        protected virtual void OnDestroy()
        {
            ClearButtonHandler<int, ButtonClickedSignal>(ButtonClickedMap);
            ClearButtonHandler<int, ButtonHoveredSignal>(ButtonHoveredMap);
            ClearButtonHandler<int, ButtonUnhoveredSignal>(ButtonUnhoveredMap);
            ClearButtonHandler<int, ButtonPressedSignal>(ButtonPressedMap);
            ClearButtonHandler<int, ButtonReleasedSignal>(ButtonReleasaedMap);


            //Factory.Get<SceneReference>().Remove(this);
            this.Publish(new OnCleanSceneSignal(this));
        }

        protected virtual void Install()
        {
            Installers.ForEach(i => i.Install());
        }

        protected virtual void UnInstall()
        {
            Installers.ForEach(i => i.UnInstall());
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

        /// <summary>
        /// Intializes the scene's canvases to use the common UI camera.
        /// </summary>
        protected virtual void SetupSceneCanvas()
        {
            SystemCanvas = SystemCanvas ?? GetComponent<SystemCanvas>();
            if (SystemCanvas != null)
            {
                SystemCanvas.SetupSceneCanvas();
            }
            else
            {
                Debug.LogWarningFormat(D.WARNING + " No SystemCanvas attached to SceneRoot. Scene:{0}\n", gameObject.name);
            }
        }

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

        /// <summary>
        /// Returns true if this scene has data.
        /// </summary>
        /// <returns></returns>
        protected bool HasSceneData()
        {
            return SceneData != null;
        }
        
        #region Signals

        private void OnClickedButton(ButtonClickedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonClickedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "Scene::OnClickedButton Button:{0}\n", button);
                ButtonClickedMap[button](signal);
            }
        }

        private void OnHoveredButton(ButtonHoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonHoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "Scene::OnHoveredButton Button:{0}\n", button);
                ButtonHoveredMap[button](signal);
            }
        }

        private void OnUnhoveredButton(ButtonUnhoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonUnhoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "Scene::OnUnhoveredButton Button:{0}\n", button);
                ButtonUnhoveredMap[button](signal);
            }
        }

        private void OnPressedButton(ButtonPressedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonPressedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "Scene::OnPressedButton Button:{0}\n", button);
                ButtonPressedMap[button](signal);
            }
        }

        private void OnReleasedButton(ButtonReleasedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonReleasaedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "Scene::OnReleasedButton Button:{0}\n", button);
                ButtonReleasaedMap[button](signal);
            }
        }

        #endregion

        #region Helpers

        public static bool HasScene(EScene scene)
        {
            return Factory.Get<SceneReference>().Exists(scene);
        }

        public static bool HasScene(string scene)
        {
            return IsLoaded(scene);
        }

        public static T GetScene<T>(EScene scene) where T : Scene
        {
            Debug.LogFormat(D.F + "Scene::GetScene Scene:{0} Type:{1} IsLoaded:{2}\n", scene, typeof(T).FullName, IsLoaded(scene));

            if (!HasScene(scene))
            {
                return null;
            }
            
            SceneEntry entry = Factory.Get<SceneReference>().Find(scene);
            return entry.RootObject.GetComponent<T>();
        }

        public static T GetScene<T>(string scene) where T : Scene
        {
            if (!IsLoaded(scene))
            {
                return null;
            }

            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> rootObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<T> scenes = rootObjects.ToArray<T>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(scenes.Count == 1, D.ERROR + " Scene::GetScene invalid scene! Scene:{0} Type:{1}\n", scene, typeof(T).FullName);

            return scenes[0];
        }

        public static T GetSceneObject<T>(string scene) where T : SceneObject
        {
            if (!IsLoaded(scene))
            {
                return null;
            }

            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> rootObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<T> scenes = rootObjects.ToArray<T>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(scenes.Count == 1, D.ERROR + " Scene::GetScene invalid scene! Scene:{0} Type:{1}\n", scene, typeof(T).FullName);

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