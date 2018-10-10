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
using Sandbox.Background;
using Sandbox.FGCAutomation;
using uPromise;
using Sandbox.Preloader;

namespace Retroman
{
    public class GameRoot : Scene
    {


        public GameObject PauseResetButton;
        public GameControls _GameControls;
        MessageBroker _Broker;
        //--------

        protected override void Awake()
        {
            base.Awake();
            SetupButtons();

            bool LogBackbutton = true;

            _Broker = Factory.Get<DataManagerService>().MessageBroker;


            _Broker.Receive<AUTOMATE_TRIGGER>().Subscribe(_ =>
            {
                switch (_.AutomateType)
                {
                    case AutomateType.ResetGame:
                        GoToGame();
                        break;
                }
            }).AddTo(this);

            _Broker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if (_.BackButtonType == BackButtonType.SceneIsGame)
                {
                    PopupCollectionRoot popCol = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);
                    Debug.LogError("ActivePopup :: " + popCol.HasActivePopup());
                    Debug.LogError(popCol.CurrentPopup.ToPopup().Type);
                
                    if (popCol.IsLoaded(PopupType.ConvertOnline))
                    {
                        if (LogBackbutton)
                            Debug.LogError("Close Online Popup");
                        popCol.Hide();
                    }
                    else if (popCol.IsLoaded(PopupType.ConvertOffline))
                    {
                        if (LogBackbutton)
                            Debug.LogError("Close Offline Popup");
                        popCol.Hide();
                    }
                    else if (popCol.IsLoaded(PopupType.ConnectToFGC))
                    {
                        if (LogBackbutton)
                            Debug.LogError("Close ConnectFGC Popup");
                        popCol.Hide();
                    }
                    else if (popCol.IsLoaded(PopupType.ResultsPopup))//if ( ResultsCanvas.enabled)
                    {
                        if (LogBackbutton)
                            Debug.LogError("Go to Title Scene");
                        popCol.Hide();
                        GoToTitlebutton();
                    }
                    else if (_GameControls.CanAccessBackButton() == false)
                    {
                        if (LogBackbutton)
                            Debug.LogError("Cant Access");
                        return;
                    }
                    else
                    {
                        if (LogBackbutton)
                            Debug.LogError("Pause");
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
            _Broker.Publish(new TriggerCanvasInteraction());
            _Broker.Publish(new ToggleCoins { IfActive = false });
            SoundControls.Instance._buttonClick.Play();
            _Broker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
        }
        void SetupButtons()
        {
            AddButtonHandler(ButtonType.GoToTitle, delegate (ButtonClickedSignal signal)
            {

                        GoToTitlebutton();
            });
            AddButtonHandler(ButtonType.ResetGame, delegate (ButtonClickedSignal signal)
            {
                GoToGame();
            });
            AddButtonHandler(ButtonType.GoToShop, delegate (ButtonClickedSignal signal)
            {
                GoToShop();
            });
        }

        void GoToGame()
        {
            
            _Broker.Publish(new TriggerCanvasInteraction());

            SoundControls.Instance._buttonClick.Play();
            if (PauseResetButton)
                PauseResetButton.SetActive(false);
            _Broker.Publish(new ChangeScene { Scene = EScene.GameRoot });

            FGCTrackingMatchStart signal;
            this.Publish(signal);
        }
        void GoToShop()
        {
            _Broker.Publish(new ToggleCoins { IfActive = false });
            Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
            _Broker.Publish(new TriggerCanvasInteraction());
            SoundControls.Instance._buttonClick.Play();
            _Broker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
        }
    }
}