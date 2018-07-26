using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Sirenix.OdinInspector;

using uPromise;

using Common;
using Common.Query;
using Common.Signal;

namespace Framework
{
    using UniRx;
    using UniRx.Triggers;

    public interface IBroker
    {

    }

    public interface IScriptableData
    {

    }
    
    public static class SceneExtensions
    {
        public static void Publish<T>(this Scene scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this Scene scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

        public static void Publish<T>(this MonoBehaviour scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this MonoBehaviour scene)
        {
            return MessageBroker.Default.Receive<T>();
        }
        
        public static void Publish<T>(this ScriptableObject scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this ScriptableObject scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

        public static void Publish<T>(this IBroker scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this IBroker scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

        public static void CreateAsset<T>(this IScriptableData scriptable, string filePath, string fileName) where T : ScriptableObject
        {
            if (File.Exists(fileName))
            {
                return;
            }

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            T d = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(d, filePath);
#endif
        }

        public static string FixPath(this IScriptableData data, string path, string folder, string file, int itr)
        {
            if (File.Exists(path))
            {
                string newPath = folder + "/" + itr + file;
                return data.FixPath(newPath, folder, file, ++itr);
            }

            Assertion.Assert(false, "Error");
            return string.Empty;
        }
    }
}

