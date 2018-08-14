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


        public GameObject _WAtchVidButton;

        public Text HiScore1, HiScore2, CScore1, CScore2;

        public GameObject GameBlocker;
        public GameObject PauseResetButton;
        public Canvas ResultsCanvas;
        //--------
        public Image CharImage;
        void ShowResults()
        {
            ResultsCanvas.enabled = true;
            //Factory.Get<DataManagerService>().GameControls._resultCharParent.SetActive(true);
            Debug.LogError("Repalced to UI");

            CScore1.text = "" + PlayerPrefs.GetInt("curSkor", 0);
            CScore2.text = "" + PlayerPrefs.GetInt("curSkor", 0);
            HiScore1.text = "Best Score " + PlayerPrefs.GetInt("hiSkor", 0);
            HiScore2.text = "Best Score " + PlayerPrefs.GetInt("hiSkor", 0);

            int currChar = Factory.Get<DataManagerService>().CurrentCharacterSelected -1;
            CharImage.sprite = Factory.Get<DataManagerService>().ShopItems[currChar].ItemImage.sprite;
        }



        //--------
        public GameObject _ToSpawnGameStartupScene;
        public GameObject _CurrentGameStartupScene;
        protected override void Awake()
        {
            base.Awake();
            StartupGame();
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
        public Canvas SpamBlocker;
        public void GoToTitlebutton()
        {
            SpamBlocker.enabled = true;
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
                SpamBlocker.enabled = true;
                SoundControls.Instance._buttonClick.Play();
                GameBlocker.SetActive(true);
                if(PauseResetButton)
                PauseResetButton.SetActive(false);
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
            AddButtonHandler(EButton.GoToShop, delegate (ButtonClickedSignal signal)
            {
                SpamBlocker.enabled = true;
                SoundControls.Instance._buttonClick.Play();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
        }
        private void StartupGame()
        {
            Destroy(_CurrentGameStartupScene);
            GameObject temp = Instantiate(_ToSpawnGameStartupScene, transform.position, Quaternion.identity) as GameObject;
            _CurrentGameStartupScene = temp;
            temp.transform.parent = transform;

           // Factory.Get<DataManagerService>().MessageBroker.Publish(new LaunchGamePlay());
        }
    }
}