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
    public class GameRoot : Scene
    {

        public CanvasGroup InteractiveCanvas;

        public Text HiScore1, HiScore2, CScore1, CScore2;

        public GameObject GameBlocker;
        public GameObject PauseResetButton;
        public Canvas ResultsCanvas;
        public GameControls _GameControls;
        //--------
        public Image CharImage;
        void ShowResults()
        {
            ResultsCanvas.enabled = true;
            
            CScore1.text = "" + Factory.Get<DataManagerService>().GetScore();
            CScore2.text = "" + Factory.Get<DataManagerService>().GetScore();
            HiScore1.text = "Best Score " + Factory.Get<DataManagerService>().GetHighScore();
            HiScore2.text = "Best Score " + Factory.Get<DataManagerService>().GetHighScore();

            int currChar = Factory.Get<DataManagerService>().CurrentCharacterSelected -1;
            CharImage.sprite = Factory.Get<DataManagerService>().ShopItems[currChar].ItemImage.sprite;
        }


        protected override void Awake()
        {
            base.Awake();
            SetupButtons();

            
            Factory.Get<DataManagerService>().MessageBroker.Receive<EndGame>().Subscribe(_ =>
            {
                ShowResults();
            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if (_.BackButtonType == BackButtonType.SceneIsGame)
                {
                    
                    if (ResultsCanvas.enabled)
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
            InteractiveCanvas.interactable = false;
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });
            Debug.LogError("Toggle OFF!!");
            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
        }
        void SetupButtons()
        {
            AddButtonHandler(EButton.GoToTitle, delegate (ButtonClickedSignal signal)
            {
                GoToTitlebutton();
            });
            AddButtonHandler(EButton.ResetGame, delegate (ButtonClickedSignal signal)
            {
                InteractiveCanvas.interactable = false;
                SoundControls.Instance._buttonClick.Play();
                GameBlocker.SetActive(true);
                if(PauseResetButton)
                PauseResetButton.SetActive(false);
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
            AddButtonHandler(EButton.GoToShop, delegate (ButtonClickedSignal signal)
            {
                InteractiveCanvas.interactable = false;
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
        }
    }
}