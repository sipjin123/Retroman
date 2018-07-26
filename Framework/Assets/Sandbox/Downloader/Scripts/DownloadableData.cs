using System;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using Common.Utils;

using Framework;

namespace Sandbox.Downloader
{
    [Serializable]
    public class DownloadData
    {
        public int size { get; set; }
        public string lastModified { get; set; }
        public string requestId { get; set; }
        public string shortCode { get; set; }
        public string url { get; set; }

        /// <summary>
        /// Custom Properties
        /// </summary>
        public bool Downloaded { get; set; }

        public bool HasNull()
        {
            return
                string.IsNullOrEmpty(lastModified) ||
                string.IsNullOrEmpty(requestId) ||
                string.IsNullOrEmpty(shortCode) ||
                string.IsNullOrEmpty(url);
        }

        public void UpdateFrom(DownloadData from)
        {
            size = from.size;
            lastModified = from.lastModified;
            requestId = from.requestId;
            shortCode = from.shortCode;
            url = from.url;
        }

        public void Assert()
        {
            Assertion.Assert(!string.IsNullOrEmpty(lastModified), string.Format("Error! Invalid lastModified!"));
            Assertion.Assert(!string.IsNullOrEmpty(requestId), string.Format("Error! Invalid requestId!"));
            Assertion.Assert(!string.IsNullOrEmpty(shortCode), string.Format("Error! Invalid shortCode!"));
            Assertion.Assert(!string.IsNullOrEmpty(url), string.Format("Error! Invalid url!"));
        }
    }

    public class DownloadableData : MonoBehaviour
    {
        [SerializeField]
        private List<DownloadData> _Downloads = new List<DownloadData>();
        public List<DownloadData> Downloads
        {
            get { return _Downloads; }
            set { _Downloads = value; }
        }

        private DownloadData EmptyData = new DownloadData();

        [Button(25)]
        public void Clear()
        {
            Downloads.Clear();
        }

        public void Download()
        {

        }
    
        public bool HasDownloadData(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                Debug.LogErrorFormat("Error! Invalid shortcode!");
                return false;
            }
            
            Predicate<DownloadData> condition = d => d.shortCode.Equals(shortCode);
            return Downloads.Exists(condition);
        }
        
        public DownloadData GetDownloadData(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                Debug.LogErrorFormat("Error! Invalid shortcode!");
                return EmptyData;
            }

            Predicate<DownloadData> condition = d => d.shortCode.Equals(shortCode);
            //Assertion.Assert(Downloads.Exists(condition), "ERROR! missing shortcode! Code:" + shortCode);
            
            if (!Downloads.Exists(condition))
            {
                Debug.LogErrorFormat("ERROR! missing shortcode! Code:{0}\n", shortCode);
                return EmptyData;
            }

            return Downloads.Find(condition);
        }

        public void CleanDownloadables()
        {
            //Debug.LogErrorFormat("A Clean Downloads. Count:{0}\n", Downloads.Count);
            
            Downloads.RemoveAll(d => d == null || d.HasNull());

            //Debug.LogErrorFormat("B Clean Downloads. Count:{0}\n", Downloads.Count);
        }
    }
}
