using UnityEngine;

using System;
using System.Collections;

using Common;
using Common.Query;
using Common.Signal;
using UnityEngine.UI;

namespace Synergy88 {
	
	public class CurrencyRoot : S88Scene {

		public Text _currencyText;
		public Text _currencyText2;
		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			_currencyText.text = "" + PlayerPrefs.GetInt("TotalGold",0);
			_currencyText2.text = "" + PlayerPrefs.GetInt("TotalGold",0);
			base.Start();

			S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddListener(this.OnUpdatePlayerCurrency);

			this.AddButtonHandler(EButtonType.Currency, this.OnLoadCoinScene);
		}

		protected override void OnEnable() {
			base.OnEnable();
		}

		protected override void OnDisable() {
			base.OnDisable();
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			S88Signals.ON_UPDATE_PLAYER_CURRENCY.RemoveListener(this.OnUpdatePlayerCurrency);
		}

		private void OnLoadCoinScene(ISignalParameters parameters) {
			if (QuerySystem.Query<EScene>(QueryIds.CurrentScene) == EScene.Currency) {
				return;
			}

			this.LoadScene<CoinsRoot>(EScene.Coins);
			this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
			this.LoadSceneAdditive<BackRoot>(EScene.Back);
			this.LoadSceneAdditive<GameRoot>(EScene.Game);
		}

		#region Signals

		private void OnUpdatePlayerCurrency(ISignalParameters parameters) {
			int currency = PlayerPrefs.GetInt("TotalGold",0) + (int)parameters.GetParameter(S88Params.PLAYER_CURRENCY);
			PlayerPrefs.SetInt("TotalGold",currency);
			_currencyText.text = ""+currency;
			_currencyText2.text = ""+currency;
		}
		
		#endregion

	}

}