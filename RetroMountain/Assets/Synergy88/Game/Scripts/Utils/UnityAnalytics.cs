using UnityEngine;
using UnityEngine.Analytics;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Signal;
using Common.Utils;

namespace Synergy88 {
	
	public class UnityAnalytics : MonoBehaviour {
		
		// events
		public const string UNI_ANA_SCREEN_EVENT = "ScreenEvent";
		public const string UNI_ANA_BUTTON_EVENT = "ButtonEvent";
		// settings event
		// coins event
		public const string UNI_ANA_COINS_EVENT = "CoinsEvent";
		// shop event
		public const string UNI_ANA_SHOP_EVENT = "ShopEvent";
		// moregames event
		public const string UNI_ANA_MOREGAMES_EVENT = "MoreGamesEvent";

		// TODO: Generalize the keys
		// keys
		public const string UNI_ANA_SCREEN_KEY = "Screen";
		public const string UNI_ANA_BUTTON_KEY = "Button";

		// shop keys
		// coins keys
		public const string UNI_ANA_ITEM_ID = "ItemId";
		public const string UNI_ANA_ITEM_NAME = "ItemName";
		public const string UNI_ANA_ITEM_QUANTITY = "ItemQuantity";

		// TODO: Generalize the data
		private Dictionary<string, object> data = new Dictionary<string, object>();

		private void Awake() {
			Factory.Register<UnityAnalytics>(this);

			/*
			Dictionary<string, object> dummyData = new Dictionary<string, object>() {
				{ "key_001", 1 },
			};
			
			Analytics.CustomEvent("Event001", dummyData);
			Analytics.SetUserBirthYear(1991);
			Analytics.SetUserGender(Gender.Male);
			Analytics.SetUserId("User001");
			Analytics.Transaction("Product001", 0.99, "USD");
			*/
		}

		private void OnEnable() {
			S88Signals.ON_LOAD_SCENE.AddListener(this.OnLoadScene);
			S88Signals.ON_CLICKED_BUTTON.AddListener(this.OnClickedButton);
		}

		private void OnDisable() {
			S88Signals.ON_LOAD_SCENE.RemoveListener(this.OnLoadScene);
			S88Signals.ON_CLICKED_BUTTON.RemoveListener(this.OnClickedButton);
		}

		private void OnDestroy() {
			Factory.Clean<UnityAnalytics>();
		}

		#region Signals

		private void OnLoadScene(ISignalParameters parameters) {
			EScene scene = (EScene)parameters.GetParameter(S88Params.SCENE_NAME);
			this.data.Clear();
			this.data.Add(UNI_ANA_SCREEN_KEY, (object)scene.ToString());

			Analytics.CustomEvent(UNI_ANA_SCREEN_EVENT, this.data);
		}

		private void OnClickedButton(ISignalParameters parameters) {
			EButtonType button = (EButtonType)parameters.GetParameter(S88Params.BUTTON_TYPE);
			this.data.Clear();
			this.data.Add(UNI_ANA_SCREEN_KEY, (object)button.ToString());

			Analytics.CustomEvent(UNI_ANA_SCREEN_EVENT, this.data);
		}

		#endregion

		public void Track(ShopItem item) {
			this.data.Clear();
			this.data.Add(UNI_ANA_ITEM_ID, item.ItemData.ItemId);
			this.data.Add(UNI_ANA_ITEM_QUANTITY, 1);

			Analytics.CustomEvent(UNI_ANA_SHOP_EVENT, this.data);
		}

		public void Track(CoinItem item) {
			this.data.Clear();
			this.data.Add(UNI_ANA_ITEM_ID, item.ItemData.ItemId);
			this.data.Add(UNI_ANA_ITEM_QUANTITY, 1);

			Analytics.CustomEvent(UNI_ANA_COINS_EVENT, this.data);
		}

		public void Track(MoreGamesItem item) {
			this.data.Clear();
			this.data.Add(UNI_ANA_ITEM_ID, item.ItemData.ItemId);
			this.data.Add(UNI_ANA_ITEM_NAME, item.ItemData.Name);

			Analytics.CustomEvent(UNI_ANA_MOREGAMES_EVENT, this.data);
		}
	}

}