using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Framework;

namespace Sandbox.Downloader
{
    using UniRx;
    using UniRx.Triggers;

    using Sandbox.Services;

    public enum DownloadStatus
    {
        Pending,
        Downloading,
        Downloaded,
        Error,
    }

    // TODO: Re-integrate this using Fsm
    [Serializable]
    public class DownloadProgress : IProgress<float>
    {
        public bool AllowCaching;
        public string AssetID;
        public string AssetURL;
        public string AssetPath;
        public string Timestamp;

        [HideInInspector]
        public byte[] Data;

        [Range(0f, 1f)]
        public float Progress = 0f;     //0-1

        public DownloadStatus Status = DownloadStatus.Pending;

        public void Download(MonoBehaviour runnable)
        {
            Status = DownloadStatus.Downloading;

            ObservableWWW
                .GetWWW(url: AssetURL, headers: null, progress: this)
                .Take(1)
                .Subscribe(
                    www =>
                    {
                        Debug.LogFormat("Next:{0} Id:{1} Progress:{2}\n", www, AssetID, Progress);
                        Data = www.bytes;
                        //Data.wrapMode = TextureWrapMode.Clamp;
                    },
                    error =>
                    {
                        Debug.LogWarningFormat("Error! Id:{0} Progress:{1} Error:{2}\n", AssetID, Progress, error.Message);
                        Status = DownloadStatus.Error;
                    },
                    delegate
                    {
                        Debug.LogFormat("Done! Id:{0} Progress:{1}\n", AssetID, Progress);
                        Status = DownloadStatus.Downloaded;
                    })
                    .AddTo(runnable);
        }

        public void Publish()
        {
            MessageBroker.Default.Publish(new AssetDownloadedSignal()
            {
                AssetID = AssetID,
                Data = Data,
                LocalPath = AssetPath
            });
        }

        public void Report(float value)
        {
            Progress = value;
        }
    }
}