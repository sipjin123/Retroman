using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditor.SceneManagement;

namespace Sandbox.ButtonAnimation
{
    public class ButtonTool
    {
        [MenuItem("Framework/Button/Add Animation/Shrink")]
        private static void Shrink()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_001_Button_Animation_Shrink.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Shrink Animation to " + Selection.activeObject);
            }

        }

        [MenuItem("Framework/Button/Add Animation/Wobble A")]
        private static void WobbleA()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_002_Button_Animation_Wobble_A.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Wobble A Animation to " + Selection.activeObject);
            }
        }

        [MenuItem("Framework/Button/Add Animation/Wobble B")]
        private static void WobbleB()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_003_Button_Animation_Wobble_B.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Wobble B Animation to " + Selection.activeObject);
            }
        }

        [MenuItem("Framework/Button/Add Animation/Squash")]
        private static void Squash()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_004_Button_Animation_Squash.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Squash Animation to " + Selection.activeObject);
            }
        }

        [MenuItem("Framework/Button/Add Animation/Spin")]
        private static void Spin()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_005_Button_Animation_Spin.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Spin Animation to " + Selection.activeObject);
            }
        }

        [MenuItem("Framework/Button/Add Animation/Pulse")]
        private static void Pulse()
        {
            GameObject Selected = Selection.activeGameObject;
            if (CheckButton())
            {
                RuntimeAnimatorController anim = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Sandbox/ButtonsSandbox/Animations/_006_Button_Animation_Pulse.controller", typeof(RuntimeAnimatorController));
                if (Selected.transform.GetComponent<Animator>() == null)
                {
                    Selected.AddComponent(typeof(Animator));
                }
                Selected.transform.GetComponent<Button>().transition = Selectable.Transition.Animation;
                Selected.transform.GetComponent<Animator>().runtimeAnimatorController = anim;
                Debug.Log("Added Pulse Animation to " + Selection.activeObject);
            }
        }

        public static bool CheckButton()
        {
            GameObject Selected = Selection.activeGameObject;
            RectTransform rT = Selection.activeGameObject.GetComponent<RectTransform>();
            float temp = 0.5f;
            float x, y;

            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
            }

            if (Selected.transform.GetComponent<Button>() == null
                || Selected.transform.GetComponent<RectTransform>() == null
            )
            {
                Debug.LogWarning("Selected object is not a UI button");
                return false;
            }
            else
            {
                x = ((temp - rT.pivot.x) * rT.sizeDelta.x) + rT.localPosition.x;
                y = ((temp - rT.pivot.y) * rT.sizeDelta.y) + rT.localPosition.y;
                rT.localPosition = new Vector3(x, y, rT.localPosition.z);
                rT.pivot = new Vector2(temp, temp);
                return true;
            }
        }

    }
}