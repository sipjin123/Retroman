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

    // alias
    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    public partial class SceneObject : MonoBehaviour
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
        }

        protected virtual void OnDestroy()
        {
            this.Publish(new OnCleanSceneObjectSignal(this));
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

