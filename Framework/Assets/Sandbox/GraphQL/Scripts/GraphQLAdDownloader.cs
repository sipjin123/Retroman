using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{
    using Sandbox.Downloader;

    [Serializable]
    public class AdEntry
    {
        public string AssetId;
        public string LocalPath;
    }

    public class GraphQLAdDownloader : MonoBehaviour
    {
        // key = assetid, value = local path
        [SerializeField]
        private List<AdEntry> CompletedAds = new List<AdEntry>();

        [SerializeField]
        private List<Advertisement> Ads = new List<Advertisement>();

        private void Awake()
        {
            this.Receive<AssetDownloadedSignal>()
                // Changes
                .Where(_ => Ads.Any(Ad => _.AssetID.Equals(Ad.Id) && !CompletedAds.Exists(a => a.AssetId.Equals(_.AssetID))))
                .Subscribe(_ => 
                {
                    int index = CompletedAds.FindIndex(a => a.AssetId.Equals(_.AssetID));

                    if (index < 0)
                    {
                        CompletedAds.Add(new AdEntry() { AssetId = _.AssetID, LocalPath = _.LocalPath });
                    }
                    else
                    {
                        CompletedAds[index].AssetId = _.AssetID;
                        CompletedAds[index].LocalPath = _.LocalPath;
                    }
                })
                .AddTo(this);
        }

        public void DownloadAds(List<Advertisement> NewAds)
        {
            Ads.Clear();
            Ads.AddRange(NewAds);
            Ads.ForEach(ad => 
            {
                this.Publish(new DownloadAssetSignal()
                {
                    AssetID = ad.Id,         
                    Url = ad.Url,            
                    Timestamp = ad.Timestamp
                });
            });
        }

        public bool AdDownloaded(string assetId)
        {
            return CompletedAds.Exists(_ => _.AssetId.Equals(assetId));
        }

        public string GetAdLocalPath(string assetId)
        {
            if (!AdDownloaded(assetId))
            {
                return string.Empty;
            }
            else
            {
                return CompletedAds.Find(_ => _.AssetId.Equals(assetId)).LocalPath;
            }
        }

        public bool DownloadComplete()
        {
            return false;
        }
        
    }
}