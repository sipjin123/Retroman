using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    [CustomEditor(typeof(CameraGizmo))]
    public class CameraGizmoEditor : Editor
    {
        private void OnSceneGUI()
        {
            CameraGizmo gizmo = target as CameraGizmo;
            TargetPosition targetPos = gizmo.GetComponent<TargetPosition>();
            targetPos.CalculateCameraBounds(Handles.DrawLine);
        }
    }
}