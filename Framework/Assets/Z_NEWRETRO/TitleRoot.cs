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

namespace Retroman
{
    public class TitleRoot : Scene
    {
        public Canvas MainCanvas;
        public Canvas _TutorialCanvas;
        public GameObject ExitGamePanel;
        public Canvas _SpamBlockerCanvas;

        MessageBroker _MessageBroker;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _MessageBroker = Factory.Get<DataManagerService>().MessageBroker;
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
            _MessageBroker.Receive<PressBackButton>().Subscribe(_ => 
            {
                if (ifCoolDownSpam == true)
                {
                    Debug.LogError(D.B + "Do Not Spam");
                    return;
                }

                if (_.BackButtonType == BackButtonType.SceneIsTitle)
                {
                    if (_TutorialCanvas.enabled == true)
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

            _MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ => { MainCanvas.enabled = false; }).AddTo(this);
        }

        void ButtonSetup()
        {
            AddButtonHandler(EButton.StartGame, delegate (ButtonClickedSignal signal)
            {
                GenericButtonPressed();
                _MessageBroker.Publish(new LaunchGamePlay());
            });
            AddButtonHandler(EButton.GoToShop, delegate (ButtonClickedSignal signal)
            {
                GenericButtonPressed();
                _MessageBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
            AddButtonHandler(EButton.TutorialButton, delegate (ButtonClickedSignal signal)
            {
                GenericButtonPressed();
                _MessageBroker.Publish(new ToggleCoins { IfActive = false });
                _TutorialCanvas.enabled = (true);
            });
            AddButtonHandler(EButton.SettingsButton, delegate (ButtonClickedSignal signal)
            {
                GenericButtonPressed();
                
                _MessageBroker.Publish(new ChangeScene { Scene = EScene.SettingsRoot });
                //_MessageBroker.Publish(new ToggleSetting { IfActive = true });
            });
        }

        void GenericButtonPressed()
        {
            if (ifCoolDownSpam == false)
            {
                StartCoroutine(DelaySpam());
            }
            SoundControls.Instance._buttonClick.Play();
        }

        bool ifCoolDownSpam;
        IEnumerator DelaySpam()
        {
            _SpamBlockerCanvas.enabled = true;
            ifCoolDownSpam = true;
            yield return new WaitForSeconds(1);
            ifCoolDownSpam = false;
            _SpamBlockerCanvas.enabled = false;
        }

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
            ExitGamePanel.SetActive(false);
        }
        #endregion
    }
}