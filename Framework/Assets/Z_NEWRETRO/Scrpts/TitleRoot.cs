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
using System.Collections;
using System.Threading.Tasks;
using Sandbox.ButtonSandbox;
using Sandbox.Facebook;
using Sandbox.FGCAutomation;
using Sandbox.Popup;
using Sandbox.RGC;

namespace Retroman
{
    public class TitleRoot : Scene
    {
        public Canvas MainCanvas;
        public Canvas _TutorialCanvas;
        public Canvas _ExitGameCanvas;

        MessageBroker _Broker;
        public CanvasGroup _InteractiveCanvas;

        private static bool HasAttemptedFGCLogin;
        //=====================================================================
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _Broker = Factory.Get<DataManagerService>().MessageBroker;
            ButtonSetup();
            InitializeSignals();
            _Broker.Publish(new AutomatedUIState { Scene = EScene.TitleRoot });

            if (!HasAttemptedFGCLogin)
            {
                OnConnectToFGCApp signal;
                this.Publish(signal);
                HasAttemptedFGCLogin = true;
                this.Receive<OnFacebookLoginFailedSignal>()
                    .Subscribe(_ =>
                    {
                        Debug.LogError($"{D.ERROR} Facebook Login failed");
                        CloseAllPopups();
                        
                    }).AddTo(this);
            }
        }

        private async void CloseAllPopups()
        {
            await DelayAction(2f, () =>
            {
                OnCloseActivePopup closeSignal;
                closeSignal.All = true;
                this.Publish(closeSignal);
            });
        }

        private async Task DelayAction(float delay, Action action)
        {
            await new TimeSpan(0, 0, 0, (int)delay);
            action?.Invoke();
        }
        //=====================================================================
        void InitializeSignals()
        {
            _Broker.Receive<AUTOMATE_TRIGGER>().Subscribe(_ =>
            {
                switch(_.AutomateType)
                {
                    case AutomateType.GoToGame:
                        GoToGame();
                        break;
                }
            }).AddTo(this);




            _Broker.Receive<PressBackButton>().Subscribe(_ => 
            {

                if (_.BackButtonType == BackButtonType.SceneIsTitle)
                {
                    if (_TutorialCanvas.enabled == true)
                    {
                        CloseTutorial();
                    }
                    else
                    {
                        if (_ExitGameCanvas.enabled == true)
                        {
                            _ExitGameCanvas.enabled = (false);
                        }
                        else
                        {
                            _ExitGameCanvas.enabled = (true);
                        }
                    }
                }
            }).AddTo(this);
            //Removed for title animation outro - hec
           // _MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ => { MainCanvas.enabled = false; }).AddTo(this);
        }

        //=====================================================================
        #region SCENEFLOW
        void GoToGame()
        {
            _InteractiveCanvas.interactable = false;
            SoundControls.Instance._buttonClick.Play();
            _Broker.Publish(new LaunchGamePlay());

            FGCTrackingMatchStart signal;
            this.Publish(signal);
        }
        void GoToSettings()
        {
            _InteractiveCanvas.interactable = false;
            SoundControls.Instance._buttonClick.Play();

            _Broker.Publish(new ChangeScene { Scene = EScene.SettingsRoot });
            //_MessageBroker.Publish(new ToggleSetting { IfActive = true });
        }
        void GoToShop()
        {
            _InteractiveCanvas.interactable = false;
            SoundControls.Instance._buttonClick.Play();
            _Broker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
        }
        #endregion
        //=====================================================================
        #region BUTTONS CLICKS
        public void CloseTutorial()
        {
            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = true });
            _TutorialCanvas.enabled = (false);
        }
        public void ExitGameYes()
        {
            SoundControls.Instance._buttonClick.Play();
            Application.Quit();
        }
        public void ExitGameNo()
        {
            SoundControls.Instance._buttonClick.Play();
            _ExitGameCanvas.enabled = (false);
        }
        #endregion
        //=====================================================================
        #region BUTTON SETUP
        void ButtonSetup()
        {
            AddButtonHandler(ButtonType.StartGame, delegate (ButtonClickedSignal signal)
            {
                GoToGame();
            });
            AddButtonHandler(ButtonType.GoToShop, delegate (ButtonClickedSignal signal)
            {
                GoToShop();
            });
            AddButtonHandler(ButtonType.TutorialButton, delegate (ButtonClickedSignal signal)
            {
                SoundControls.Instance._buttonClick.Play();
                _Broker.Publish(new ToggleCoins { IfActive = false });
                _TutorialCanvas.enabled = (true);
            });
            AddButtonHandler(ButtonType.SettingsButton, delegate (ButtonClickedSignal signal)
            {
                GoToSettings();
            });
        }
        #endregion
        //=====================================================================
    }
}