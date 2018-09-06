using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.GraphQL
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [Serializable]
    public class GraphInfo : ScriptableObject
    {
        [SerializeField]
        private string _Build;
        public string Build { get { return _Build; } }

        [SerializeField]
        private string _GraphURL;
        public string GraphURL { get { return _GraphURL; } }

        [SerializeField]
        private string _GameSlug;
        public string GameSlug { get { return _GameSlug; } }

#if UNITY_EDITOR
        [MenuItem("Framework/Create/GraphInfo", false, 1011)]
        public static void CreateURLMap()
        {
            const string dataFolder = "Assets/Sandbox/GraphQLSandbox/UAssets";
            const string dataName = "GraphInfo.asset";
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

            GraphInfo downloadData = CreateInstance<GraphInfo>();
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