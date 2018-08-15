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
        private GameObject _InsufficientFunds;

        [SerializeField]
        private GameObject template;
        
        
        public Transform _ShopListParent;
        public ShopItemData SelectedItem;
        public List<ShopItem> _GeneratedItems;
        public UnityEngine.UI.Button PlayButton;
        void SetupListeners()
        {
            Debug.LogError("Listeners Yes");


            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if(_.BackButtonType == BackButtonType.SceneIsShop)
                {
                    if (_InsufficientFunds.activeSelf)
                    {
                        _InsufficientFunds.SetActive(false);
                    }
                    else if(_confirmationWindow.active)
                    {
                        _confirmationWindow.SetActive(false);

                    }
                    else
                    {
                        GoBack();
                    }
                }
            }).AddTo(this);



            Factory.Get<DataManagerService>().MessageBroker.Receive<RefreshShopItems>().Subscribe(_ => 
            {
                RefreshGeneratedItems();
            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<InsufficientCoins>().Subscribe(_ =>
            {
                _InsufficientFunds.SetActive(true);
            }).AddTo(this);
            
            Factory.Get<DataManagerService>().MessageBroker.Receive<SelectItem>().Subscribe(_ =>
            {
                SelectedItem = _.ShopItem;

                ActivateConrimationWindow();

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

        public void GoBack()
        {
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });
            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });

        }

        void SetupButtons()
        {
            AddButtonHandler(EButton.GoToTitle, delegate (ButtonClickedSignal signal)
            {
                Debug.LogError("Title Button");
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
            });

            AddButtonHandler(EButton.StartGame, delegate (ButtonClickedSignal signal)
            {
                PlayButton.interactable = false;
                SoundControls.Instance._buttonClick.Play();
                Debug.LogError("STart GAme Button");
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });

            AddButtonHandler(EButton.Back, (ButtonClickedSignal signal) =>
            {
                GoBack();
            });
        }

        public void PopulateItems()
        {
            Transform parent = this.template.transform.parent;
            Vector3 localScale = this.template.transform.localScale;
            int len = Factory.Get<DataManagerService>().ShopItems.Count;
            int children = parent.childCount - 1; ;
            bool create = len == children;

            for (int i = 0; i < len; i++)
            {
                ShopItemData item = null;
                GameObject itemObject = null;
                ShopItem gameItem = null;

                try
                {
                    item = Factory.Get<DataManagerService>().ShopItems[i];
                    itemObject = parent.GetChild(i + 1).gameObject;
                    gameItem = itemObject.GetComponent<ShopItem>();
                }
                catch (UnityException e)
                {
                    item = Factory.Get<DataManagerService>().ShopItems[i];
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


            Factory.Get<DataManagerService>().MessageBroker.Publish(new AddCoin { CoinsToAdd = -(SelectedItem.ItemPrice) });

            Factory.Get<DataManagerService>().SaveThisItem(SelectedItem.ItemNameId);
            Factory.Get<DataManagerService>().UpdateCurrentCharacter(SelectedItem.ItemStoreId);

            RefreshGeneratedItems();
            SoundControls.Instance._buttonClick.Play();
        }
        public void CloseInsufficientPanel()
        {
            SoundControls.Instance._buttonClick.Play();
            _InsufficientFunds.SetActive(false);
        }


    }
}