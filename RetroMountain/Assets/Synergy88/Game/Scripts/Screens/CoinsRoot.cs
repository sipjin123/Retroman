﻿using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common;
using Common.Signal;
using Common.Utils;
using Common.Query;

namespace Synergy88 {

	public class CoinsRoot : S88Scene {

		[SerializeField]
		private GameObject template;

		[SerializeField]
		private List<CoinItemData> items;

		protected override void Awake() {
			base.Awake();
			Assertion.AssertNotNull(this.template);

			this.AddButtonHandler(EButtonType.CoinItem, (ISignalParameters parameters) => {
				CoinItem item = (CoinItem)parameters.GetParameter(S88Params.BUTTON_DATA);
				item.Purchase();
				Factory.Get<UnityAnalytics>().Track(item);
			});
		}

		protected override void Start() {
			base.Start();
		}

		protected override void OnEnable() {
			base.OnEnable();

			this.StartCoroutine(this.ProcessStoreItems());

			S88Signals.ON_STORE_ITEM_PURCHASE_SUCCESSFUL.AddListener(this.OnStorePurchaseSuccessful);
			S88Signals.ON_STORE_ITEM_PURCHASE_FAILED.AddListener(this.OnStorePurchaseFailed);
		}

		protected override void OnDisable() {
			base.OnDisable();

			S88Signals.ON_STORE_ITEM_PURCHASE_SUCCESSFUL.RemoveListener(this.OnStorePurchaseSuccessful);
			S88Signals.ON_STORE_ITEM_PURCHASE_FAILED.RemoveListener(this.OnStorePurchaseFailed);
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

		private IEnumerator ProcessStoreItems() {
			yield return new WaitForSeconds(1.0f);

			if (!QuerySystem.Query<bool>(QueryIds.StoreIsReady)) {
				yield break;
			}

			//IEnumerable<Product> products = QuerySystem.Query<IEnumerable<Product>>(QueryIds.StoreItems);
			IQueryRequest request = QuerySystem.Start(QueryIds.StoreItemsWithType);
			request.AddParameter(QueryIds.StoreItemType, ProductType.Consumable);
			IEnumerable<Product> products = QuerySystem.Complete<IEnumerable<Product>>();

			foreach (Product product in products) {
				ProductMetadata meta = product.metadata;
				ProductDefinition definition = product.definition;
				//Debug.LogFormat("ShopRoot::ProcessStoreItems Product Id:{0} StoreId:{1} Details:{2}\n", definition.id, definition.storeSpecificId, meta.ToString());

				// push rpdocut to items
				if (!this.items.Exists(p => p.ItemId.Equals(definition.id))) {
					CoinItemData item = new CoinItemData();
					item.ItemId = definition.id;
					item.ItemStoreId = definition.storeSpecificId;
					item.ItemPrice = meta.localizedPriceString;
					this.items.Add(item);
				}
			}

			// propulate views
			this.PopulateItems();
		}

		public void PopulateItems() {
			Transform parent = this.template.transform.parent;
			Vector3 localScale = this.template.transform.localScale;
			int len = this.items.Count;
			int children = parent.childCount - 1;;
			bool create = len == children;

			for (int i = 0; i < len; i++) {
				CoinItemData item = null;
				GameObject itemObject = null;
				CoinItem gameItem = null;

				try {
					item = this.items[i];
					itemObject = parent.GetChild(i + 1).gameObject;
					gameItem = itemObject.GetComponent<CoinItem>();
				}
				catch (UnityException e) {
					item = this.items[i];
					itemObject = (GameObject)GameObject.Instantiate(this.template);
					gameItem = itemObject.GetComponent<CoinItem>();
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
			Debug.LogFormat("CoinsRoot::OnStorePurchaseSuccessful ItemId:{0}\n", product.definition.id);
		}

		private void OnStorePurchaseFailed(ISignalParameters parameters) {
			Debug.LogFormat("CoinsRoot::OnStorePurchaseFailed\n");
		}

		#endregion
	}

}