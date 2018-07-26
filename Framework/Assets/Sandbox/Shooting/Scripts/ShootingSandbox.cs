using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using uPromise;

using Common;
using Common.Query;
using Common.Utils;

using Framework;

namespace Sandbox.Shooting
{
    public struct VectorAccuracy
    {
        public float Angle;
        public float Accuracy;
        public float Magnitude;
    }

    public class ShootingSandbox : SceneObject
    {
        public const float CENTER_RING = 0.5f;

        public TargetPosition TargetPosition;
        public Rigidbody RingRef;
        public Rigidbody BallRef;
        public PrefabManager Balls;
        public Text Score;

        [Range(0f, 2f)]
        public float VelocityScaler = 0.25f;

        private Projectile Ball;
        private int ScoreValue = 0;

        public AnimationCurve SwipeCurve;

        /*
        [SerializeField]
        [DisableInEditorMode]
        private string _SceneTypeString;
        public string SceneTypeString
        {
            get { return _SceneTypeString; }
            private set { _SceneTypeString = value; }
        }

        private void UpdateSceneTypeString()
        {
            SceneTypeString = SceneType.ToString();
        }
        
        [ValueDropdown("Scenes")]
        [OnValueChanged("UpdateSceneTypeString")]
        public EScene SceneType;

        public static ValueDropdownList<EScene> SceneList;
        public ValueDropdownList<EScene> Scenes()
        {
            SceneList = SceneList ?? GenerateSceneList();
            return SceneList;
        }

        public static ValueDropdownList<EScene> GenerateSceneList()
        {
            List<string> scenes = new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkScenes.dat"));
            ValueDropdownList<EScene> dropdown = new ValueDropdownList<EScene>();
            scenes.ForEach(s => dropdown.Add(s, s.ToEnum<EScene>()));

            return dropdown;
        }
        
        [SerializeField]
        private SceneOptions SceneOptions;
        //*/

        protected override void Awake()
        {
            base.Awake();

            TKSwipeRecognizer recognizer = new TKSwipeRecognizer();
            recognizer.gestureRecognizedEvent += (r) =>
            {
                VectorAccuracy result = CheckAccuracy(r.endPoint - r.startPoint);
                Debug.LogErrorFormat("Accuracy:{0} Angle:{1} Velocity:{2}\n", result.Accuracy, result.Angle, r.swipeVelocity);
                
                BallRef.GetComponent<Projectile>().AdjustDuration(Mathf.Clamp01(r.swipeVelocity / 50f));
                Ball.AdjustDuration(Mathf.Clamp01(r.swipeVelocity / 50f));

                TargetPosition.UpdateShotTarget(1f - (result.Angle / 180f));
                Throw(r.swipeVelocity);
            };

            TouchKit.addGestureRecognizer(recognizer);

            this.Receive<OnScoreSignal>()
                .Subscribe(_ =>
                {
                    ScoreValue += _.Score;
                    Score.text = string.Format("{0}", ScoreValue);
                })
                .AddTo(this);

            BallRef.gameObject.SetActive(false);
            Ball = GetBall();
        }
        
        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Throw();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        [Button(25)]
        public void Throw()
        {
            TargetPosition.UpdateShotTarget(CENTER_RING);
            BallRef.GetComponent<Projectile>().AdjustDuration(1f);
            Ball.AdjustDuration(1f);

            Debug.LogErrorFormat("Accuracy:{0} Angle:{1} Velocity:{2}\n", 1f, 90f, Ball.GetVelocity().magnitude);
            
            Ball.Throw();

            // Enable collider
            Ball.GetComponent<Collider>().enabled = true;
            Ball.GetComponent<SwarmItem>().minimumLifeSpan = 10f;
            
            // Create new ball
            Ball = GetBall();
        }

        public void Throw(float swipeVelocity)
        {
            Vector3 velocity = (RingRef.transform.position - Ball.transform.position).normalized * swipeVelocity * VelocityScaler;
            //Vector3 velocity = Ball.GetVelocity().normalized * swipeVelocity * VelocityScaler;
            velocity = Ball.Throw(velocity);

            // Enable collider
            Ball.GetComponent<Collider>().enabled = true;
            Ball.GetComponent<SwarmItem>().minimumLifeSpan = 10f;

            //Debug.LogErrorFormat("Velocity:{0}\n", velocity.magnitude);

            // Create new ball
            Ball = GetBall();
        }

        public Projectile GetBall()
        {
            Rigidbody projectile = Balls.Request("Ball").GetComponent<Rigidbody>();
            projectile.position = Vector3.zero;
            projectile.useGravity = false;
            projectile.velocity = Vector3.zero;
            projectile.angularVelocity = Vector3.zero;
            projectile.gameObject.SetActive(true);

            //projectile.transform.localPosition = BallRef.transform.localPosition;
            projectile.transform.position = BallRef.transform.position;
            projectile.transform.localScale = BallRef.transform.localScale;
            projectile.transform.localRotation = BallRef.transform.localRotation;
            projectile.GetComponent<Collider>().enabled = false;

            return projectile.GetComponent<Projectile>();
        }

        public VectorAccuracy CheckAccuracy(Vector2 direction)
        {
            VectorAccuracy result;
            result.Accuracy = Vector3.Dot(Vector2.up, direction.normalized);
            //result.Angle = Vector3.Angle(direction, Vector2.up);
            result.Angle = Vector3.Angle(direction, Vector2.right);
            result.Magnitude = direction.magnitude;

            return result;
        }
    }
}
