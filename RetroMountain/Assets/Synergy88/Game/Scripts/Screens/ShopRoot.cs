using UnityEngine;
using UnityEngine.Purchasing;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;
using UnityEngine.UI;

namespace Synergy88 {
	
	public class ShopRoot : S88Scene {

		public GameObject _confirmationWindow;
		public Text _testTExt;

		[SerializeField]
		private GameObject template;

		[SerializeField]
		private GameObject ItemImages;

		[SerializeField]
		private List<ShopItemData> items;

		protected override void Awake() {
			PlayerPrefs.SetInt("BoughtNormal",1);
			base.Awake();
			Assertion.AssertNotNull(this.template);

			this.AddButtonHandler(EButtonType.ShopItem, (ISignalParameters parameters) => {
				ShopItem item = (ShopItem)parameters.GetParameter(S88Params.BUTTON_DATA);
				item.Purchase();
				Factory.Get<UnityAnalytics>().Track(item);
			});

			this.AddButtonHandler(EButtonType.Play, (ISignalParameters parameters) => {
				this.LoadScene<GameRoot>(EScene.Game);
				//S88Signals.ON_GAME_START.Dispatch();
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);

			});
		}


		[SerializeField]
		private ShopItemData data;

		public void ActivateConrimationWindow()
		{
			SoundControls.Instance._buttonClick.Play();
			_confirmationWindow.SetActive(true);
		}
		public void DisableConrimationWindow()
		{
			SoundControls.Instance._buttonClick.Play();
			_confirmationWindow.SetActive(false);
		}
		public void ConfirmationYes()
		{
			SoundControls.Instance._buttonClick.Play();
			string _id = data.ItemId;
			int _price = int.Parse(data.ItemPrice);
			int _number = int.Parse(data.ItemStoreId);


			S88Signals.ON_UPDATE_PLAYER_CURRENCY.ClearParameters();
			S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddParameter(S88Params.PLAYER_CURRENCY, -_price );
			S88Signals.ON_UPDATE_PLAYER_CURRENCY.Dispatch();
			PlayerPrefs.SetInt("Bought"+ data.ItemId,1);
			PlayerPrefs.SetInt("CurrentCharacter",_number) ;

			DisableConrimationWindow();
			S88Signals.ON_STORE_REFRESHWINDOW.Dispatch();

			S88Signals.ON_STORE_ITEM_PURCHASE.ClearParameters();
			S88Signals.ON_STORE_ITEM_PURCHASE.AddParameter(S88Params.STORE_ITEM_ID, _id);
			S88Signals.ON_STORE_ITEM_PURCHASE.Dispatch();
		}


		public void ResetPlayerPrefs()
		{

			PlayerPrefs.SetInt("BoughtCat",0);
			PlayerPrefs.SetInt("BoughtUnicorn",0);
			PlayerPrefs.SetInt("BoughtYoshi",0);
			PlayerPrefs.SetInt("BoughtSonic",0);
			PlayerPrefs.SetInt("BoughtDonkey",0);
			PlayerPrefs.SetInt("CurrentCharacter",0);
		}

		protected override void Start() {
			base.Start();

			this.PopulateItems();
		}

		protected override void OnEnable() {
			base.OnEnable();
			//this.StartCoroutine(this.ProcessStoreItems());

			S88Signals.ON_STORE_ITEM_PURCHASE_SUCCESSFUL.AddListener(this.OnStorePurchaseSuccessful);
			S88Signals.ON_STORE_ITEM_PURCHASE_FAILED.AddListener(this.OnStorePurchaseFailed);
			S88Signals.ON_STORE_ITEM_SOFTCURRENCY.AddListener(this.SoftCurrencyStore);

		}

		protected override void OnDisable() {
			base.OnDisable();
			this.StopAllCoroutines();

			S88Signals.ON_STORE_ITEM_PURCHASE_SUCCESSFUL.RemoveListener(this.OnStorePurchaseSuccessful);
			S88Signals.ON_STORE_ITEM_PURCHASE_FAILED.RemoveListener(this.OnStorePurchaseFailed);
			S88Signals.ON_STORE_ITEM_SOFTCURRENCY.RemoveListener(this.SoftCurrencyStore);

		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}
		/*
		private IEnumerator ProcessStoreItems() {
			yield return new WaitForSeconds(1.0f);

			if (!QuerySystem.Query<bool>(QueryIds.StoreIsReady)) {
				yield break;
			}
				
			//IEnumerable<Product> products = QuerySystem.Query<IEnumerable<Product>>(QueryIds.StoreItems);
			IQueryRequest request = QuerySystem.Start(QueryIds.StoreItemsWithType);
			request.AddParameter(QueryIds.StoreItemType, ProductType.NonConsumable);
			IEnumerable<Product> products = QuerySystem.Complete<IEnumerable<Product>>();

			foreach (Product product in products) {
				ProductMetadata meta = product.metadata;
				ProductDefinition definition = product.definition;
				//Debug.LogFormat("ShopRoot::ProcessStoreItems Product Id:{0} StoreId:{1} Details:{2}\n", definition.id, definition.storeSpecificId, meta.ToString());

				// push rpdocut to items
				if (!this.items.Exists(p => p.ItemId.Equals(definition.id))) {
					Debug.LogFormat("ShopRoot::ProcessStoreItems Product:{0}\n", product.definition.ToString());
					ShopItemData item = new ShopItemData();
					item.ItemId = definition.id;
					item.ItemStoreId = definition.storeSpecificId;
					item.ItemPrice = meta.localizedPriceString;

					this.items.Add(item);

				}

			}

			// propulate views
			this.PopulateItems();
		}
*/
		public void PopulateItems() {
			Transform parent = this.template.transform.parent;
			Vector3 localScale = this.template.transform.localScale;
			int len = this.items.Count;
			int children = parent.childCount - 1;;
			bool create = len == children;

			for (int i = 0; i < len; i++) {
				ShopItemData item = null;
				GameObject itemObject = null;
				ShopItem gameItem = null;

				try {
					item = this.items[i];
					itemObject = parent.GetChild(i + 1).gameObject;
					gameItem = itemObject.GetComponent<ShopItem>();
				}
				catch (UnityException e) {
					item = this.items[i];
					itemObject = (GameObject)GameObject.Instantiate(this.template);
					gameItem = itemObject.GetComponent<ShopItem>();

				}

				gameItem.UpdateData(item);

				// display item
				itemObject.transform.SetParent(parent);
				itemObject.transform.localScale = localScale;
				itemObject.SetActive(true);
			}
		}
		#region Signals

		private void OnStorePurchaseSuccessful(ISignalParameters parameters) {
			Product product = (Product)parameters.GetParameter(S88Params.STORE_ITEM);
			Debug.LogFormat("ShopRoot::OnStorePurchaseSuccessful ItemId:{0}\n", product.definition.id);
		}

		private void OnStorePurchaseFailed(ISignalParameters parameters) {
			Debug.LogFormat("ShopRoot::OnStorePurchaseFailed\n");
		}

		private void SoftCurrencyStore(ISignalParameters parameters)
		{
			data = ( ShopItemData )parameters.GetParameter(S88Params.STORE_ITEM);
		}
		#endregion

	}

}