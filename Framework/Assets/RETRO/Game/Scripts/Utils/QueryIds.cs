using UnityEngine;
using System.Collections;

namespace Synergy88 { 
	
	public abstract class QueryIds {

		// scene
		public const string CurrentScene = "CurrentScene";
		public const string PreviousScene = "PreviousScene";

		// Facebook Login
		public const string HasLoggedInUser = "HasLoggedInUser";
		public const string UserEmail = "UserEmail";
		public const string UserFacebookId = "UserFacebookId";
		public const string UserFullName = "UserFullName";
		public const string UserProfilePhoto = "UserProfilePhoto";

		// IAP
		public const string StoreIsReady = "StoreIsReady";
		public const string StoreItems = "StoreItems";
		public const string StoreItemsWithType = "StoreItemsWithType";
		public const string StoreItemType = "StoreItemType";
		public const string PurchaseInProgress = "PurchaseInProgress";
	}

}