using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

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

    public struct OnCacheSceneSignal
    {
        public Scene Scene;

        public OnCacheSceneSignal(Scene scene)
        {
            Scene = scene;
        }
    }

    public struct OnCacheSceneObjectSignal
    {
        public SceneObject Scene;

        public OnCacheSceneObjectSignal(SceneObject scene)
        {
            Scene = scene;
        }
    }

    public struct OnCleanSceneSignal
    {
        public Scene Scene;

        public OnCleanSceneSignal(Scene scene)
        {
            Scene = scene;
        }
    }

    public struct OnCleanSceneObjectSignal
    {
        public SceneObject Scene;

        public OnCleanSceneObjectSignal(SceneObject scene)
        {
            Scene = scene;
        }
    }

    public class SceneReference : MonoBehaviour
    {
        [SerializeField]
        private List<SceneEntry> Scenes = new List<SceneEntry>();

        private void Awake()
        {
            this.Receive<OnCacheSceneSignal>()
                .Subscribe(_ => Add(_.Scene))
                .AddTo(this);

            this.Receive<OnCacheSceneObjectSignal>()
                .Subscribe(_ => Add(_.Scene))
                .AddTo(this);

            this.Receive<OnCleanSceneSignal>()
                .Subscribe(_ => Remove(_.Scene))
                .AddTo(this);

            this.Receive<OnCleanSceneObjectSignal>()
                .Subscribe(_ => Remove(_.Scene))
                .AddTo(this);

            Factory.Register(this);
        }

        private void OnDestroy()
        {
            Factory.Clean<SceneReference>();
        }

        public bool Exists(EScene scene)
        {
            return Exists(scene.ToString());
        }

        public bool Exists(string scene)
        {
            return Scenes.Exists(s => s.Scene.Equals(scene));
        }

        public SceneEntry Find(EScene scene)
        {
            return Find(scene.ToString());
        }

        public SceneEntry Find(string scene)
        {
            return Scenes.FindAll(s => s.Scene.Equals(scene)).FirstOrDefault();
        }

        public void Add(Scene scene)
        {
            // Cache the Root scene object
            Scenes.Add(new SceneEntry()
            {
                Scene = scene.SceneType.ToString(),
                RootObject = scene.gameObject
            });

            Debug.LogFormat(D.F + "SceneReference::Add Scene:{0} Added to CachedScenes! Count:{1} RootObject:{2}\n", scene.SceneType, Scenes.Count, scene.gameObject.name);
        }

        public void Add(SceneObject scene)
        {
            // Cache the Root scene object
            Scenes.Add(new SceneEntry()
            {
                Scene = scene.Name,
                RootObject = scene.gameObject
            });

            Debug.LogFormat(D.F + "SceneReference::Add Scene:{0} Added to CachedScenes! Count:{1} RootObject:{2}\n", scene.Name, Scenes.Count, scene.gameObject.name);
        }

        public void Remove(Scene scene)
        {
            SceneEntry entry = null;
            string sceneName = scene.SceneType.ToString();
            foreach (SceneEntry e in Scenes)
            {
                if (e.Scene.Equals(sceneName))
                {
                    entry = e;
                    break;
                }
            }

            Scenes.Remove(entry);
            Debug.LogFormat(D.F + "SceneReference::Remove Scene:{0} Removed from CachedScenes! Count:{1} RootObject:{2}\n", scene.SceneType, Scenes.Count, scene.gameObject.name);
        }

        public void Remove(SceneObject scene)
        {
            SceneEntry entry = null;
            string sceneName = scene.Name;
            foreach (SceneEntry e in Scenes)
            {
                if (e.Scene.Equals(sceneName))
                {
                    entry = e;
                    break;
                }
            }

            Scenes.Remove(entry);
            Debug.LogFormat(D.F + "SceneReference::Remove Scene:{0} Removed from CachedScenes! Count:{1} RootObject:{2}\n", scene.Name, Scenes.Count, scene.gameObject.name);
        }
    }
}

