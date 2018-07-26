using System;
using System.Collections;
using UnityEngine.UI;

namespace Synergy88 {

	[Serializable]
	public class ShopItemData : IItemData {
		
		public string ItemId;
		public string ItemStoreId;
		public string ItemPrice;
		public Image ItemImage;
		public bool IfInGameItem;
	}

}