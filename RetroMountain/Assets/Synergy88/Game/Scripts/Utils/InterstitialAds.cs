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

	public class InterstitialAds : MonoBehaviour {

		private const string DUMMY_TEST_DEVICE = "0123456789ABCDEF0123456789ABCDEF";
		private const string DUMMY_KEYWORD = "game";
		private static readonly DateTime DUMMY_DATE = new DateTime(1985, 1, 1);
		private const string DUMMY_KEY = "key";
		private const string DUMMY_VALUE = "value";

		// interstitals
		private InterstitialAd interstitial;

		private void Awake() {
			S88Signals.ON_SHOW_INTERSTITIAL_ADS.AddListener(this.OnShowInterstitialAds);
			S88Signals.ON_HIDE_INTERSTITIAL_ADS.AddListener(this.OnHideInterstitialAds);
			S88Signals.ON_TOGGLE_INTERSTITIAL_ADS.AddListener(this.OnToggleInterstitialAds);
		}

		private void Start() {
			string adUnitId = string.Empty;

			// mobile
			if (Platform.IsMobileAndroid()) {
				adUnitId = S88Const.GA_INTERSTITIALS_AD_UNIT_ID_ANDROID;
			}
			else if (Platform.IsMobileIOS()) {
				adUnitId = S88Const.GA_INTERSTITIALS_AD_UNIT_ID_IOS;
			}
			// editor
			else {
				// editor
				#if UNITY_ANDROID
				adUnitId = S88Const.GA_INTERSTITIALS_AD_UNIT_ID_ANDROID;
				#elif UNITY_IOS
				adUnitId = S88Const.GA_INTERSTITIALS_AD_UNIT_ID_IOS;
				#else
				Assertion.Assert(false, "Unsupported Platform");
				#endif
			}

			// create interstitials
			this.interstitial = new InterstitialAd(adUnitId);

			// register for ad events.
			this.interstitial.AdLoaded += HandleInterstitialLoaded;
			this.interstitial.AdFailedToLoad += HandleInterstitialFailedToLoad;
			this.interstitial.AdOpened += HandleInterstitialOpened;
			this.interstitial.AdClosing += HandleInterstitialClosing;
			this.interstitial.AdClosed += HandleInterstitialClosed;
			this.interstitial.AdLeftApplication += HandleInterstitialLeftApplication;

			// add handler
			MobileAdsHandler handler = new MobileAdsHandler();
			this.interstitial.SetInAppPurchaseHandler(handler);

			// Load an interstitial ad.
			this.interstitial.LoadAd(this.CreateAdRequest());
		}

		private void OnDestroy() {
			S88Signals.ON_SHOW_INTERSTITIAL_ADS.RemoveListener(this.OnShowInterstitialAds);
			S88Signals.ON_HIDE_INTERSTITIAL_ADS.RemoveListener(this.OnHideInterstitialAds);
			S88Signals.ON_TOGGLE_INTERSTITIAL_ADS.RemoveListener(this.OnToggleInterstitialAds);
		}

		private AdRequest CreateAdRequest() {
			return new AdRequest.Builder()
				.AddTestDevice(AdRequest.TestDeviceSimulator)
				.AddTestDevice(DUMMY_TEST_DEVICE)
				.AddKeyword(DUMMY_KEYWORD)
				.SetGender(Gender.Male)
				.SetBirthday(DUMMY_DATE)
				.TagForChildDirectedTreatment(false)
				.AddExtra(DUMMY_KEY, DUMMY_VALUE)
				.Build();

		}

		#region Signals

		private void OnShowInterstitialAds(ISignalParameters parameters) {
			if (this.interstitial.IsLoaded()) {
				this.interstitial.Show();
			}
			// not yet ready
		}

		private void OnHideInterstitialAds(ISignalParameters parameters) {
			
		}

		private void OnToggleInterstitialAds(ISignalParameters parameters) {

		}
		
		#endregion

		#region Interstitial callback handlers

		private void HandleInterstitialLoaded(object sender, EventArgs args) {
		}

		private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
		}

		private void HandleInterstitialOpened(object sender, EventArgs args) {
		}

		private void HandleInterstitialClosing(object sender, EventArgs args) {
		}

		private void HandleInterstitialClosed(object sender, EventArgs args) {
		}

		private void HandleInterstitialLeftApplication(object sender, EventArgs args) {
		}

		#endregion
	}

	public class MobileAdsHandler : IInAppPurchaseHandler {
		
		private readonly string[] validSkus = { "android.test.purchased" };

		//Will only be sent on a success.
		public void OnInAppPurchaseFinished(IInAppPurchaseResult result) {
			result.FinishPurchase();
			// "Purchase Succeeded! Credit user here.";
		}

		//Check SKU against valid SKUs.
		public bool IsValidPurchase(string sku) {
			foreach (string validSku in validSkus) {
				if (sku == validSku) {
					return true;
				}
			}

			return false;
		}

		//Return the app's public key.
		public string AndroidPublicKey {
			//In a real app, return public key instead of null.
			get { return null; }
		}
	}
}