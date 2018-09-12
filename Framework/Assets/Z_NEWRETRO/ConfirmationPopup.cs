using Common.Utils;
using Framework;
using Retroman;
using Sandbox.Popup;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Retroman
{
    public class ConfirmationPopup : MonoBehaviour
    {



        public void YesButton()
        {
            Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();

            ShopItemData itemSelected = Scene.GetScene<ShopRoot>(EScene.ShopRoot).SelectedItem;
            Factory.Get<DataManagerService>().MessageBroker.Publish(new AddCoin { CoinsToAdd = -(itemSelected.ItemPrice) });

            Factory.Get<DataManagerService>().SaveThisItem(itemSelected.ItemNameId);
            Factory.Get<DataManagerService>().UpdateCurrentCharacter(itemSelected.ItemStoreId);

            Factory.Get<DataManagerService>().MessageBroker.Publish(new RefreshShopItems());
            SoundControls.Instance._buttonClick.Play();
        }
        public void NoButton()
        {
            Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
            SoundControls.Instance._buttonClick.Play();
        }
    }
}