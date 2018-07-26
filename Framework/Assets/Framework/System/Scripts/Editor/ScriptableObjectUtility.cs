using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        string path = "Assets/GeneratedScriptableObject";
        string name = "New " + typeof(T).ToString();
        CreateAsset<T>(path, name);
    }

    public static void CreateAsset<T>(string path, string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        
        string guid = AssetDatabase.CreateFolder("Assets", "GeneratedScriptableObject");
        string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}