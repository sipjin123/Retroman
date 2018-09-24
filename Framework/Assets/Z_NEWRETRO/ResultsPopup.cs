using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retroman;
using Framework;
using UniRx;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sandbox.GraphQL;
using Sandbox.RGC;

namespace RetroMountain
{
    public class ResultsPopup : MonoBehaviour
    {
        public CanvasGroup InteractiveCanvas;

        public Text HiScore1, HiScore2, CScore1, CScore2;

        public Image CharImage;

        private DataManagerService DataService;
        private MessageBroker DataServiceBroker;

        private void Awake()
        {
            DataService = Factory.Get<DataManagerService>();
            DataServiceBroker = DataService.MessageBroker;

            DataServiceBroker.Receive<EndGame>()
                .Subscribe(_ => ShowResults())
                .AddTo(this);

            DataServiceBroker.Receive<TriggerCanvasInteraction>()
                .Subscribe(_ => InteractiveCanvas.interactable = false)
                .AddTo(this);
        }

        private void OnEnable()
        {
            ShowResults();
        }

        private void ShowResults()
        {
            float score = DataService.GetScore();
            float highScore = DataService.GetHighScore();

            CScore1.text = "" + score;
            CScore2.text = "" + score;
            HiScore1.text = "Best Score " + highScore;
            HiScore2.text = "Best Score " + highScore;

            this.Publish(new OnSendToFGCWalletSignal() { Value = (int)score, Event = RGCConst.GAME_END });
            this.Publish(new OnFetchCurrenciesSignal());

            int currChar = DataService.CurrentCharacterSelected - 1;
            CharImage.sprite = DataService.ShopItems[currChar].ItemImage.sprite;
        }
    }
}