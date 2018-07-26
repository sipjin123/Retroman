using UnityEngine;
using UnityEngine.Video;

using Framework;

namespace Sandbox.Downloader
{
    /// <summary>
    /// A downloaded image with given ID.
    /// </summary>
    public class AssetDownloadedSignal
    {
        /// <summary>
        /// The ID of the downloaded image.
        /// </summary>
        public string AssetID { get; set; }

        /// <summary>
        /// The downloaded image.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The downloaded asset.
        /// </summary>
        public byte[] Data;
        
        /// <summary>
        /// The downloaded asset path.
        /// </summary>
        public string LocalPath { get; set; }
    }
}
