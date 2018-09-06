using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

using uPromise;

using UniRx;

using Common;
using Common.Query;
using Common.Signal;

namespace Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    // alias
    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    #region Scene extension (Load, Unload, and Wait)

    public partial class SceneObject : SerializedMonoBehaviour
    {
        public void PassDataToScene<T>(string scene, ISceneData data) where T : SceneObject
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<GameObject> scenes = objects.FindAll(g => g.GetComponent<T>() != null);
            List<T> items = scenes.ToArray<T>();

            Assertion.Assert(items.Count > 0, "Error! Scene:" + gameObject.name);

            // pass the data 
            items.FirstOrDefault().SceneData = data;
        }

        public Promise Clean()
        {
            Deferred def = new Deferred();
            StartCoroutine(this.Clean(def));
            return def.Promise;
        }

        protected virtual IEnumerator Clean(Deferred def)
        {
            yield return null;
            def.Resolve();
        }

        public Promise EndFrame()
        {
            Deferred def = new Deferred();
            StartCoroutine(EndFrame(def));
            return def.Promise;
        }

        public Promise Wait()
        {
            Deferred def = new Deferred();
            StartCoroutine(Wait(def));
            return def.Promise;
        }

        public Promise Wait(float seconds)
        {
            Deferred def = new Deferred();
            StartCoroutine(Wait(def, seconds));
            return def.Promise;
        }

        protected IEnumerator EndFrame(Deferred def)
        {
            yield return new WaitForEndOfFrame();
            def.Resolve();
        }

        protected IEnumerator Wait(Deferred def, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            def.Resolve();
        }

        protected virtual IEnumerator Wait(Deferred def)
        {
            yield return null;
            def.Resolve();
        }

        public Promise LoadSceneAdditive<T>(string scene) where T : SceneObject
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadSceneAdditive<T>(def, scene));
            return def.Promise;
        }

        public Promise LoadSceneAdditive<T>(string scene, ISceneData data) where T : SceneObject
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadSceneAdditive<T>(def, scene, data));
            return def.Promise;
        }

        public Promise UnloadScene<T>(string scene) where T : SceneObject
        {
            Deferred def = new Deferred();
            StartCoroutine(UnloadScene<T>(def, scene));
            return def.Promise;
        }

        public IEnumerator LoadSceneAdditive<T>(Deferred def, string scene) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;
            def.Resolve();
        }

        public IEnumerator LoadSceneAdditive<T>(Deferred def, string scene, ISceneData data) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene, data);

            def.Resolve();
        }

        public IEnumerator UnloadScene<T>(Deferred def, string scene) where T : SceneObject
        {
            yield return SceneManager.UnloadSceneAsync(scene);
            def.Resolve();
        }
        
        public T GetSceneRoot<T>(string scene) where T : SceneObject
        {
            return SceneObject.GetScene<T>(scene);
        }
    }
    #endregion
}