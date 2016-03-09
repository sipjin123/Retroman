using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using Common;
using Common.Signal;
using Common.Query;

namespace Synergy88 {

	public class ShopItem : MonoBehaviour {

		public GameObject _shopRoot;
		public GameObject _confirmationWindow;
		[SerializeField]
		private Text labelTitle; 

		[SerializeField]
		private Text labelStoreId; 

		[SerializeField]
		private Text labelPrice; 

		[SerializeField]
		private ShopItemData data;

		[SerializeField]
		private int _isInGame;

		public Image _itemImg;
		public GameObject _unselect, _select;

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
			this._isInGame = this.data.IfInGameItem;

			if(this._isInGame == 1)
			{
				this._itemImg.sprite = this.data.ItemImage.sprite;
				this._itemImg.color = Color.white;


				if(PlayerPrefs.GetInt("Bought"+this.labelTitle.text, 0) == 1)
				{
					this.data.ItemPrice ="0";
					this.labelPrice.text = "Select";
					_unselect.SetActive(true);
					_select.SetActive(false);
				}
				if(PlayerPrefs.GetInt("CurrentCharacter",0) == int.Parse( this.labelStoreId.text ))
				{
					this.labelPrice.text = "Selected";
					_unselect.SetActive(false);
					_select.SetActive(true);
				}
			}
		}

		public void UpdateData(ShopItemData data) {
			this.data = data;
		}

		public ShopItemData ItemData {
			get { return this.data; }
		}

		public void Purchase() {
			/*if (QuerySystem.Query<bool>(QueryIds.PurchaseInProgress)) {
				return;
			}*/

			//Debug.LogError(this.data.ItemId+" "+this.data.ItemPrice);

			string _itemID = this.data.ItemId;
			int _itemPrice = int.Parse( this.data.ItemPrice.ToString() );
			int _itemNum= int.Parse( this.data.ItemStoreId.ToString() );



			if(PlayerPrefs.GetInt("Bought"+_itemID,0) == 1)
			{
				PlayerPrefs.SetInt("CurrentCharacter",_itemNum) ;
				S88Signals.ON_STORE_REFRESHWINDOW.Dispatch();
				SoundControls.Instance._buttonClick.Play();
			}
			else
			{
				if(PlayerPrefs.GetInt("TotalGold",0) > _itemPrice)
				{
					_shopRoot.GetComponent<ShopRoot>().ActivateConrimationWindow();
					Signal signal = S88Signals.ON_STORE_ITEM_SOFTCURRENCY;
					signal.ClearParameters();
					signal.AddParameter(S88Params.STORE_ITEM, this.ItemData);
					signal.AddParameter(S88Params.STORE_ITEM_ID, this.ItemData.ItemId);
					signal.AddParameter(S88Params.STORE_ITEM_PRICE, this.ItemData.ItemPrice);
					signal.AddParameter(S88Params.STORE_ITEM_STORE_ID, this.ItemData.ItemStoreId);
					signal.Dispatch();
				}
			}

			/*
			Signal signal = S88Signals.ON_STORE_ITEM_PURCHASE;
			signal.ClearParameters();
			signal.AddParameter(S88Params.STORE_ITEM_ID, da
			signal.Dispatch();

			*/
			/*

			S88Signals.ON_UPDATE_PLAYER_CURRENCY.ClearParameters();
			S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddParameter(S88Params.PLAYER_CURRENCY, -_itemPrice);
			S88Signals.ON_UPDATE_PLAYER_CURRENCY.Dispatch();
			PlayerPrefs.SetInt("Bought"+_itemID,1);

			Signal signal = S88Signals.ON_STORE_ITEM_PURCHASE;
			signal.ClearParameters();
			signal.AddParameter(S88Params.STORE_ITEM_ID, this.data.ItemId);
			signal.Dispatch();
			*/
		}



		private void OnEnable()
		{
			S88Signals.ON_STORE_REFRESHWINDOW.AddListener(refreshButtons);
		}
		private void OnDestroy()
		{
			S88Signals.ON_STORE_REFRESHWINDOW.RemoveListener(refreshButtons);
		}
		private void refreshButtons(ISignalParameters parameters)
		{
			Refresh();
		}
	}

}