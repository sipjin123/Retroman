using Framework;

namespace Sandbox.UnityAds
{
    /// <summary>
    /// A request to show unity ads.
    /// </summary>
    public class ShowUnityAdsSignal
    {
        /// <summary>
        /// Optional ID of region the ads are for.
        /// </summary>
        public string Region { get; set; }
        public bool isRewardedVideo { get; set; }
        public UnityAds.AdReward rewardType { get; set; }
    }
}
