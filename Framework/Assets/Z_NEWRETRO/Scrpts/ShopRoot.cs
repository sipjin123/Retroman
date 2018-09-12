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
using Sandbox.ButtonSandbox;
using Sandbox.Popup;

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


        bool _DisableBackButton;
        public CanvasGroup InteractiveCanvas;


        

        [SerializeField]
        private GameObject template;
        
        
        public Transform _ShopListParent;
        public ShopItemData _SelectedItem;
        public ShopItemData SelectedItem { get { return _SelectedItem; } }
        public List<ShopItem> _GeneratedItems;
        public UnityEngine.UI.Button PlayButton;


        PopupCollectionRoot popCol;

        void SetupListeners()
        {
            Debug.LogError("Listeners Yes");

            popCol = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);
            

            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if(_.BackButtonType == BackButtonType.SceneIsShop)
                {
                    if(popCol.IsLoaded(PopupType.InsufficientGoldPopup))//if (_InsufficientFunds.activeSelf)
                    {
                        popCol.Hide();//_InsufficientFunds.SetActive(false);
                    }
                    else if(popCol.IsLoaded(PopupType.PurchaseConfirmationPopup))//(_confirmationWindow.active)
                    {
                        //_confirmationWindow.SetActive(false);
                        popCol.Hide();
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
                popCol.Show(PopupType.InsufficientGoldPopup);
                //_InsufficientFunds.SetActive(true);
            }).AddTo(this);
            
            Factory.Get<DataManagerService>().MessageBroker.Receive<SelectItem>().Subscribe(_ =>
            {
                _SelectedItem = _.ShopItem;

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


        void SetupButtons()
        {
            AddButtonHandler(ButtonType.GoToTitle, delegate (ButtonClickedSignal signal)
            {
                Debug.LogError("Title Button");
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
            });

            AddButtonHandler(ButtonType.StartGame, delegate (ButtonClickedSignal signal)
            {
                PlayButton.interactable = false;
                SoundControls.Instance._buttonClick.Play();
                Debug.LogError("STart GAme Button");
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });

            AddButtonHandler(ButtonType.Back, (ButtonClickedSignal signal) =>
            {
                GoBack();
            });
        }
        public void GoBack()
        {
            if (_DisableBackButton)
            {
                Debug.LogError(D.ERROR + " SPAM IS BLOCKED BY SHOP");
                return;
            }
            _DisableBackButton = true;
            InteractiveCanvas.interactable = false;

            Debug.LogError(D.LOG + " Going Back ");
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });
            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });

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
            popCol.Show(PopupType.PurchaseConfirmationPopup);
            //_confirmationWindow.SetActive(true);
        }


    }
}