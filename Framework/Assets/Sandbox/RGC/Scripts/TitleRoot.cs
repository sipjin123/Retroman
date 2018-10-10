using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Newtonsoft.Json;

using Common;
using Common.Query;

using Framework;

namespace Sandbox.RGC
{
    using Framework.ExtensionMethods;

    using Sandbox.ButtonSandbox;
    using Sandbox.GraphQL;
    using Sandbox.Popup;
    using Sandbox.Preloader;

    using TMPro;

    // Alias
    using Color = UnityEngine.Color;
    using Currency = Sandbox.GraphQL.WalletRequest.CurrencyUpdate<Sandbox.GraphQL.WalletRequest.ScoreCurrency>;

    public struct Id
    {
        public string id;
    }

    public struct Error
    {
        public string message;
    }

    public class TitleRoot : SceneObject
    {
        [SerializeField]
        private TextMeshProUGUI Id;

        [SerializeField]
        private TextMeshProUGUI DeviceId;

        [SerializeField]
        private TextMeshProUGUI FBId;

        [SerializeField]
        private TextMeshProUGUI GSId;
        
        [SerializeField]
        private TextMeshProUGUI Tickets;

        [SerializeField]
        private TextMeshProUGUI CurrencyId;

        [SerializeField]
        private TextMeshProUGUI CurrencyName;

        [SerializeField]
        private TextMeshProUGUI CurrencyValue;

        [SerializeField]
        private TextMeshProUGUI CurrencyRate;
        
        protected override void Start()
        {
            base.Start();

            Assertion.AssertNotNull(Id);
            Assertion.AssertNotNull(DeviceId);
            Assertion.AssertNotNull(FBId);
            Assertion.AssertNotNull(GSId);
            Assertion.AssertNotNull(Tickets);
            Assertion.AssertNotNull(CurrencyId);
            Assertion.AssertNotNull(CurrencyName);
            Assertion.AssertNotNull(CurrencyValue);
            Assertion.AssertNotNull(CurrencyRate);

            //AddButtonHandler(ButtonType.FGC, delegate(ButtonClickedSignal signal)
            //{
            //    this.Publish(new OnConnectToFGCApp());
            //});

            //AddButtonHandler(ButtonType.ConnectToFGC, delegate (ButtonClickedSignal signal)
            //{
            //    this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
            //});

            //AddButtonHandler(ButtonType.GetSynerytix, delegate (ButtonClickedSignal signal)
            //{
            //    this.Publish(new OnGetSynertix());
            //});

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.GET_FGC_WALLET))
                .Subscribe(_ => ShowFGCWallet(_.GetData<FGCWallet>()))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.GET_CURRENCY))
                .Subscribe(_ => ShowCurrencyDetails(_.GetData<FGCCurrency>()))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.PLAYER_DATA))
                .Subscribe(_ => ShowPlayerInfo(_.GetData<PlayerIDContainer>()))
                .AddTo(this);

            IQueryRequest request = QuerySystem.Start(WalletRequest.HAS_CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);

            if (QuerySystem.Complete<bool>())
            {
                request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
                request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
                ShowCurrencyDetails(QuerySystem.Complete<FGCCurrency>());
            }
        }

        private void ShowFGCWallet(FGCWallet wallet)
        {
            this.Tickets.text = string.Format("Tickets: {0}", wallet.Amount.ToString().RichTextColorize(Color.green));
        }

        private void ShowCurrencyDetails(FGCCurrency wallet)
        {
            CurrencyValue.text = string.Format("Submitted Score: {0}", wallet.Amount.ToString().RichTextColorize(Color.green));
            CurrencyId.text = string.Format("Currency Id: {0}", wallet.CurrencyInfo.Id.RichTextColorize(Color.green));
            CurrencyName.text = string.Format("Currency Name: {0}", wallet.CurrencyInfo.CurrencySlug.RichTextColorize(Color.green));
            CurrencyRate.text = string.Format("Currency Rate: {0}", wallet.CurrencyInfo.Rate.ToString().RichTextColorize(Color.green));
        }

        private void ShowPlayerInfo(PlayerIDContainer info)
        {
            Id.text = string.Format("Id: {0}", info.Id.RichTextColorize(Color.green));
            DeviceId.text = string.Format("Device Id: {0}", info.DeviceId.RichTextColorize(Color.green));
            FBId.text = string.Format("FB Id: {0}", info.FbId.RichTextColorize(Color.green));
            GSId.text = string.Format("GS Id: {0}", info.GSId.RichTextColorize(Color.green));
        }

        public void OnClickedOnline()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            GameResultInfo data = GameResultFactory.Create(
                currency.CurrencyInfo.CurrencySlug,
                currency.Amount,
                currency.CurrencyInfo.Rate);

            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(data) });
        }

        public void OnClickedOffline()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            GameResultInfo data = GameResultFactory.Create(
                currency.CurrencyInfo.CurrencySlug,
                currency.Amount,
                currency.CurrencyInfo.Rate);

            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOffline, PopupData = new PopupData(data) });
        }

        public void OnClickedSend()
        {
            this.Publish(new OnTestSendScore() { Score = 2 });
        }

        public void OnClickedConnectGuest()
        {
            this.Publish(new OnAutoConnectToFGC() { Type = AccountType.Guest });
        }

        public void OnClickedConnectFacebook()
        {
            this.Publish(new OnAutoConnectToFGC() { Type = AccountType.Facebook });
        }
    }
}