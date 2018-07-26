using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class RingTargetTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            ScoreCalculator score = other.GetComponent<ScoreCalculator>();

            if (score != null)
            {
                score.CheckShoot(gameObject);
            }
        }
    }
}