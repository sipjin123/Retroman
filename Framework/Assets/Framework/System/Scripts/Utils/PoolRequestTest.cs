using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Framework
{
    public class PoolRequestTest : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private PoolRequest PoolRequest;

        [SerializeField, ShowInInspector]
        private string Key;

        [SerializeField, ShowInInspector]
        private Transform Parent;

        [SerializeField, ShowInInspector]
        private List<GameObject> PoolItems;
        
        [Button(25)]
        public void Spawn()
        {
            PoolItems.Add(PoolRequest.Request<SwarmItem>(Key, Parent).gameObject);
        }

        [Button(25)]
        public void Kill()
        {
            PoolItems.FirstOrDefault().GetComponent<SwarmItem>().Kill();
            PoolItems.RemoveAt(0);
        }
    }
}