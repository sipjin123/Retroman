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
        }
        
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
