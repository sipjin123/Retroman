using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// alias
using UDebug = UnityEngine.Debug;

namespace Framework
{
    [InitializeOnLoad]
    public static class FrameworkEditor
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // CONSTANTS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public const string FRAMEWORK_ROOT = "Framework/";
        public const string FRAMEWORK_TOOLS = "Tools/";
        public const string FRAMEWORK_BUILD = "Build/";

        // scenes
        public const string SYSTEM_SCENE = "Assets/Framework/System/Scenes/System.unity";
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // BUILD
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // cmd/alt + shift + r = &#r
        // ctr + shift + r = %#r
        //[MenuItem(FRAMEWORK_ROOT + FRAMEWORK_DEBUG + "Aries/Run MainGame %#r", false)]
        // alt + shift + r
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_BUILD + "Run System &#r", false)]
        public static void RunSystem()
        {
            Recompile();

            // Apply your debug settings here
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(SYSTEM_SCENE);
            EditorApplication.isPlaying = true;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // DEBUG
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_BUILD + "Recompile", false)]
        public static void Recompile()
        {
            // Apply your debug settings here
            AssetDatabase.StartAssetEditing();
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in allAssetPaths)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
                if (script != null)
                {
                    AssetDatabase.ImportAsset(assetPath);
                    break;
                }
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

#if UNITY_ANDROID
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_BUILD + "Build (Android)", false)]
#elif UNITY_IOS
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_BUILD + "Build (iOS)", false)]
#else
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_BUILD + "Build (Invalid Platform)", false)]
#endif
        public static void Build()
        {
#if UNITY_ANDROID
            string keyStoreName = "D:/Projects/Framework/Files/Frameworkframework.keystore";
            string keyAliasName = "com.Frameworkdigital.framework";
            string password = "88Framework";

            PlayerSettings.Android.keystoreName = keyStoreName;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = keyAliasName;
            PlayerSettings.Android.keyaliasPass = password;

            /*
            string path = "D:/Projects/Builds/Barangay143/Android";

            BuildOptions options = BuildOptions.None;
            options |= BuildOptions.AllowDebugging;
            options |= BuildOptions.ConnectWithProfiler;
            options |= BuildOptions.Development;

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, options);
            //*/
#elif UNITY_IOS
            /*
            BuildOptions options = BuildOptions.None;
            options |= BuildOptions.AllowDebugging;
            options |= BuildOptions.ConnectWithProfiler;
            options |= BuildOptions.Development;

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		    BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.iOS, options);
            //*/
#endif
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // TOOLS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // cmd/alt + shift + d = &#d
        // ctr + shift + d = %#d
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_TOOLS + "Clear Prefs %#d", false)]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();

            if (Directory.Exists(Application.persistentDataPath))
            {
                Directory.Delete(Application.persistentDataPath, true);
            }
            
        }
        
        // cmd + shift + x
        [MenuItem(FRAMEWORK_ROOT + FRAMEWORK_TOOLS + "Clear Logs %#x", false)]
        public static void ClearLogs()
        {
            var logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
        
    }

}