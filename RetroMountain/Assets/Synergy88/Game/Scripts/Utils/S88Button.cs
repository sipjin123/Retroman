using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Signal;

public enum EButtonType {
	Invalid,

	// Login (Facebook and non Facebook)
	Login,
	LoginFacebook,

	// Home
	MoreGames,
	Currency,
	Help,
	Leaderboard,
	Shop,
	Play,
	Settings,

	// Coins
	Back,

	// In Game
	Score,

	Refresh, // retry?
	Home,
	Upload, // share?
	Ads,		// clicked watch button for ads

	Credits,
	Restore,

	MoreGamesItem,
	ShopItem,
	CoinItem,
};
	
namespace Synergy88 {
	
	public class S88Button : MonoBehaviour {

		[SerializeField]
		private EButtonType button;

		private void Awake() {
			Assertion.Assert(this.button != EButtonType.Invalid);
		}

		public void OnClickedButton() {
			Signal signal = S88Signals.ON_CLICKED_BUTTON;
			signal.ClearParameters();
			signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			signal.Dispatch();
		}

		public void OnClickedButton(object data) {
			Signal signal = S88Signals.ON_CLICKED_BUTTON;
			signal.ClearParameters();
			signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			signal.AddParameter(S88Params.BUTTON_DATA, data);
			signal.Dispatch();
		}

		public void OnClickedButtonWithItem(MoreGamesItem item) {
			Signal signal = S88Signals.ON_CLICKED_BUTTON;
			signal.ClearParameters();
			signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			signal.AddParameter(S88Params.BUTTON_DATA, item);
			signal.Dispatch();
		}

		public void OnClickedButtonWithItem(ShopItem item) {
			Signal signal = S88Signals.ON_CLICKED_BUTTON;

			signal.ClearParameters();
			signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			signal.AddParameter(S88Params.BUTTON_DATA, item);
			signal.Dispatch();


			//signal.ClearParameters();
			//signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			//signal.AddParameter(S88Params.BUTTON_DATA, item);

			//signal.AddParameter(S88Params.STORE_ITEM_ID, item.ItemData.ItemId);
			//signal.AddParameter(S88Params.STORE_ITEM_PRICE, item.ItemData.ItemPrice);
			//signal.Dispatch();
		}

		public void OnClickedButtonWithItem(CoinItem item) {
			Signal signal = S88Signals.ON_CLICKED_BUTTON;
			signal.ClearParameters();
			signal.AddParameter(S88Params.BUTTON_TYPE, this.button);
			signal.AddParameter(S88Params.BUTTON_DATA, item.ItemData);
			signal.Dispatch();
		}
	}

}