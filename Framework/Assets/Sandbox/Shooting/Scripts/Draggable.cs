using System;
using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class Draggable : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private Camera Camera;
        
        private Vector3 ScreenPoint;
        private Vector3 Position;
        private Vector3 TargetPosition;
        private bool IsDragging;

        private void Awake()
        {
            IsDragging = false;
        }

        private void Update()
        {
            if (IsDragging)
            {
                transform.position = TargetPosition;
            }
        }

        private void OnMouseDown()
        {
            CalculatePosition();
            CalculateDrag();
            IsDragging = true;
        }

        private void OnMouseDrag()
        {
            CalculateDrag();
            IsDragging = true;
        }

        private void OnMouseUp()
        {
            CalculateDrag();
            IsDragging = false;
        }

        private void CalculatePosition()
        {
            Position = transform.position;
            ScreenPoint = Camera.WorldToScreenPoint(Position);
        }

        private void CalculateDrag()
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Position.z);
            TargetPosition = Camera.ScreenToWorldPoint(screenPos);
        }
    }
}
