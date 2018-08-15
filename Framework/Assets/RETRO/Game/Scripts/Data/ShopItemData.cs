using System;
using System.Collections;
using UnityEngine.UI;

namespace Synergy88 {

	[Serializable]
	public class ShopItemData : IItemData {
		
		public string ItemNameId;
		public int ItemStoreId;
		public float ItemPrice;
		public Image ItemImage;
		public bool IfInGameItem;
	}

}