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
using Common.Utils;
using Sandbox.ButtonSandbox;
using Sandbox.Popup;

namespace Retroman
{
    public class GameRoot : Scene
    {


        public GameObject PauseResetButton;
        public GameControls _GameControls;
        MessageBroker msgBroker;
        //--------

        protected override void Awake()
        {
            base.Awake();
            SetupButtons();

            msgBroker = Factory.Get<DataManagerService>().MessageBroker;
            msgBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if (_.BackButtonType == BackButtonType.SceneIsGame)
                {
                    PopupCollectionRoot popCol = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);
                    if(popCol.IsLoaded(PopupType.ResultsPopup))//if ( ResultsCanvas.enabled)
                    {
                        GoToTitlebutton();
                    }
                    else if (_GameControls.CanAccessBackButton() == false)
                    {
                        return;
                    }
                    else
                    {

                        Factory.Get<DataManagerService>().MessageBroker.Publish(new TogglePause());
                    }
                }

            }).AddTo(this);
        }

        protected override void Start()
        {
            base.Start();
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
        public void GoToTitlebutton()
        {
            msgBroker.Publish(new TriggerCanvasInteraction());
            msgBroker.Publish(new ToggleCoins { IfActive = false });
            Debug.LogError("Toggle OFF!!");
            SoundControls.Instance._buttonClick.Play();
            msgBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
        }
        void SetupButtons()
        {
            AddButtonHandler(ButtonType.GoToTitle, delegate (ButtonClickedSignal signal)
            {

                Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
                        GoToTitlebutton();
            });
            AddButtonHandler(ButtonType.ResetGame, delegate (ButtonClickedSignal signal)
            {

                Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
                msgBroker.Publish(new TriggerCanvasInteraction());

                SoundControls.Instance._buttonClick.Play();
                if (PauseResetButton)
                    PauseResetButton.SetActive(false);
                msgBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
            AddButtonHandler(ButtonType.GoToShop, delegate (ButtonClickedSignal signal)
            {
                Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
                msgBroker.Publish(new TriggerCanvasInteraction());
                SoundControls.Instance._buttonClick.Play();
                msgBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
        }
    }
}