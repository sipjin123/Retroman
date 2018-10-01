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

namespace Sandbox.RGC
{
    public enum RegData
    {
        first_name,
        middle_name,
        last_name,
        birthdate,
        gender,
        address,
        city,
        mobile_number,
        email,
    };

    public enum Gender
    {
        male,
        female,
    };

    [Serializable]
    public class StorageEntry : IJson
    {
        public RegData Type;
        public string Value;
    }

    public class StorageData : ScriptableObject 
    {
        protected static readonly char SEP = Path.DirectorySeparatorChar;

        protected LocalData LocalData;

        [SerializeField]
        public string LocalPath;

        [SerializeField]
        protected RegData _Type;
        public RegData Type
        {
            get { return _Type; }
            protected set { _Type = value; }
        }

        [SerializeField]
        protected string _Value;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private readonly Dictionary<RegData, string> DEFAULTS = new Dictionary<RegData, string>()
        {
            { RegData.first_name,       "first" },
            { RegData.middle_name,      "middle" },
            { RegData.last_name,        "last" },
            { RegData.birthdate,        DateTime.Now.ToString("MM/dd/yyyy") },
            { RegData.gender,           Gender.male.ToString() },
            { RegData.address,          "1600 Amphitheatre Parkway, Mountain View, CA, 94043" },
            { RegData.city,             "Mountain View" },
            { RegData.mobile_number,    "09697777666" },
            { RegData.email,            "bojack.cholo@thunder.com" },
        };

        [Button(ButtonSizes.Medium)]
        public virtual void Save()
        {
            StorageEntry entry = new StorageEntry();
            entry.Type = Type;
            entry.Value = Value;

            LocalData = new LocalData("User", "Profile", Type.ToString());
            LocalData.ReplaceToDisk(entry);
        }

        [Button(ButtonSizes.Medium)]
        public virtual void Load()
        {
            LocalData = new LocalData("User", "Profile", Type.ToString());
            if (LocalData.Exists())
            {
                StorageEntry entry = LocalData.LoadFromDisk<StorageEntry>();
                Type = entry.Type;
                Value = entry.Value;
            }
            else
            {
                // Defaults
                Value = DEFAULTS[Type];
                Save();
            }
        }

        [Button(ButtonSizes.Medium)]
        public void Delete()
        {
            LocalData = new LocalData("User", "Profile", Type.ToString());
            LocalData.Delete();
        }

        public string GetDefaultValue()
        {
            return Value;
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium)]
        public void Rename()
        {
            LocalPath = AssetDatabase.GetAssetPath(GetInstanceID());
            AssetDatabase.RenameAsset(LocalPath, string.Format("{0}.asset", Type));
        }


        [MenuItem("Framework/Create/StorageData", false, 1011)]
        public static void CreateData()
        {
            string dataFolder = string.Format("Assets{0}Sandbox{0}RGC{0}UAssets", SEP);
            string dataName = "StorageData.asset";
            string dataPath = dataFolder + SEP + dataName;
            string filePath = FixPath(dataPath, dataFolder, dataName, 0);

            if (File.Exists(dataName))
            {
                return;
            }

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            StorageData data = CreateInstance<StorageData>();
            AssetDatabase.CreateAsset(data, filePath);
        }

        private static string FixPath(string path, string folder, string file, int itr)
        {
            if (File.Exists(path))
            {
                string newPath = folder + SEP + itr + file;
                return FixPath(newPath, folder, file, ++itr);
            }

            return path;
        }
#endif
    }
}
