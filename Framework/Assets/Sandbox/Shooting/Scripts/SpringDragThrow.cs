using System;
using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class SpringDragThrow : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private Camera Camera;

        [SerializeField, ShowInInspector]
        private Transform Target;

        private int normalCollisionCount = 1;
        private float spring = 50.0f;
        private float damper = 5.0f;
        private float drag = 10.0f;
        private float angularDrag = 5.0f;
        private float distance = 0.2f;
        private float throwForce = 500;
        private float throwRange = 1000;
        private bool attachToCenterOfMass = false;

        private SpringJoint springJoint;
 
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.angularDrag = 0f;
                rb.position = Vector3.zero;
            }

            // Make sure the user pressed the mouse down
            if (!Input.GetMouseButtonDown(0))
                return;

            Camera mainCamera = FindCamera();

            // We need to actually hit an object
            RaycastHit hit;
            if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                return;
            // We need to hit a rigidbody that is not kinematic
            if (!hit.rigidbody || hit.rigidbody.isKinematic)
                return;

            // Enable gravity
            hit.rigidbody.useGravity = true;

            if (!springJoint)
            {
                GameObject go = new GameObject("Rigidbody dragger");
                Rigidbody body = go.AddComponent<Rigidbody>();
                springJoint = go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            springJoint.transform.position = hit.point;
            if (attachToCenterOfMass)
            {
                var anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
                anchor = springJoint.transform.InverseTransformPoint(anchor);
                springJoint.anchor = anchor;
            }
            else
            {
                springJoint.anchor = Vector3.zero;
            }

            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = distance;
            springJoint.connectedBody = hit.rigidbody;

            //StartCoroutine("DragObject", hit.distance);
            StartCoroutine(DragObject(hit.distance));
        }

        private IEnumerator DragObject(float distance)
        {
            float oldDrag = springJoint.connectedBody.drag;
            float oldAngularDrag = springJoint.connectedBody.angularDrag;

            springJoint.connectedBody.drag = drag;
            springJoint.connectedBody.angularDrag = angularDrag;

            Camera mainCamera = FindCamera();

            while (Input.GetMouseButton(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                springJoint.transform.position = ray.GetPoint(distance);
                yield return null;

                if (Input.GetMouseButton(1))
                {
                    springJoint.connectedBody.AddExplosionForce(throwForce, mainCamera.transform.position, throwRange);
                    
                    springJoint.connectedBody.drag = oldDrag;
                    springJoint.connectedBody.angularDrag = oldAngularDrag;
                    springJoint.connectedBody = null;

                    //Vector3 force = (Target.position - transform.position).normalized * throwForce;
                    //Vector3 force = Target.position - mainCamera.transform.position;
                    Vector3 force = Target.position - transform.position;
                    springJoint.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                }
            }
            if (springJoint.connectedBody)
            {
                springJoint.connectedBody.drag = oldDrag;
                springJoint.connectedBody.angularDrag = oldAngularDrag;
                springJoint.connectedBody = null;
            }
        }

        private Camera FindCamera()
        {
            return Camera;
        }
    }
}