using System;
using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class LookAt : MonoBehaviour
    {
        public Transform Target;
        public bool LockX;
        public bool LockY;
        public bool LockZ;

        private void Update()
        {
            if (!LockX && !LockY && !LockZ)
            {
                return;
            }

            Vector3 rotation = transform.localRotation.eulerAngles;
            transform.LookAt(Target);

            Vector3 newRotation = transform.localRotation.eulerAngles;

            if (LockX)
            {
                newRotation.x = rotation.x;
            }

            if (LockY)
            {
                newRotation.y = rotation.y;
            }

            if (LockZ)
            {
                newRotation.z = rotation.z;
            }

            transform.localRotation = Quaternion.Euler(newRotation);
        }
    }
}