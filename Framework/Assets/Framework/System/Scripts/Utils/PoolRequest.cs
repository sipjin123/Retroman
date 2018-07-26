using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Framework
{
    public class PoolRequest : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private PrefabManager PrefabPool;
        
        public T Request<T>(string key) where T : Component
        {
            GameObject obj = PrefabPool.Request(key);
            return obj.GetComponent<T>();
        }

        public T Request<T>(string key, Transform parent) where T : Component
        {
            GameObject obj = PrefabPool.Request(key, parent);
            return obj.GetComponent<T>();
        }
    }
}