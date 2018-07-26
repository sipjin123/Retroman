using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

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
    using System.IO;
    using System.Linq;

    // alias
    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    #region Scene extension (Load, Unload, and Wait)

    public partial class Scene : MonoBehaviour
    {
        public static ValueDropdownList<string> SceneList;

        public static ValueDropdownList<string> GenerateSceneList()
        {
            List<string> scenes = new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkScenes.dat"));
            ValueDropdownList<string> dropdown = new ValueDropdownList<string>();
            scenes.ForEach(s => dropdown.Add(s));

            return dropdown;
        }

        public ValueDropdownList<string> Scenes()
        {
            SceneList = SceneList ?? GenerateSceneList();
            return SceneList;
        }

        private void UpdateSceneTypeString()
        {
            CachedScene = SelectedScene;
            SceneType = SelectedScene.ToEnum<EScene>();
        }
        
        [SerializeField]
        [HideInInspector]
        [TabGroup("Scene")]
        [DisableInEditorMode]
        private string CachedScene;
        
        public void PassDataToScene<T>(string scene, ISceneData data) where T : Scene
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<GameObject> scenes = objects.FindAll(g => g.GetComponent<T>() != null);
            List<T> items = scenes.ToArray<T>();

            Assertion.Assert(items.Count > 0, "Error! Scene:" + gameObject.name);

            // pass the data 
            items.FirstOrDefault().SceneData = data;
        }
        
        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene scene) where T : Scene
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(def, scene));
            return def.Promise;
        }

        /// <summary>
        /// Unloads everything except the SystemRoot then loads the target scene.
        /// </summary>
        public IEnumerator LoadSceneAsync<T>(Deferred def, EScene scene) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadSceneAsync Scene:{0} is already loaded.\n", scene);

                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                def.Resolve();
            }
            else
            {
                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;
                def.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene, true));
        }

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(string scene) where T : SceneObject
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(def, scene));
            return def.Promise;
        }

        /// <summary>
        /// Unloads everything except the SystemRoot then loads the target scene.
        /// </summary>
        public IEnumerator LoadSceneAsync<T>(Deferred def, string scene) where T : SceneObject
        {
            bool sceneIsLoaded = IsLoaded(scene) || HasScene(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadSceneAsync Scene:{0} is already loaded.\n", scene);

                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                def.Resolve();
            }
            else
            {
                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());

                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;
                def.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene, true));
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene scene, ISceneData data) where T : Scene
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(def, scene, data));
            return def.Promise;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred def, EScene scene, ISceneData data) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadSceneAsync Scene:{0} is already loaded. Data:{1}\n", scene, data);

                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                def.Resolve();
            }
            else
            {
                // Unload all other scenes except flagged as persistent
                yield return StartCoroutine(UnloadAllScenes());
                
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;

                PassDataToScene<T>(scene.ToString(), data);
                def.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene, true));
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sScenee"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise(string scene)
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync(def, scene));
            return def.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync(Deferred def, string scene)
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);
                yield return null;
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;
                def.Resolve();
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
            Deferred def = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(def, eScene));
            return def.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred def, EScene scene) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = IsLoaded(scene) || HasScene(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                yield return operation;
                def.Resolve();
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
            Deferred def = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(def, scene));
            return def.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred def, string scene) where T : Scene
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded.\n", scene);
                yield return null;
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;
                def.Resolve();
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
        public Promise LoadSceneAdditivePromise<T>(EScene eScene, ISceneData data) where T : Scene
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(def, eScene, data));
            return def.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred def, EScene scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            def.Resolve();

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
        public Promise LoadSceneAdditivePromise<T>(string scene, ISceneData data) where T : Scene
        {
            Deferred def = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(def, scene, data));
            return def.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred def, string scene, ISceneData data) where T : Scene
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            if (sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::LoadAdditiveSceneAsync Scene:{0} is already loaded. Data:{1}\n", scene, data);
                yield return null;
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return operation;

                PassDataToScene<T>(scene, data);
                def.Resolve();
            }

            this.Publish(new OnLoadSceneSignal(scene));
        }

        public Promise UnloadScenePromise(EScene scene)
        {
            Deferred def = new Deferred();
            StartCoroutine(UnoadSceneAsync(def, scene));
            return def.Promise;
        }

        public IEnumerator UnoadSceneAsync(Deferred def, EScene scene)
        {
            Assertion.Assert(scene != EScene.System);

            bool sceneIsLoaded = Scene.IsLoaded(scene);
            //Assertion.Assert(sceneIsLoaded, D.ERROR + " Scene::UnloadScene Scene:{0} is not loaded.", scene);
            if (!sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::UnoadSceneAsync Scene:{0} is not loaded.\n", scene);
                yield return null;
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.ToString());
                yield return operation;
                def.Resolve();
            }

            this.Publish(new OnUnloadSceneSignal(scene));
        }

        public Promise UnloadScenePromise(string sceneName)
        {
            Deferred def = new Deferred();
            StartCoroutine(UnoadSceneAsync(def, sceneName));
            return def.Promise;
        }

        public IEnumerator UnoadSceneAsync(Deferred def, string scene)
        {
            bool sceneIsLoaded = Scene.IsLoaded(scene);
            //Assertion.Assert(sceneIsLoaded, D.ERROR + " Scene::UnloadScene Scene:{0} is not loaded.", scene);
            if (!sceneIsLoaded)
            {
                Debug.LogFormat(D.ERROR + " Scene::UnoadSceneAsync Scene:{0} is not loaded.\n", scene);
                yield return null;
                def.Resolve();
            }
            else
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                yield return operation;
                def.Resolve();
            }

            this.Publish(new OnUnloadSceneSignal(scene));
        }

        public IEnumerator UnloadAllScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            UScene[] scenes = new UScene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = SceneManager.GetSceneAt(i);
            }

            foreach (UScene scene in scenes)
            {
                AsyncOperation operation = null;
                string sceneName = scene.name;
                bool unload = false;
                bool isEnum = sceneName.IsEnum<EScene>();
                EScene loadedScene = !isEnum ? EScene.Invalid : sceneName.ToEnum<EScene>();

                do
                {
                    // If not loaded. stop
                    if (!IsLoaded(sceneName))
                    {
                        break;
                    }

                    // If the loaded scene is SystemRoot. stop
                    if (loadedScene == EScene.System)
                    {
                        break;
                    }

                    // If the loaded scene is persistent. stop
                    if (isEnum && GetScene<Scene>(loadedScene).IsPersistent)
                    {
                        break;
                    }

                    Debug.LogFormat(D.L("[FRAMEWORK]") + " Scene::UnloadAllScenes Unloading Scene:{0}\n", sceneName);

                    operation = SceneManager.UnloadSceneAsync(sceneName);
                    unload = true;
                    break;
                } while (true);

                if (unload)
                {
                    yield return operation;

                    this.Publish(new OnUnloadSceneSignal(loadedScene, sceneName));
                }

                yield return null;
            }
        }

        public Promise EndFramePromise()
        {
            Deferred def = new Deferred();
            StartCoroutine(EndFrame(def));
            return def.Promise;
        }

        protected IEnumerator EndFrame(Deferred def)
        {
            yield return null;
            def.Resolve();
        }

        public Promise WaitPromise(float seconds = 1.0f)
        {
            Deferred def = new Deferred();
            StartCoroutine(Wait(def, seconds));
            return def.Promise;
        }

        protected IEnumerator Wait(Deferred def, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            def.Resolve();
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

        public T GetSceneRoot<T>(EScene scene) where T : Scene
        {
            return Scene.GetScene<T>(scene);
        }
        
        public T GetSceneRoot<T>(string scene) where T : Scene
        {
            return Scene.GetScene<T>(scene);
        }
    }
    #endregion
}