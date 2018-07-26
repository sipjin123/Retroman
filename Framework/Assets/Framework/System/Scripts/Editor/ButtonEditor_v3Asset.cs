using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonEditor_v3Asset
{
    //CREATES A SCRIPTABLE OBJECT THAT HANDLES THE LIST OF BUTTONS BEING USED THROUGH OUT THE GAME
    [MenuItem("Framework/Create/ButtonEditor")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<ButtonEditor_v3>("Assets/Framework/System/UAssets", "ButtonEditor");
    }
}