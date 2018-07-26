using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sandbox.ScratchCard
{
    [CreateAssetMenu]
    public class TextureBlitData : ScriptableObject, IScriptableData
    {
        [SerializeField, ShowInInspector]
        private Texture2D _Texture;
        public Texture2D Texture
        {
            get { return _Texture; }
        }
        
        [SerializeField, ShowInInspector]
        private Texture2D _Brush;
        public Texture2D Brush
        {
            get { return _Brush; }
        }

        [SerializeField, ShowInInspector]
        private Material _Material;
        public Material Material
        {
            get { return _Material; }
        }

#region UNITY_EDITOR
#if UNITY_EDITOR
        [MenuItem("Framework/Create/ZipDownloadData")]
        public static void CreateAsset()
        {
            /*
            const string dataFolder = "Assets/Sandbox/ScratchCard/UAssets";
            const string fileName = "TextureBlitData.asset";
            const string dataPath = dataFolder + "/" + data;
            string filePath = FixPath(dataPath, dataFolder, fileName, 0);

            
            if (File.Exists(fileName))
            {
                return;
            }

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            TextureBlitData downloadData = CreateInstance<TextureBlitData>();
            AssetDatabase.CreateAsset(downloadData, filePath);
            //*/
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