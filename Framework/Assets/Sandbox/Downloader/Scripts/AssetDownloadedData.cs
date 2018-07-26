using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using UnityEngine;

using Sirenix.OdinInspector;

using Common;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.Downloader
{
    using UniRx;
    using UniRx.Triggers;

    public enum PathSetting
    {
        Streaming,
        Persistent,
    }

    public class AssetDownloadedData : MonoBehaviour, IProgress<float>//, IAsyncResult
    {
        private static readonly char SEP = Path.DirectorySeparatorChar;
        
        [SerializeField]
        private string FilePath = string.Empty;

        private float Progress;

        private LocalData LocalData;

        private void Awake()
        {
            TryGeneratingPath();
        }

        [Button(25)]
        public void RegeneratePath()
        {
            FilePath = string.Empty;
            TryGeneratingPath();
        }

        public void Report(float value)
        {
            Progress = value;
        }

        private void TryGeneratingPath()
        {
            string DIRECTORY = Application.persistentDataPath;

            LocalData = LocalData ?? new LocalData("Assets", "Downloaded", "Textures", "TextureFiles");
            FilePath = LocalData.LocalFolder;
        }

        public bool ImageExistsLocally(string imageId, out string path)
        {
            TryGeneratingPath();

            path = string.Format("{0}{1}.png", FilePath, imageId);

#if ENABLE_LOCAL_CACHE
            if (File.Exists(path))
            {
                Debug.LogWarningFormat("WARINING! Image:{0} exists at path:{1}\n", imageId, path);
                return true;
            }
#endif
            return false;
        }

        public bool ImageExistsLocally(string assetID, out string path,string fileExtension)
        {
            TryGeneratingPath();

            path = string.Format("{0}{1}{2}", FilePath, assetID, fileExtension);

#if ENABLE_LOCAL_CACHE
            if (File.Exists(path))
            {
                Debug.LogWarningFormat("WARINING! Image:{0} exists at path:{1}\n", assetID, path);
                return true;
            }
#endif
            return false;
        }


        public void SaveData(string imageId, Texture2D texture)
        {
#if ENABLE_LOCAL_CACHE
            string path = string.Empty;
            if (!ImageExistsLocally(imageId, out path))
            {
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            else
            {
                File.Delete(path);
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
#endif
        }


        public void SaveData(string assetID, byte[] data, string fileExtension)
        {
#if ENABLE_LOCAL_CACHE
            string path = string.Empty;
            if (!ImageExistsLocally(assetID, out path, fileExtension))
            {
                File.WriteAllBytes(path, data);
            }
            else
            {
                File.Delete(path);

                File.WriteAllBytes(path, data);
                //ManualResetEvent manualEvent = new ManualResetEvent(false);
                //using (FileStream sourceStream = new FileStream(path,
                //     FileMode.CreateNew, FileAccess.Write, FileShare.None,
                //     bufferSize: 4096, useAsync: true))
                //{
                //    sourceStream.BeginWrite(data,0,data.Length,new AsyncCallback(bla),)
                //};
            }
#endif
        }

        public void GetData(string assetId, string fileExtension)
        {
            string path = string.Empty;
            if (ImageExistsLocally(assetId, out path, fileExtension))
            {
                byte[] fileData = File.ReadAllBytes(path);

                this.Publish(new AssetDownloadedSignal()
                {
                    AssetID = assetId,
                    Data = fileData,
                    LocalPath = path
                });
            }
        }

        public void GetImage(string imageId)
        {
            string path = string.Empty;
            if (ImageExistsLocally(imageId, out path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(fileData);
                //texture.alphaIsTransparency = true;
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;

                this.Publish(new AssetDownloadedSignal()
                {
                    AssetID = imageId,
                    Texture = texture,
                    Data = fileData,
                    LocalPath = path
                });
            }
        }
    }
}