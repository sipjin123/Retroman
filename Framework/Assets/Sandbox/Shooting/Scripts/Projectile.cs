using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Sandbox.Shooting
{
    using URandom = UnityEngine.Random;

    public enum ShootKey
    {
        SPACE = KeyCode.Space,
        Q = KeyCode.Q,
        E = KeyCode.E,
        R = KeyCode.R,
        T = KeyCode.T,
    }

    public class Projectile : MonoBehaviour
    {
        [TabGroup("Projectile")]
        public Transform Source;

        [TabGroup("Projectile")]
        public Transform Target;
        
        [TabGroup("Projectile")]
        [MinMaxSlider(0f, 10f)]
        public float Duration;

        [TabGroup("Projectile")]
        [MinMaxSlider(0f, 10f)]
        public float DurationScaler;

        [TabGroup("Projectile")]
        [MinMaxSlider(0f, 10f)]
        public float GravityScale;
        
        [TabGroup("Projectile")]
        public bool UseGravity = false;

        [TabGroup("Projectile")]
        public bool ModifyEulerAngles = false;

        [TabGroup("Projectile")]
        public bool UseAngularVelocity = false;

        [DisableContextMenu]
        [TabGroup("Projectile")]
        [SerializeField, ShowInInspector]
        private bool IsThrown = false;

        [TabGroup("Debug")]
        public ShootKey ShotKey;

        private Rigidbody Rigidbody;
        private static float CachedDistance = -1f;
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Stop();
        }

        private void Start()
        {
            if (CachedDistance <= -1f)
            {
                CachedDistance = Vector3.Distance(transform.position, Target.position);
            }
        }

        private void FixedUpdate()
        {
            /*
            if (Input.GetKey(KeyCode.S))
            {
                Stop();
            }

            if (Input.GetKey((KeyCode)ShotKey))
            {
                Throw();
            }
            //*/

            if (IsThrown && !Rigidbody.useGravity)
            {
                Rigidbody.AddForce(Physics.gravity * GravityScale, ForceMode.Force);
            }

            if (!IsThrown)
            {
                DurationScaler = Vector3.Distance(transform.position, Target.position) / CachedDistance;
            }
        }

        [TabGroup("Projectile")]
        [Button(25)]
        public Vector3 Throw()
        {
            Stop();
            
            return Throw(Target.position - Source.position);
        }

        public Vector3 Throw(Vector3 direction)
        {
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.useGravity = UseGravity;

            if (ModifyEulerAngles)
            {
                Rigidbody.transform.eulerAngles = new Vector3(0f, 0f, URandom.Range(0, 360f));
            }

            if (UseAngularVelocity)
            {
                Rigidbody.angularVelocity = Vector3.one * URandom.Range(-360f, 360f);
            }

            Vector3 velocity = GetVelocity(direction, Duration, Physics.gravity * GravityScale);
            Rigidbody.velocity = velocity;

            IsThrown = true;
            
            return velocity;
        }

        [TabGroup("Projectile")]
        [Button(25)]
        public void Stop()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.position = Source.position;
            Rigidbody.useGravity = false;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;

            transform.position = Source.position;

            IsThrown = false;
        }

        /// <summary>
        /// Returns the pre-calculated velocity
        /// </summary>
        /// <returns></returns>
        public Vector3 GetVelocity()
        {
            return GetVelocity(Target.position, Source.position, Duration, Physics.gravity * GravityScale);
        }

        public Vector3 GetVelocity(Vector3 target, Vector3 start, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return ((target - start) / duration) - 0.5f * gravity * duration;
        }

        public Vector3 GetVelocity(Vector3 direction, float magnitude, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return ((direction.normalized * magnitude) / duration) - 0.5f * gravity * duration;
        }

        public Vector3 GetVelocity(Vector3 direction, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return (direction / duration) - 0.5f * gravity * duration;
        }

        public float TimeOfFlight(float timeOfFlight)
        {
            // Ignore TimeOfFlight scaler for now
            //return Mathf.Max(timeOfFlight, timeOfFlight * DurationScaler);
            return timeOfFlight;
        }

        public void AdjustDuration(float scale)
        {
            Duration = 1.25f * scale;
        }
    }
}