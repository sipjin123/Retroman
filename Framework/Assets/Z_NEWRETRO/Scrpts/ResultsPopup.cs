﻿using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using Common.Query;
using UnityEngine;
using Retroman;
using Framework;
using Sandbox.Facebook;
using Sandbox.FGCAutomation;
using Sandbox.FGCAutomation.Data;
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
        public GameObject ConfetiObject;

        [SerializeField]
        private GameObject _BlackBackground;
        private void Awake()
        {
            DataService = Factory.Get<DataManagerService>();
            DataServiceBroker = DataService.MessageBroker;

            DataServiceBroker.Receive<EndGame>()
                .Subscribe(_ => ShowResults())
                .AddTo(this);

            DataServiceBroker.Receive<TriggerCanvasInteraction>()
                .Subscribe(_ =>
                {
                    _BlackBackground.SetActive(true);
                    InteractiveCanvas.interactable = false;
                    ConfetiObject.SetActive(false);
                })
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

            if(highScore == 0)
            {
                HiScore1.gameObject.SetActive(false);
                HiScore2.gameObject.SetActive(false);
            }
            if(score >= highScore && highScore != 0)
            {
                ConfetiObject.SetActive(true);
            }
            else
            {
                ConfetiObject.SetActive(false);
            }

            if (QuerySystem.Query<bool>(FBID.HasLoggedInUser))
            {
                this.Publish(new OnSendToFGCWalletSignal() { Value = (int)score, Event = RGCConst.GAME_END });
                this.Publish(new OnFetchCurrenciesSignal());


                TrackCurrency signal;
                signal.Currency = CurrencyEnum.Score;
                signal.MatchNumber = -1;

                IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
                request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
                float currencyRate = QuerySystem.Complete<FGCCurrency>().CurrencyInfo.Rate;

                TrackedData data = new TrackedData()
                {
                    Score = (int)score,
                    StampsFloat = ((int)score) * currencyRate
                };
                data.StampsInt = (int) data.StampsFloat;
                data.IsAverage = false;
                signal.Value = data;

                this.Publish(signal);

            }

            int currChar = DataService.CurrentCharacterSelected - 1;
            CharImage.sprite = DataService.ShopItems[currChar].ItemImage.sprite;
        }
    }
}