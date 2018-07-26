using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Common;
using Common.Query;

using Framework;
using Synergy88;
using Common.Utils;
namespace Retroman
{
    public class ShopRoot : Scene
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            PopulateItems();
            SetupListeners();
            SetupButtons();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        public GameObject _confirmationWindow;
        public Text _testTExt;

        [SerializeField]
        private GameObject template;

        [SerializeField]
        private GameObject ItemImages;

        [SerializeField]
        private List<ShopItemData> items;
        public Transform _ShopListParent;
        public ShopItemData SelectedItem;
        public List<ShopItem> _GeneratedItems;

        void SetupListeners()
        {
            Debug.LogError("Listeners Yes");
            Factory.Get<DataManagerService>().MessageBroker.Receive<RefreshShopItems>().Subscribe(_ => 
            {
                RefreshGeneratedItems();
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<SelectItem>().Subscribe(_ =>
            {
                Debug.LogError("Item Selected");
                SelectedItem = _.ShopItem;

                if (PlayerPrefs.GetInt("Bought" + SelectedItem.ItemId, 0) == 1)
                {
                    PlayerPrefs.SetInt(DataManagerService.CurrentCharacterSelected, int.Parse(SelectedItem.ItemStoreId));

                    Debug.LogError("I Bought this already");
                }
                else
                {
                    Debug.LogError("I want to buy this");
                    ActivateConrimationWindow();
                }

                RefreshGeneratedItems();
            }).AddTo(this);
        }


        void RefreshGeneratedItems()
        {
            int generatedItemList = _GeneratedItems.Count;
            for (int i = 0; i < generatedItemList; i++)
            {
                //if(_GeneratedItems[i].ItemData.ItemId != SelectedItem.ItemId)
                _GeneratedItems[i].Refresh();
            }
        }


        void SetupButtons()
        {
            AddButtonHandler(EButton.GoToTitle, delegate (ButtonClickedSignal signal)
            {
                Debug.LogError("Title Button");
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
            });
            AddButtonHandler(EButton.StartGame, delegate (ButtonClickedSignal signal)
            {
                Debug.LogError("STart GAme Button");
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
        }

        public void PopulateItems()
        {
            Transform parent = this.template.transform.parent;
            Vector3 localScale = this.template.transform.localScale;
            int len = this.items.Count;
            int children = parent.childCount - 1; ;
            bool create = len == children;

            for (int i = 0; i < len; i++)
            {
                ShopItemData item = null;
                GameObject itemObject = null;
                ShopItem gameItem = null;

                try
                {
                    item = this.items[i];
                    itemObject = parent.GetChild(i + 1).gameObject;
                    gameItem = itemObject.GetComponent<ShopItem>();
                }
                catch (UnityException e)
                {
                    item = this.items[i];
                    itemObject = (GameObject)GameObject.Instantiate(this.template);
                    gameItem = itemObject.GetComponent<ShopItem>();

                }

                gameItem.UpdateData(item);

                // display item
                itemObject.transform.SetParent(_ShopListParent);
                itemObject.transform.localScale = localScale;
                itemObject.SetActive(true);

                _GeneratedItems.Add(gameItem);
            }
        }





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
            DisableConrimationWindow();
            Debug.LogError("Buying :: " + float.Parse(SelectedItem.ItemPrice));
            Debug.LogError("My Current Coins are :: " + Factory.Get<DataManagerService>().GetTotalCoins());


            if ( Factory.Get<DataManagerService>().GetTotalCoins() >=  float.Parse(SelectedItem.ItemPrice) )
            {
                Debug.LogError("Success Buy");
                Factory.Get<DataManagerService>().MessageBroker.Publish(new AddCoin { CoinsToAdd = -float.Parse(SelectedItem.ItemPrice) });

                PlayerPrefs.SetInt("Bought" +SelectedItem.ItemId,1);
    

                PlayerPrefs.SetInt(DataManagerService.CurrentCharacterSelected, int.Parse(SelectedItem.ItemStoreId));

                RefreshGeneratedItems();
            }
            else
            {
                Debug.LogError("You are Poor");
            }
            SoundControls.Instance._buttonClick.Play();
        }


    }
}