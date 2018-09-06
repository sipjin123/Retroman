using System;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Sandbox.GraphQL
{
    [Serializable]
    public class GraphQLSetupData : ScriptableObject
    {
        [SerializeField]
        private float _StartLoginRetryTimer;
        public float StartLoginRetryTimer
        {
            get { return _StartLoginRetryTimer; }
        }

        [SerializeField]
        private float _FetchAdRequestTimer;
        public float FetchAdRequestTimer
        {
            get { return _FetchAdRequestTimer; }
        }

        [SerializeField]
        private float _CacheCheckTimer;
        public float CacheCheckTimer
        {
            get { return _CacheCheckTimer; }
        }

        [SerializeField]
        private float _FlushRequestsTimer;
        public float FlushRequestsTimer
        {
            get { return _FlushRequestsTimer; }
        }

        [SerializeField]
        private float _SkipTimeForImageAds;

        public float SkipTimeForImageAds
        {
            get { return _SkipTimeForImageAds; }
        }

        [SerializeField]
        private float _EndTimeForImageAds;
        public float EndTimeForImageAds
        {
            get { return _EndTimeForImageAds; }
        }

        [SerializeField]
        private float _SkipTimeForVideoAds;
        public float SkipTimeForVideoAds
        {
            get { return _SkipTimeForVideoAds; }
        }

        [SerializeField]
        private int _MaxInterstitialAdsPerDay;
        public int MaxInterstitialAdsPerDay
        {
            get { return _MaxInterstitialAdsPerDay; }
        }

        [SerializeField]
        private int _MaxRewardsAdsPerDay;
        public int MaxRewardAdsPerDay
        {
            get { return _MaxRewardsAdsPerDay; }
        }
    }
}