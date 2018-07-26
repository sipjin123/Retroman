using System;
using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class SpringJointDrag : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private Camera Camera;

        [SerializeField, ShowInInspector]
        private SpringJoint Spring;
        
        private Vector3 ScreenPoint;
        private Vector3 Position;

        const float SPRING = 50.0f;
        const float DAMP = 5.0f;
        const float DRAG = 10.0f;
        const float ANGULAR_DRAG = 5.0f;
        const float DISTANCE = 0.2f;

        private void Awake()
        {
            Spring.connectedAnchor = transform.position;
            Spring.anchor = Vector3.zero;
        }
        
        private void OnMouseDown()
        {
            CalculatePosition();

            Spring.enablePreprocessing = true;

            Spring.transform.position = transform.position;
            Spring.anchor = Vector3.zero;
            Spring.spring = SPRING;
            Spring.damper = DAMP;
            Spring.maxDistance = DISTANCE;
            Spring.connectedBody = GetComponent<Rigidbody>();
        }

        private void OnMouseDrag()
        {
            if (Spring.enablePreprocessing == true)
            {
                Spring.connectedAnchor = CalculateDrag();
                Spring.connectedBody.drag = DRAG;
                Spring.connectedBody.angularDrag = ANGULAR_DRAG;
            }
        }

        private void OnMouseUp()
        {
            CalculatePosition();

            Spring.enablePreprocessing = false;
            Spring.connectedBody = null;
        }

        private void CalculatePosition()
        {
            Position = transform.position;
            ScreenPoint = Camera.WorldToScreenPoint(Position);
        }

        private Vector3 CalculateDrag()
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Position.z);
            Vector3 pos = Camera.ScreenToWorldPoint(screenPos);
            
            return pos;
        }
    }
}
