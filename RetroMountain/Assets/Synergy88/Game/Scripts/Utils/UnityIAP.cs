using UnityEngine;
using UnityEngine.Purchasing;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Query;
using Common.Signal;

// alias
using StoreProduct = System.String;
using StoreName = System.String;

namespace Synergy88 {
	
	public class UnityIAP : MonoBehaviour, IStoreListener {

		public const string STORE_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2O/9/H7jYjOsLFT/uSy3ZEk5KaNg1xx60RN7yWJaoQZ7qMeLy4hsVB3IpgMXgiYFiKELkBaUEkObiPDlCxcHnWVlhnzJBvTfeCPrYNVOOSJFZrXdotp5L0iS2NVHjnllM+HA1M0W2eSNjdYzdLmZl1bxTpXa4th+dVli9lZu7B7C2ly79i/hGTmvaClzPBNyX+Rtj7Bmo336zh2lYbRdpD5glozUq+10u91PMDPH+jqhx10eyZpiapr8dFqXl5diMiobknw9CgcjxqMTVBQHK6hS0qYKPmUDONquJn280fBs1PTeA6NMG03gb9FLESKFclcuEZtvM8ZwMMRxSLA9GwIDAQAB";

		/*
		// products
		public static readonly StoreProduct[] STORE_PRODUCTS = new StoreProduct[] {
			"StoreItem001",
			"StoreItem002",
			"StoreItem003",
			"StoreItem004",
			"StoreItem005",
			"StoreItem006",

			"CoinsItem001",
			"CoinsItem002",
			"CoinsItem003",

			"SubscriptionItem001",
		};

		public static readonly Dictionary<StoreProduct, ProductType> STORE_PRODUCT_TYPE = new Dictionary<StoreProduct, ProductType>() {
			{ STORE_PRODUCTS[0], ProductType.NonConsumable },
			{ STORE_PRODUCTS[1], ProductType.NonConsumable },
			{ STORE_PRODUCTS[2], ProductType.NonConsumable },
			{ STORE_PRODUCTS[3], ProductType.NonConsumable },
			{ STORE_PRODUCTS[4], ProductType.NonConsumable },
			{ STORE_PRODUCTS[5], ProductType.NonConsumable },

			{ STORE_PRODUCTS[6], ProductType.Consumable },
			{ STORE_PRODUCTS[7], ProductType.Consumable },
			{ STORE_PRODUCTS[8], ProductType.Consumable },

			{ STORE_PRODUCTS[9], ProductType.Subscription },
		};

		// TODO: Create an editor for this
		public static readonly Dictionary<StoreProduct, Dictionary<StoreName, string>> STORE_PRODUCT_NAMES = new Dictionary<StoreProduct, Dictionary<StoreName, string>>() {
			// non consumables
			{ 
				STORE_PRODUCTS[0], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem001.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem001.AAS" },
				}
			},

			{
				STORE_PRODUCTS[1], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem002.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem002.AAS" },
				}
			},

			{
				STORE_PRODUCTS[2], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem003.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem003.AAS" },
				}
			},

			{
				STORE_PRODUCTS[3], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem004.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem004.AAS" },
				}
			},

			{
				STORE_PRODUCTS[4], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem005.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem005.AAS" },
				}
			},

			{
				STORE_PRODUCTS[5], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.StoreItem006.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.StoreItem006.AAS" },
				}
			},

			// consumables
			{
				STORE_PRODUCTS[6], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.CoinsItem001.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.CoinsItem001.AAS" },
				}
			},

			{
				STORE_PRODUCTS[7], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.CoinsItem002.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.CoinsItem002.AAS" },
				}
			},

			{
				STORE_PRODUCTS[8], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.CoinsItem003.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.CoinsItem003.AAS" },
				}
			},

			// subscriptions
			{
				STORE_PRODUCTS[9], new Dictionary<StoreName, string>() {
					{ GooglePlay.Name, 		"com.unity3d.unityiap.unityiapdemo.SubscriptionItem001.GP" },
					{ AppleAppStore.Name, 	"com.unity3d.unityiap.unityiapdemo.SubscriptionItem001.AAS" },
				}
			},
		};
		*/

		// Unity IAP objects 
		private IStoreController storeController;
		private IAppleExtensions appExtension;

		// cached products
		[SerializeField]
		private List<Product> products;
		[SerializeField]
		private List<Product> consumableProducts;
		[SerializeField]
		private List<Product> nonConsumableProducts;
		[SerializeField]
		private List<Product> subscriptionProducts;

		[SerializeField]
		private bool storeIsReady = false;

		[SerializeField]
		private bool purchaseInProgress = false;

		[SerializeField]
		private List<ProductItem> Products;

		private void Awake() {
			var module = StandardPurchasingModule.Instance();

			// Microsoft
			module.useMockBillingSystem = true; 

			// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
			// developer ui (initialization, purchase, failure code setting). These correspond to 
			// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
			module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

			// create the builder
			var builder = ConfigurationBuilder.Instance(module);
			builder.Configure<IGooglePlayConfiguration>().SetPublicKey(STORE_PUBLIC_KEY);

			// add products (manual)
			/*
			IDs coins = new IDs();
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[0]][GooglePlay.Name], GooglePlay.Name);
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[0]][AppleAppStore.Name], AppleAppStore.Name);
			builder.AddProduct(STORE_PRODUCTS[0], ProductType.Consumable, coins);

			IDs swords = new IDs();
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[1]][GooglePlay.Name], GooglePlay.Name);
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[1]][AppleAppStore.Name], AppleAppStore.Name);
			builder.AddProduct(STORE_PRODUCTS[1], ProductType.Consumable, swords);

			IDs descriptions = new IDs();
			//coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[2]][GooglePlay.Name], GooglePlay.Name, AppleAppStore.Name);
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[2]][GooglePlay.Name], GooglePlay.Name);
			coins.Add(STORE_PRODUCT_NAMES[STORE_PRODUCTS[2]][AppleAppStore.Name], AppleAppStore.Name);
			builder.AddProduct(STORE_PRODUCTS[2], ProductType.Consumable, descriptions);
			*/

			// add products (loop)
			/*
			int count = STORE_PRODUCTS.Length;
			for (int i = 0; i < count; i++) {
				StoreProduct product = STORE_PRODUCTS[i];
				Dictionary<StoreName, string> storeIds = STORE_PRODUCT_NAMES[product];
				IDs productIds = new IDs();
				productIds.Add(storeIds[GooglePlay.Name], GooglePlay.Name);
				productIds.Add(storeIds[AppleAppStore.Name], AppleAppStore.Name);
				builder.AddProduct(product, STORE_PRODUCT_TYPE[product], productIds);
			}
			*/

			int count = this.Products.Count;
			for (int i = 0; i < count; i++) {
				ProductItem product = this.Products[i];
				IDs productIds = new IDs();
				productIds.Add(product.ProductIdAndroid, GooglePlay.Name);
				productIds.Add(product.ProductIdIOS, AppleAppStore.Name);
				builder.AddProduct(product.Product, product.Type, productIds);
			}

			// Now we're ready to initialize Unity IAP.
			UnityPurchasing.Initialize(this, builder);

			// setup queries
			QuerySystem.RegisterResolver(QueryIds.StoreIsReady, delegate(IQueryRequest request, IMutableQueryResult result) {
				result.Set(this.storeIsReady);
			});

			QuerySystem.RegisterResolver(QueryIds.StoreItems, delegate(IQueryRequest request, IMutableQueryResult result) {
				result.Set(this.products);
			});

			QuerySystem.RegisterResolver(QueryIds.StoreItemsWithType, delegate(IQueryRequest request, IMutableQueryResult result) {
				ProductType type = (ProductType)request.GetParameter(QueryIds.StoreItemType);
				if (type == ProductType.Consumable) {
					result.Set(this.consumableProducts);
				}
				else if (type == ProductType.NonConsumable) {
					result.Set(this.nonConsumableProducts);
				}
				else if (type == ProductType.Subscription) {
					result.Set(this.subscriptionProducts);
				}
			});

			QuerySystem.RegisterResolver(QueryIds.PurchaseInProgress, delegate(IQueryRequest request, IMutableQueryResult result) {
				result.Set(this.purchaseInProgress);
			});
		}

		private void Start() {
			S88Signals.ON_STORE_ITEM_PURCHASE.AddListener(this.OnStoreItemPurchase);
			S88Signals.ON_RESTORE_PURCHASE.AddListener(this.OnRestorePurchase);
		}

		private void OnDestroy() {
			QuerySystem.RemoveResolver(QueryIds.StoreIsReady);
			QuerySystem.RemoveResolver(QueryIds.StoreItems);
			QuerySystem.RemoveResolver(QueryIds.PurchaseInProgress);

			S88Signals.ON_STORE_ITEM_PURCHASE.RemoveListener(this.OnStoreItemPurchase);
			S88Signals.ON_RESTORE_PURCHASE.RemoveListener(this.OnRestorePurchase);
		}

		private IEnumerable<Product> StoreItems {
			get {
				return this.products.ToArray();
			}
		}

		private IEnumerable<Product> StoreItemConsumables {
			get {
				return this.consumableProducts.ToArray();
			}
		}

		private IEnumerable<Product> StoreItemNonConsumables {
			get {
				return this.nonConsumableProducts.ToArray();
			}
		}

		private IEnumerable<Product> StoreItemSubscriptions {
			get {
				return this.subscriptionProducts.ToArray();
			}
		}

		#region Signals

		private void OnStoreItemPurchase(ISignalParameters parameters) {
			// block when has an inprogress purchase
			if (this.purchaseInProgress) {
				return;
			}

			string itemId = (string)parameters.GetParameter(S88Params.STORE_ITEM_ID);
			//Product product = this.products.Find(p => p.definition.id.Equals(itemId));
			//this.storeController.InitiatePurchase(product);
			this.storeController.InitiatePurchase(itemId);
			this.purchaseInProgress = true;
		}

		private void OnRestorePurchase(ISignalParameters parameters) {
			this.appExtension.RestoreTransactions(OnPurchaseRestored);
		}

		#endregion

		#region IStoreListener Methods

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
			this.storeController = controller;
			this.appExtension = extensions.GetExtension<IAppleExtensions>();

			// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
			// On non-Apple platforms this will have no effect; OnDeferred will never be called.
			this.appExtension.RegisterPurchaseDeferredListener(OnDeferred);

			// cache the products
			this.products = new List<Product>(this.storeController.products.all);
			this.consumableProducts = this.products.FindAll(p => p.definition.type == ProductType.Consumable);
			this.nonConsumableProducts = this.products.FindAll(p => p.definition.type == ProductType.NonConsumable);
			this.subscriptionProducts = this.products.FindAll(p => p.definition.type == ProductType.Subscription);

			/*
			Debug.Log("Available items:\n");
			foreach (var item in controller.products.all) {
				if (item.availableToPurchase) {
					Debug.Log(string.Join(" - ",
						new[] {
							item.metadata.localizedTitle,
							item.metadata.localizedDescription,
							item.metadata.isoCurrencyCode,
							item.metadata.localizedPrice.ToString(),
							item.metadata.localizedPriceString
						}) + "\n");
				}
			}
			*/

			// set store is ready
			this.storeIsReady = true;
		}

		public void OnInitializeFailed(InitializationFailureReason error) {
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
			Debug.LogErrorFormat("UnityIAP::OnPurchaseFailed Product:{0} Receipt:{1} Reason:{2}\n", i.definition.id, i.receipt, p.ToString());
			this.purchaseInProgress = false;

			Signal signal = S88Signals.ON_STORE_ITEM_PURCHASE_FAILED;
			signal.ClearParameters();
			signal.Dispatch();
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
			Debug.LogFormat("UnityIAP::ProcessPurchase Product:{0} Receipt:{1}\n", e.purchasedProduct.definition.id, e.purchasedProduct.receipt);
			this.purchaseInProgress = false;

			Signal signal = S88Signals.ON_STORE_ITEM_PURCHASE_SUCCESSFUL;
			signal.ClearParameters();
			signal.AddParameter(S88Params.STORE_ITEM, e.purchasedProduct);
			signal.Dispatch();

			// indicate we have handled this purchase, we will not be informed of it again.x
			return PurchaseProcessingResult.Complete;
		}
		
		#endregion

		#region Store Callbacks

		/// <summary>
		/// iOS Specific.
		/// This is called as part of Apple's 'Ask to buy' functionality,
		/// when a purchase is requested by a minor and referred to a parent
		/// for approval.
		/// 
		/// When the purchase is approved or rejected, the normal purchase events
		/// will fire.
		/// </summary>
		/// <param name="item">Item.</param>
		private void OnDeferred(Product item) {
			Debug.LogFormat("UnityIAP::OnDeferred Purchase deferred:{0}", item.definition.id);
		}

		private void OnPurchaseRestored(bool success) {
			Debug.LogFormat("UnityIAP::OnTransactionsRestored Result:{0}\n", success);
		}

		#endregion
	}

	[Serializable]
	public class ProductItem {
		public StoreProduct Product;
		public ProductType Type;
		public string ProductIdIOS;
		public string ProductIdAndroid;
	}
}