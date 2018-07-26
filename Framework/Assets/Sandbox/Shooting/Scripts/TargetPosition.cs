using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class TargetPosition : MonoBehaviour
    {
        [MinMaxSlider(0f, 1f)]
        public float ShootTarget = 0f;
        [MinMaxSlider(1f, 3f)]
        public float WidthScaler = 2f;
        public Transform RingCenter;
        public Transform RingTarget;
        public Transform Min;
        public Transform Max;
        public Camera Camera;

        private float RingCenterDistance = 0f;

        private void Update()
        {
            UpdateShotTarget();
        }

        private void LateUpdate()
        {
            CalculateCameraBounds();
        }

        public void UpdateShotTarget()
        {
            UpdateShotTarget(ShootTarget);
        }

        public void UpdateShotTarget(float shotTarget)
        {
            ShootTarget = shotTarget;
            Vector3 pos = Min.position + (Max.position - Min.position) * ShootTarget;
            //pos.z = RingCenter.position.z;
            //pos.y = RingCenter.position.y;
            RingTarget.position = FinalPosition(pos);
        }

        public Vector3 FinalPosition(Vector3 position)
        {
            Vector3 screenPos = Camera.WorldToScreenPoint(position);
            Vector3 worldPosN = Camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.nearClipPlane));
            Vector3 direction = (position - worldPosN).normalized;

            return worldPosN + direction * RingCenterDistance;
        }

        public void CalculateCameraBounds(Action<Vector3, Vector3> drawLine = null)
        {
            Vector3 screenPos = Camera.WorldToScreenPoint(RingCenter.position);
            Vector3 worldPosNear = Camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.nearClipPlane));
            RingCenterDistance = Vector3.Distance(RingCenter.position, worldPosNear);

            Func<Vector2, float, Vector3> DrawLine = delegate (Vector2 p, float d)
            {
                Ray r = Camera.ScreenPointToRay(p);
                Vector3 w = r.origin + r.direction.normalized * d;

                if (drawLine != null)
                {
                    drawLine(r.origin, w);
                }

                return w;
            };

            Vector3 left = Vector3.zero;
            Vector3 right = Vector3.zero;
            left.x = screenPos.x - (screenPos.x * WidthScaler);
            left.y = screenPos.y;
            left.z = screenPos.z;
            right.x = screenPos.x + (screenPos.x * WidthScaler);
            right.y = screenPos.y;
            right.z = screenPos.z;

            DrawLine(screenPos, RingCenterDistance);
            Min.position = DrawLine(left, RingCenterDistance);
            Max.position = DrawLine(right, RingCenterDistance);
        }
    }
}