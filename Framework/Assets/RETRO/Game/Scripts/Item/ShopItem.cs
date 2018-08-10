using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using Common;
using Common.Signal;
using Common.Utils;
using Retroman;
using UniRx;

namespace Synergy88
{

	public class ShopItem : MonoBehaviour
    {

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
		private bool _isInGame;

		public Image _itemImg;
		public GameObject _unselect, _select;

		private void Awake()
        {
			Assertion.AssertNotNull(this.labelTitle);
			Assertion.AssertNotNull(this.labelStoreId);
			Assertion.AssertNotNull(this.labelPrice);
            Factory.Get<DataManagerService>().MessageBroker.Receive<RefereshAllItems>().Subscribe(_ => 
            {
                this.Refresh();
            }).AddTo(this);
		}
		private void Start()
        {
            this.Refresh();
		}

		public void Refresh()
        {
			// refresh visuals here
			this.labelTitle.text = this.data.ItemNameId;
			this.labelStoreId.text = this.data.ItemStoreId.ToString();
			this.labelPrice.text = this.data.ItemPrice.ToString();
			this._isInGame = this.data.IfInGameItem;



				this._itemImg.sprite = this.data.ItemImage.sprite;
				this._itemImg.color = Color.white;

				if(Factory.Get<DataManagerService>().DoesThisExist(data.ItemNameId))
                {

                    this.data.ItemPrice = 0;
					this.labelPrice.text = "Select";
					_unselect.SetActive(true);
					_select.SetActive(false);
				}
                
                if ( Factory.Get<DataManagerService>().CurrentCharacterSelected == int.Parse( this.labelStoreId.text ))
				{
					this.labelPrice.text = "Selected";
					_unselect.SetActive(false);
					_select.SetActive(true);
				}
                else
                {
                    _unselect.SetActive(true);
                    _select.SetActive(false);
                }
            Factory.Get<DataManagerService>().MessageBroker.Publish(new RefreshCoins());
		}

		public void UpdateData(ShopItemData data)
        {
			this.data = data;
		}


		public ShopItemData ItemData
        {
			get { return this.data; }
		}
        

        public void SelectThisItem()
        {
            SoundControls.Instance._buttonClick.Play();
            bool ifItemIsBough = Factory.Get<DataManagerService>().DoesThisExist(data.ItemNameId);
            bool canIAffordThis = Factory.Get<DataManagerService>().GameCoins >= data.ItemPrice;

            MessageBroker broker = Factory.Get<DataManagerService>().MessageBroker;
            if (ifItemIsBough)
            {
                Factory.Get<DataManagerService>().UpdateCurrentCharacter(data.ItemStoreId);
                broker.Publish(new RefereshAllItems());
            }
            else
            {
                if(canIAffordThis)
                {
                    broker.Publish(new SelectItem { ShopItem = data });
                }
                else
                {
                    broker.Publish(new InsufficientCoins());
                }
            }
        }
	}

}