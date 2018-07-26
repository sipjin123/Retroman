using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.Shooting
{
    public struct OnScoreSignal
    {
        public int Score { get; set; }
    }

    public class ScoreCalculator : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private Dictionary<GameObject, bool> Colliders;
        
        public void CheckShoot(GameObject ring)
        {
            if (!Colliders.ContainsKey(ring))
            {
                Colliders.Add(ring, true);
            }

            if (Colliders.Count >= 2)
            {
                this.Publish(new OnScoreSignal() { Score = 1 });
            }
        }
    }
}