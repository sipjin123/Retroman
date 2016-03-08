using UnityEngine;
using UnityEngine.Advertisements;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Signal;

// alias
using S88Const = Synergy88.Const;

namespace Synergy88 {
	
	public class UnityAds : MonoBehaviour {
		
		private void OnEnable() {
			S88Signals.ON_SHOW_UNITY_ADS.AddListener(this.OnShowUnityAds);
		}

		private void OnDisable() {
			S88Signals.ON_SHOW_UNITY_ADS.RemoveListener(this.OnShowUnityAds);
		}

		private void Initialize() {
			string adUnitId = string.Empty;

			// mobile
			if (Platform.IsMobileAndroid()) {
				adUnitId = S88Const.UNITY_GAME_ID_ANDROID;
			}
			else if (Platform.IsMobileIOS()) {
				adUnitId = S88Const.UNITY_GAME_ID_IOS;
			}
			// editor
			else {
				// editor
				#if UNITY_ANDROID
				adUnitId = S88Const.UNITY_GAME_ID_ANDROID;
				#elif UNITY_IOS
				adUnitId = S88Const.UNITY_GAME_ID_IOS;
				#else
				Assertion.Assert(false, "Unsupported Platform");
				#endif
			}

			if (!Advertisement.isInitialized) {
				Advertisement.Initialize(adUnitId);
			}
		}

		private void OnShowUnityAds(ISignalParameters parameters) {
			if (parameters != null && parameters.HasParameter(S88Params.UNITY_ADS_REGION)) {
				string region = (string)parameters.GetParameter(S88Params.UNITY_ADS_REGION);
				if (Advertisement.IsReady(region)) {
					ShowOptions options = new ShowOptions();
					options.resultCallback = this.HandleResult;
					Advertisement.Show(region, options);
				}
			}
			else {
				if (Advertisement.IsReady()) {
					Advertisement.Show();

					S88Signals.ON_UPDATE_PLAYER_CURRENCY.ClearParameters();
					S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddParameter(S88Params.PLAYER_CURRENCY,20);
					S88Signals.ON_UPDATE_PLAYER_CURRENCY.Dispatch();
				}
			}
		}

		private void HandleResult(ShowResult result) {
				Debug.LogError(result);
			Debug.LogFormat("UnityAds::HandleResult Result:{0}\n", result);

		}
	}

}