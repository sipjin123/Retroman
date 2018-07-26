using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Common.Utils
{
    public class DestroyOnAwake : MonoBehaviour
    {
        [SerializeField]
        private bool WillDestroy = true;

        private void Awake()
        {
            if (WillDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}