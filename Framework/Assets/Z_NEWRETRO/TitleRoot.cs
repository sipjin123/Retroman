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

            Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ => { MainCanvas.enabled = false; }).AddTo(this);
        }

        void ButtonSetup()
        {
            AddButtonHandler(EButton.StartGame, delegate (ButtonClickedSignal signal)
            {
                Factory.Get<DataManagerService>().MessageBroker.Publish(new LaunchGamePlay());
                //Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.GameRoot });
            });
            AddButtonHandler(EButton.GoToShop, delegate (ButtonClickedSignal signal)
            {
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.ShopRoot });
            });
            AddButtonHandler(EButton.TutorialButton, delegate (ButtonClickedSignal signal)
            {
                TutorialPanel.SetActive(true);
            });
        }
        public void CloseTutorial()
        {
            TutorialPanel.SetActive(false);
        }
    }
}