using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Sirenix.OdinInspector;

using MiniJSON;

using Common;

using Framework;

namespace Sandbox.Downloader
{
    [Serializable]
    public class DownloadProfile : IJson
    {
        public string ImageId = string.Empty;
        
        public string Timestamp = string.Empty;

        public string LocalPath = string.Empty;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class AssetDownloadProfile : ScriptableObject
    {
        public static readonly string DEBUG = D.L("[IMAGE_DOWNLOAD]");
        public static readonly string EMPTY = "EMPTY";
        public static readonly string EMPTY_JSON = "{}";
        private static readonly char SEP = Path.DirectorySeparatorChar;
        
        [SerializeField]
        private string DirectoryPath;

        [SerializeField]
        private string Profile;
        
        [SerializeField]
        private List<DownloadProfile> _Downloads = new List<DownloadProfile>();
        public List<DownloadProfile> Downloads
        {
            get { return _Downloads; }
            private set { _Downloads = value; }
        }

        private readonly DownloadProfile Empty = new DownloadProfile();
        private LocalData LocalData;

        [Button(25)]
        public void SetupProfiles()
        {
            FixDirectories();
            LoadCachedProfiles();
        }

        private void FixDirectories()
        {
            string DIRECTORY = Application.persistentDataPath;

            LocalData = new LocalData("Assets", "Downloaded", "Profiles", "Profiles");
            DirectoryPath = LocalData.LocalFolder;
            Profile = LocalData.LocalPath;
        }

        [Button(25)]
        private void LoadCachedProfiles()
        {
            Downloads.Clear();

            if (LocalData.Exists())
            {
                Downloads.AddRange(LocalData.LoadSetFromDisk<DownloadProfile>());
            }
            else
            {
                // Create empty
                LocalData.ReplaceToDisk(Empty);
                Downloads.Add(Empty);
            }
        }

        [Button(25)]
        private void WriteProfiles()
        {
            LocalData.ReplaceToDisk<DownloadProfile>(Downloads);
        }

        [Button(25)]
        public void ClearDownloads()
        {
            Downloads.Clear();
        }

        public bool HasProfile(string imageId)
        {
            Assertion.AssertNotEmpty(imageId);

            return Downloads != null 
                && Downloads.Count > 0
                && Downloads.Exists(d => d.ImageId.Equals(imageId));
        }

        public bool CheckTimestamp(string imageId, string timestamp)
        {
            DownloadProfile profile;
            return CheckTimestamp(imageId, timestamp, out profile);
        }

        public bool CheckTimestamp(string imageId, string timestamp, out DownloadProfile profile)
        {
            profile = Downloads.Find(d => d.ImageId.Equals(imageId));
            
            if (string.IsNullOrEmpty(timestamp))
            {
                timestamp = EMPTY;
            }
                
            if (profile == null)
            {
                return false;
            }

            return timestamp.Equals(profile.Timestamp);
        }

        public void AddDownload(DownloadProgress progress)
        {
            DownloadProfile profile = new DownloadProfile();
            bool hasProfile = HasProfile(progress.AssetID);
            bool hasUpdate = false;
            
            if (hasProfile)
            {
                hasUpdate = !CheckTimestamp(progress.AssetID, progress.Timestamp, out profile);
            }
            
            if (hasProfile && hasUpdate)
            {
                Debug.LogFormat("ImageDownloadProfile Updating.. Id:{0}-{1} Timestamp:{2}-{3}\n", profile.ImageId, progress.AssetID, profile.Timestamp, progress.Timestamp);

                profile.ImageId = progress.AssetID;
                profile.Timestamp = progress.Timestamp;
                profile.LocalPath = progress.AssetPath;
            }
            else if (!hasProfile)
            {
                Debug.LogFormat("ImageDownloadProfile Adding.. Id:{0}Timestamp:{1}\n", progress.AssetID, progress.Timestamp);

                profile = profile ?? new DownloadProfile();
                profile.ImageId = progress.AssetID;
                profile.Timestamp = progress.Timestamp;
                profile.LocalPath = progress.AssetPath;
                Downloads.Add(profile);
            }

            WriteProfiles();
        }

#if UNITY_EDITOR
        [MenuItem("Framework/Create/AssetDownloadProfile", false, 1011)]
        public static void CreateDownloadProfile()
        {
            const string dataFolder = "Assets/Sandbox/Downloader/UAssets";
            const string dataName = "AssetDownloadProfile.asset";
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

            AssetDownloadProfile downloadData = CreateInstance<AssetDownloadProfile>();
            AssetDatabase.CreateAsset(downloadData, filePath);
        }

        private static string FixPath(string path, string folder, string file, int itr)
        {
            if (File.Exists(path))
            {
                string newPath = folder + "/" + itr + file;
                return FixPath(newPath, folder, file, ++itr);
            }

            //Assertion.Assert(false, "Error");
            //return string.Empty;
            return path;
        }
#endif
    }
}
