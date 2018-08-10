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

namespace Retroman
{
    public class TitleRoot : Scene
    {
        public Canvas MainCanvas;
        public GameObject TutorialPanel;
        public GameObject ExitGamePanel;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            ButtonSetup();
            InitializeSignals();
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

        void InitializeSignals()
        {
            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ => 
            {
                if (_.BackButtonType == BackButtonType.SceneIsTitle)
                {
                    if (QuerySystem.Query<bool>(QueryIds.IF_SETTINGS_ACTIVE) == true)
                    {
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleSetting { IfActive = false });
                    }
                    else if (TutorialPanel.activeSelf == true)
                    {
                        CloseTutorial();
                    }
                    else
                    {
                        if (ExitGamePanel.activeSelf == true)
                        {
                            ExitGamePanel.SetActive(false);
                        }
                        else
                        {
                            ExitGamePanel.SetActive(true);
                        }
                    }
                }
            }).AddTo(this);


            Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ => { MainCanvas.enabled = false; }).AddTo(this);
        }

        void ButtonSetup()
        {
            AddButtonHandler(EButton.StartGame, delegate (ButtonClickedSignal signal)
            {
                Factory.Get<DataManagerService>().MessageBroker.Publish(new LaunchGamePlay());
                SoundControls.Instance._buttonClick.Play();
                //Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
            AddButtonHandler(EButton.GoToShop, delegate (ButtonClickedSignal signal)
            {
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
            AddButtonHandler(EButton.TutorialButton, delegate (ButtonClickedSignal signal)
            {

                Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });
                SoundControls.Instance._buttonClick.Play();
                TutorialPanel.SetActive(true);
            });
            AddButtonHandler(EButton.SettingsButton, delegate (ButtonClickedSignal signal)
            {
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleSetting { IfActive = true });
            });
        }
        public void CloseTutorial()
        {

            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = true });
            SoundControls.Instance._buttonClick.Play();
            TutorialPanel.SetActive(false);
        }
        public void ExitGameYes()
        {
            SoundControls.Instance._buttonClick.Play();
            Application.Quit();
        }
        public void ExitGameNo()
        {
            SoundControls.Instance._buttonClick.Play();
            ExitGamePanel.SetActive(false);
        }
    }
}