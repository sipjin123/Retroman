using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneDepthEditor_v3Asset : MonoBehaviour {

    //CREATES A SCRIPTABLE OBJECT THAT HANDLES THE LIST OF SCENES BEING USED THROUGH OUT THE GAME
    [MenuItem("Assets/Create/SceneDepthEditor_v3")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<SceneDepthEditor_V3>();
    }
}
