using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using Common.Query;
using Common.Signal;

namespace Synergy88 {
	
	public class CoinItem : MonoBehaviour {

		[SerializeField]
		private Text labelTitle; 

		[SerializeField]
		private Text labelStoreId; 

		[SerializeField]
		private Text labelPrice; 

		[SerializeField]
		private CoinItemData data;

		private void Awake() {
			Assertion.AssertNotNull(this.labelTitle);
			Assertion.AssertNotNull(this.labelStoreId);
			Assertion.AssertNotNull(this.labelPrice);
		}

		private void Start() {
			this.Refresh();
		}

		private void Refresh() {
			// refresh visuals here
			this.labelTitle.text = this.data.ItemId;
			this.labelStoreId.text = this.data.ItemStoreId;
			this.labelPrice.text = this.data.ItemPrice;
		}

		public void UpdateData(CoinItemData data) {
			this.data = data;
		}

		public CoinItemData ItemData {
			get { return this.data; }
		}

		public void Purchase() {
			if (QuerySystem.Query<bool>(QueryIds.PurchaseInProgress)) {
				return;
			}

			Signal signal = S88Signals.ON_STORE_ITEM_PURCHASE;
			signal.ClearParameters();
			signal.AddParameter(S88Params.STORE_ITEM_ID, this.data.ItemId);
			signal.Dispatch();
		}
	}

}