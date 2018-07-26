using Framework;

namespace Sandbox.Downloader
{
    /// <summary>
    /// A request to download an image with given ID from the given URL.
    /// </summary>
    public class DownloadAssetSignal
    {
        /// <summary>
        /// The ID of the image to be downloaded.
        /// </summary>
        public string AssetID { get; set; }

        /// <summary>
        /// The URL from which the image is to be downloaded.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Timestamp of the Downloadable Image
        /// </summary>
        public string Timestamp { get; set; }

        public bool AllowCaching = true;

        public DownloadAssetSignal()
        {

        }

        public DownloadAssetSignal(DownloadData data)
        {
            AssetID = data.shortCode;
            Url = data.url;
            Timestamp = data.lastModified;
        }

        public void FixTimestamp()
        {
            if (string.IsNullOrEmpty(Timestamp))
            {
                Timestamp = AssetDownloadProfile.EMPTY;
            }
        }
    }
}