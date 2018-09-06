using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;

using Framework;

namespace Sandbox.GraphQL
{
    using Sandbox.UnityAds;

    public class GraphQLDebugButton : MonoBehaviour
    {
        public void RequestRewardedAd()
        {
            this.Publish(new PlayAdRequestSignal() { IsSkippable = false, CustomAdType = CustomAdType.Reward, FallbackAdType = UnityAds.AdReward.FreeCoins });
        }

        public void RequestInterstitialAD()
        {
            this.Publish(new PlayAdRequestSignal() { IsSkippable = true, CustomAdType = CustomAdType.Interstitial, FallbackAdType = UnityAds.AdReward.NoReward });
        }
    }

}