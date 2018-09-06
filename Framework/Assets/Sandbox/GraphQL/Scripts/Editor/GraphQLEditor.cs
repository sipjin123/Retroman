using System.IO;

using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

namespace Sandbox.GraphQL
{
    using Framework;

    public class GraphQLEditor 
    {
        [MenuItem("Assets/Create/GarphQLData")]
        public static void CreateGraphQLData()
        {
            const string dataFolder = "Assets/Sandbox/GraphQLSandbox/UAssets";
            const string dataName = "GraphSetupData.asset";
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

            GraphQLSetupData downloadData = ScriptableObject.CreateInstance<GraphQLSetupData>();
            AssetDatabase.CreateAsset(downloadData, filePath);
            AssetDatabase.SaveAssets();
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
    }
}