using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Common.Signal;

namespace Synergy88 {
	
	public abstract class S88Signals {

		// Scenes
		public static readonly Signal ON_LOAD_SCENE = new Signal("OnLoadScene");
		public static readonly Signal ON_LOAD_SPLASH = new Signal("OnLoadSplash");
		public static readonly Signal ON_SPLASH_DONE = new Signal("OnSplashDone");
		public static readonly Signal ON_LOGIN_DONE = new Signal("OnLoginDone");

		// Buttons
		public static readonly Signal ON_CLICKED_BUTTON = new Signal("OnClickedButton");

		// player data
		public static readonly Signal ON_UPDATE_PLAYER_CURRENCY = new Signal("OnUpdatePlayerCurrency");

		// Login
		// Facebook
		public static readonly Signal FACEBOOK_LOGIN_REQUESTED = new Signal("OnFacebookLoginRequested");
		public static readonly Signal ON_FACEBOOK_LOGIN_FAILED = new Signal("OnFacebookLoginFailed");
		public static readonly Signal ON_FACEBOOK_LOGIN_SUCCESSFUL = new Signal("OnFacebookLoginSuccessful");

		// downloader
		public static readonly Signal ON_IMAGE_DOWNLOAD = new Signal("OnImageDownload");
		public static readonly Signal ON_IMAGE_DOWNLOADED = new Signal("OnImageDownloaded");

		// banner ads
		public static readonly Signal ON_SHOW_BANNER_ADS = new Signal("OnShowBannerAds");
		public static readonly Signal ON_HIDE_BANNER_ADS = new Signal("OnHideBannerAds");
		public static readonly Signal ON_TOGGLE_BANNER_ADS = new Signal("OnToggleBannerAds");

		// interstitials ads
		public static readonly Signal ON_SHOW_INTERSTITIAL_ADS = new Signal("OnShowInterstitialAds");
		public static readonly Signal ON_HIDE_INTERSTITIAL_ADS = new Signal("OnHideInterstitialAds");
		public static readonly Signal ON_TOGGLE_INTERSTITIAL_ADS = new Signal("OnToggleInterstitialAds");

		// unity ads
		public static readonly Signal ON_SHOW_UNITY_ADS = new Signal("OnShowUnityAds");

		// IAP
		public static readonly Signal ON_STORE_ITEM_PURCHASE = new Signal("OnStoreItemPurchase");
		public static readonly Signal ON_STORE_ITEM_SOFTCURRENCY= new Signal("OnStoreItemSoftCurrency");
		public static readonly Signal ON_STORE_REFRESHWINDOW= new Signal("OnStoreRefreshWindow");
		public static readonly Signal ON_STORE_ITEM_PURCHASE_SUCCESSFUL = new Signal("OnStoreItemPurchaseSuccessful");
		public static readonly Signal ON_STORE_ITEM_PURCHASE_FAILED = new Signal("OnStoreItemPurchaseFailed");
		public static readonly Signal ON_RESTORE_PURCHASE = new Signal("OnRestorePurchase");


		public static readonly Signal ON_GAME_START= new Signal("OnGameStartUp");
		public static readonly Signal ON_GAME_RESTART= new Signal("OnGameRestart");
		public static readonly Signal ON_GAME_PAUSE= new Signal("OnGamePause");
		public static readonly Signal ON_GAME_RESUME= new Signal("OnGameResume");
		public static readonly Signal ON_GAME_OVER= new Signal("OnGameOver");

	}

}