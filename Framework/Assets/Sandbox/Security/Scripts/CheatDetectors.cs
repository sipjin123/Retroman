using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;

namespace Sandbox.Security
{
    using CodeStage.AntiCheat.Detectors;
    using CodeStage.AntiCheat.ObscuredTypes;
    using Sandbox.Services;

    public struct OnCheaterDetectedSignal
    {

    }

    public class CheatDetectors : BaseService
    {
        #region Services
        public override void InitializeService()
        {
            InjectionDetector.StartDetection(OnCheaterDetected);
            ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
            SpeedHackDetector.StartDetection(OnCheaterDetected);
            WallHackDetector.StartDetection(OnCheaterDetected);
            TimeCheatingDetector.StartDetection(OnCheaterDetected);
            ObscuredPrefs.onAlterationDetected = OnCheaterDetected;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
        }
        #endregion

        public void OnCheaterDetected(string info)
        {
            Debug.LogErrorFormat(D.WARNING + "CheatDetectors::OnCheaterDetected Info:{0}\n", info);
            this.Publish(new OnCheaterDetectedSignal());
        }

        public void OnCheaterDetected()
        {
            Debug.LogErrorFormat(D.WARNING + "CheatDetectors::OnCheaterDetected\n");
            this.Publish(new OnCheaterDetectedSignal());
        }
    }
}