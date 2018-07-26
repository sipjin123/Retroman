using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Sirenix.OdinInspector;

using MiniJSON;

using Common;

using Framework;

namespace Sandbox.Build
{
    public class BuildProfile : ScriptableObject
    {
        [SerializeField, ShowInInspector]
        public string Company;

        [SerializeField, ShowInInspector]
        public string AppName;

        [SerializeField, ShowInInspector]
        public string Package;

        [SerializeField, ShowInInspector]
        public string KeyStore;

        [SerializeField, ShowInInspector]
        public string Alias;

        [SerializeField, ShowInInspector]
        public string Password;

#if UNITY_EDITOR
        public SceneAsset SceneAsset;
#endif

        [SerializeField, ShowInInspector]
        public List<string> AssetbundleScenes = new List<string>()
        {
            "Synergy88/Game/Scenes/ParesoMami",
            "Wowowillie/BigyanNgJacket/Scenes/BigyanNgJacket",
            "Sandbox/SpinTheWheelSandbox/Scenes/SpintheWheel",
        };

#if UNITY_EDITOR
        [Button(25)]
        public void BrowseKeystore()
        {
            //string path = EditorUtility.OpenFolderPanel("Load Keystore", string.Empty, string.Empty);
            //Debug.LogErrorFormat("Path:{0}\n", path);

            KeyStore = EditorUtility.OpenFilePanel("Load Keystore", string.Empty, string.Empty);
            Debug.LogErrorFormat("Path:{0}\n", KeyStore);
        }
        
        public void Setup()
        {
            PlayerSettings.companyName = Company;
            PlayerSettings.productName = AppName;
            PlayerSettings.applicationIdentifier = Package;
            PlayerSettings.Android.keystoreName = KeyStore;
            PlayerSettings.Android.keystorePass = Password;
            PlayerSettings.Android.keyaliasName = Alias;
            PlayerSettings.Android.keyaliasPass = Password;
        }

        [Button(25)]
        public void SetupAndroid()
        {
            Setup();

            ModifyBuildSettings(AssetbundleScenes, true);
        }
        
        private void ModifyBuildSettings(List<string> bundles, bool isEnabled)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.ForEach(s =>
            {
                //Debug.LogErrorFormat();
            });
            //bundles.ForEach(b => scenes.Find(s => s.path.Contains(b)).enabled = false);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        /*
        [Button(25)]
        public void BuildAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, options);
        }
        //*/

        [MenuItem("Framework/Create/DownloadProfile", false, 1011)]
        public static void CreateDownloadProfile()
        {
            const string dataFolder = "Assets/Sandbox/Build/UAssets";
            const string dataName = "BuildProfile.asset";
            const string dataPath = dataFolder + "/" + dataName;
            string filePath = FixPath(dataPath, dataFolder, dataName, 0);

            if (File.Exists(dataName))
            {
                return;
            }

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            BuildProfile data = CreateInstance<BuildProfile>();
            AssetDatabase.CreateAsset(data, filePath);
        }

        private static string FixPath(string path, string folder, string file, int itr)
        {
            if (File.Exists(path))
            {
                string newPath = folder + "/" + itr + file;
                return FixPath(newPath, folder, file, ++itr);
            }
            
            return path;
        }
#endif
    }
}
