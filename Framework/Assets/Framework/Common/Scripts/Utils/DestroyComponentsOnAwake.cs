using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Common.Utils
{
    public class DestroyComponentsOnAwake : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private bool WillDestroy = false;

        [SerializeField, ShowInInspector]
        private List<Component> Components;
        
        private void Awake()
        {
            if (WillDestroy)
            {
                Components.ForEach(c => Destroy(c));
                Components.Clear();
                Destroy(this);
            }
        }
    }
}