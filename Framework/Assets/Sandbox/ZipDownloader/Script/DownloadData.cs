using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.ZipDownloader
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "ZipDownloadData", menuName = "Zip/DownloadData", order = 1)]
    public class DownloadData : ScriptableObject
    {
#region Properties
        [SerializeField, ShowInInspector]
        private string _BundleName;
        public string BundleName
        {
            get { return _BundleName; }
            private set { _BundleName = value; }
        }
        
        [SerializeField, ShowInInspector]
        private string _DownloadUrl;
        public string DownloadUrl
        {
            get { return _DownloadUrl; }
            private set { _DownloadUrl = value; }
        }

        [SerializeField, ShowInInspector]
        private string _RootFolder;
        public string RootFolder
        {
            get { return _RootFolder; }
            private set { _RootFolder = value; }
        }

        [SerializeField, DisableContextMenu]
        private string _LocalFolderPath;
        public string LocalFolderPath
        {
            get { return _LocalFolderPath; }
            private set { _LocalFolderPath = value; }
        }
        
        [SerializeField, DisableContextMenu]
        private bool _Downloaded;
        public bool Downloaded
        {
            get { return _Downloaded; }
            private set { _Downloaded = value; }
        }

        [DisableContextMenu]
        public float DownloadProgress;

        /// <summary>
        /// Array of files included in zip file.
        /// </summary>
        [SerializeField, ShowInInspector]
        private List<string> _ZipContent;
        public List<string> ZipContent
        {
            get { return _ZipContent; }
            private set { _ZipContent = value; }
        }
#endregion

#region Download Functionalities
        public void ProcessDownload(byte[] bytes)
        {
            // fix local folder path
            LocalFolderPath = string.Format("{0}/{1}", Application.persistentDataPath, BundleName);
            
            // unzip to folder path
            ZipFile.UnZip(LocalFolderPath, bytes);

            Downloaded = true;
        }

        public Texture2D GetTextureFile(string file)
        {
            // load unzipped image file and assign it to the material's main texture.
            string filePath = string.Format("{0}/{1}/{2}", LocalFolderPath, RootFolder, file);
            return filePath.ToTexture();
        }
        #endregion

#region Editor buttons
        [Button(25)]
        public void ResetDownload()
        {
            LocalFolderPath = string.Empty;
            Downloaded = false;
            DownloadProgress = 0f;
        }
#endregion

#region UNITY_EDITOR
#if UNITY_EDITOR
        [MenuItem("Framework/Create/ZipDownloadData")]
        public static void CreateZipData()
        {
            const string zipDataFolder = "Assets/Sandbox/ZipSandbox/UAssets";
            const string zipData = "ZipData.asset";
            const string zipDataPath = zipDataFolder + "/" + zipData;
            string filePath = FixPath(zipDataPath, zipDataFolder, zipData, 0);

            if (File.Exists(zipData))
            {
                return;
            }

            if (!Directory.Exists(zipDataFolder))
            {
                Directory.CreateDirectory(zipDataFolder);
            }

            DownloadData downloadData = CreateInstance<DownloadData>();
            AssetDatabase.CreateAsset(downloadData, filePath);
        }

        private static string FixPath(string path, string folder, string file, int itr)
        {
            if (File.Exists(path))
            {
                string newPath = folder + "/" + itr + file;
                return FixPath(newPath, folder, file, ++itr);
            }

            Assertion.Assert(false, "Error");
            return string.Empty;
        }
#endif
#endregion
    }
}