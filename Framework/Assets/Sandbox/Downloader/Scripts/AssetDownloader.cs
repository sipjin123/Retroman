using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.Downloader
{
    using UniRx;
    using UniRx.Triggers;

    using Sandbox.Services;
    
    public class AssetDownloader : BaseService
    {
        #region IService implementation

        [SerializeField]
        private AssetDownloadProfile DownloadProfile;

        [SerializeField]
        private List<DownloadProgress> DownloadQueue = new List<DownloadProgress>();

        [SerializeField]
        private List<DownloadProgress> Downloaded = new List<DownloadProgress>();

        [SerializeField]
        private List<DownloadProgress> Errors = new List<DownloadProgress>();

        private AssetDownloadedData DownloadedData;

        public override IEnumerator InitializeServiceSequentially()
        {
            this.Receive<DownloadAssetSignal>()
                //.ObserveOnMainThread()
                //.Delay(TimeSpan.FromSeconds(0.15))
                .Subscribe(sig => OnDownloadImage(sig))
                .AddTo(this);

            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void InitializeService()
        {
            this.Receive<DownloadAssetSignal>()
                //.ObserveOnMainThread()
                //.Delay(TimeSpan.FromSeconds(0.15))
                .Subscribe(sig => OnDownloadImage(sig))
                .AddTo(this);

            CurrentServiceState.Value = ServiceState.Initialized;
        }
        
        public override void TerminateService()
        {
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            DownloadedData = GetComponent<AssetDownloadedData>();
            DownloadProfile.SetupProfiles();
        }

        private void Update()
        {
            if (DownloadQueue.Count > 0)
            {
                DownloadProgress progress = DownloadQueue.FirstOrDefault();
                
                switch (progress.Status)
                {
                    case DownloadStatus.Pending:
                        progress.Download(this);
                        break;
                    case DownloadStatus.Downloading:
                        break;
                    case DownloadStatus.Downloaded:
                        progress.Publish();

                        DownloadQueue.RemoveAt(0);

                        if (progress.AllowCaching)
                        {
                            string ext = Path.GetExtension(progress.AssetURL);

                            Debug.LogFormat(D.L("[DOWNLOAD]") + " AssetDownloader::Downloaded Saving file:{0} ext:{1} locally.\n", progress.AssetID, ext);

                            Downloaded.Add(progress);
                            
                            DownloadedData.SaveData(progress.AssetID, progress.Data, ext);
                            DownloadProfile.AddDownload(progress);
                            progress.Data = null;
                            GC.Collect();
                        }
                        break;
                    case DownloadStatus.Error:
                        Debug.LogFormat(D.E("[DOWNLOAD ERROR]") + " AssetDownloader::Download Error downloading file:{0}\n", progress.AssetID);

                        //Factory.Get<GSDownloadableModule>().DownloadRequest(progress.ImageId);
                        Errors.Add(progress);
                        DownloadQueue.RemoveAt(0);
                        break;
                }
            }
        }

        private void OnDownloadImage(DownloadAssetSignal signal)
        {
            // fix timestamp
            signal.FixTimestamp();

            string imageId = signal.AssetID;
            string imageUrl = signal.Url;
            string timestamp = signal.Timestamp;
            string path = string.Empty;

            Assertion.AssertNotEmpty(imageId, D.ERROR + "ImageDownloader::OnDownloadImage Asset:{0} should never be null or empty.\n", imageId);
            Assertion.AssertNotEmpty(imageUrl, D.ERROR + "ImageDownloader::OnDownloadImage Asset:{0} should never be null or empty.\n", imageId);

            if (string.IsNullOrEmpty(imageId)
            || string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            bool hasUpdates = true;
            if (DownloadProfile.HasProfile(imageId))
            {
                hasUpdates = !DownloadProfile.CheckTimestamp(imageId, timestamp);
            }

            if (signal.AllowCaching
            && DownloadedData.ImageExistsLocally(imageId, out path, Path.GetExtension(imageUrl))
            && !hasUpdates)
            {
                DownloadedData.GetData(imageId, Path.GetExtension(imageUrl));
                return;
            }

            if (signal.AllowCaching
            && DownloadQueue.Exists(d => d.AssetID.Equals(imageId))
            && !hasUpdates)
            {
                return;
            }

            if (signal.AllowCaching
            && Downloaded.Exists(d => d.AssetID.Equals(imageId))
            && !hasUpdates)
            {
                Downloaded.Find(d => d.AssetID.Equals(imageId)).Publish();
                return;
            }

            // Add to download queue
            DownloadProgress progress = new DownloadProgress();
            progress.AssetID = imageId;
            progress.AssetURL = imageUrl;
            progress.AssetPath = path;
            progress.Timestamp = timestamp;
            progress.Progress = 0f;
            progress.AllowCaching = signal.AllowCaching;

            DownloadQueue.Add(progress);
        }
    }
}