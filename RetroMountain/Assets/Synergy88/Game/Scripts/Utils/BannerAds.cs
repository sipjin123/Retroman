using UnityEngine;

using System;
using System.Collections;

using GoogleMobileAds;
using GoogleMobileAds.Api;

using Common;
using Common.Signal;

// alias
using S88Const = Synergy88.Const;

namespace Synergy88 {
	
	public class BannerAds : MonoBehaviour {

		// banner
		private BannerView bannerView;
		private bool bannerToggle = false;

		private void Awake() {
			S88Signals.ON_SHOW_BANNER_ADS.AddListener(this.OnShowBannerAds);
			S88Signals.ON_HIDE_BANNER_ADS.AddListener(this.OnHideBannerAds);
			S88Signals.ON_TOGGLE_BANNER_ADS.AddListener(this.OnToggleBannerAds);
		}

		private void Start() {
			string adUnitId = string.Empty;

			// mobile
			if (Platform.IsMobileAndroid()) {
				adUnitId = S88Const.GA_BANNER_AD_UNIT_ID_ANDROID;
			}
			else if (Platform.IsMobileIOS()) {
				adUnitId = S88Const.GA_BANNER_AD_UNIT_ID_IOS;
			}
			// editor
			else {
				// editor
				#if UNITY_ANDROID
				adUnitId = S88Const.GA_BANNER_AD_UNIT_ID_ANDROID;
				#elif UNITY_IOS
				adUnitId = S88Const.GA_BANNER_AD_UNIT_ID_IOS;
				#else
				Assertion.Assert(false, "Unsupported Platform");
				#endif
			}

			// create ad
			this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

			// Create ad request
			AdRequest request = new AdRequest.Builder().Build();

			// Load a banner ad.
			this.bannerView.LoadAd(request);

			// Register for ad events.
			this.bannerView.AdLoaded += HandleAdLoaded;
			this.bannerView.AdFailedToLoad += HandleAdFailedToLoad;
			this.bannerView.AdOpened += HandleAdOpened;
			this.bannerView.AdClosing += HandleAdClosing;
			this.bannerView.AdClosed += HandleAdClosed;
			this.bannerView.AdLeftApplication += HandleAdLeftApplication;

			// hide banner ads
			this.OnHideBannerAds(null);
		}

		private void OnDestroy() {
			S88Signals.ON_SHOW_BANNER_ADS.RemoveListener(this.OnShowBannerAds);
			S88Signals.ON_HIDE_BANNER_ADS.RemoveListener(this.OnHideBannerAds);
			S88Signals.ON_TOGGLE_BANNER_ADS.RemoveListener(this.OnToggleBannerAds);

			// Register for ad events.
			this.bannerView.AdLoaded -= HandleAdLoaded;
			this.bannerView.AdFailedToLoad -= HandleAdFailedToLoad;
			this.bannerView.AdOpened -= HandleAdOpened;
			this.bannerView.AdClosing -= HandleAdClosing;
			this.bannerView.AdClosed -= HandleAdClosed;
			this.bannerView.AdLeftApplication -= HandleAdLeftApplication;
		}

		#region Signals

		private void OnShowBannerAds(ISignalParameters parameters) {
			this.bannerView.Show();
			this.bannerToggle = true;
		}

		private void OnHideBannerAds(ISignalParameters parameters) {
			this.bannerView.Hide();
			this.bannerToggle = false;
		}

		private void OnToggleBannerAds(ISignalParameters parameters) {
			if (this.bannerToggle) {
				this.OnHideBannerAds(null);
			}
			else {
				this.OnShowBannerAds(null);
			}
		}

		#endregion

		#region Banner Events

		private void HandleAdLoaded(object sender, EventArgs args) {
		}

		private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
		}

		private void HandleAdOpened(object sender, EventArgs args) {
		}

		private void HandleAdClosing(object sender, EventArgs args) {
		}

		private void HandleAdClosed(object sender, EventArgs args) {
		}

		private void HandleAdLeftApplication(object sender, EventArgs args) {
		}

		#endregion
	}

}