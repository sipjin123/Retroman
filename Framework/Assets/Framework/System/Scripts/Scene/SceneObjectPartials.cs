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

    public partial class SceneObject : MonoBehaviour
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

        /*
        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene eScene) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Unloads everything except the SystemRoot then loads the target scene.
        /// </summary>
        public IEnumerator LoadSceneAsync<T>(Deferred deferred, EScene scene) where T : SceneObject
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene<T>(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadSceneAsync Scene:{0} is already loaded.\n", scene);

                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                deferred.Resolve();
            }
            else
            {
                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());

                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene eScene, ISceneData data) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deferred, EScene scene, ISceneData data) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene<T>(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadSceneAsync Scene:{0} is already loaded. Data:{1}\n", scene, data);

                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                deferred.Resolve();
            }
            else
            {
                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());

                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;

                PassDataToScene<T>(scene.ToString(), data);
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sScenee"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise(string sScenee)
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync(deferred, sScenee));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync(Deferred deferred, string scene)
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);
                yield return null;
                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(EScene eScene) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deferred, EScene scene) where T : SceneObject
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene<T>(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);

                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(string scene) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, scene));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deferred, string scene) where T : SceneObject
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);
                yield return null;
                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(EScene eScene, ISceneData data) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deferred, EScene scene, ISceneData data) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deferred.Resolve();

            this.Publish(new OnLoadSceneSignal(scene));
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(string scene, ISceneData data) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, scene, data));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deferred, string scene, ISceneData data) where T : SceneObject
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded. Data:{1}\n", scene, data);
                yield return null;
                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;

                PassDataToScene<T>(scene, data);
                deferred.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        public Promise UnloadScenePromise(EScene scene)
        {
            Deferred deferred = new Deferred();
            StartCoroutine(UnoadSceneAsync(deferred, scene));
            return deferred.Promise;
        }

        public IEnumerator UnoadSceneAsync(Deferred deferred, EScene scene)
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = Scene.IsLoaded(scene);
            //Assertion.Assert(sceneIsLoaded, D.ERROR + " Scene::UnloadScene Scene:{0} is not loaded.", scene);
            if (!sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::UnoadSceneAsync Scene:{0} is not loaded.\n", scene);
                yield return null;
                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.ToString());
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnUnloadSceneSignal(scene));
        }

        public Promise UnloadScenePromise(string sceneName)
        {
            Deferred deferred = new Deferred();
            StartCoroutine(UnoadSceneAsync(deferred, sceneName));
            return deferred.Promise;
        }

        public IEnumerator UnoadSceneAsync(Deferred deferred, string scene)
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            //Assertion.Assert(sceneIsLoaded, D.ERROR + " Scene::UnloadScene Scene:{0} is not loaded.", scene);
            if (!sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::UnoadSceneAsync Scene:{0} is not loaded.\n", scene);
                yield return null;
                deferred.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                yield return operation;
                deferred.Resolve();
            }

            this.Publish(new OnUnloadSceneSignal(scene));
        }
        
        public Promise EndFramePromise()
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.EndFrame(deferred));
            return deferred.Promise;
        }

        protected IEnumerator EndFrame(Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }

        public Promise WaitPromise(float seconds = 1.0f)
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.Wait(deferred, seconds));
            return deferred.Promise;
        }

        protected IEnumerator Wait(Deferred deferred, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            deferred.Resolve();
        }

        public void LoadScene(string scene)
        {
            bool sceneIsLoaded = IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadScene Scene:{0} is already loaded.\n", scene);
                return;
            }

            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
         //*/
    }
    #endregion
}