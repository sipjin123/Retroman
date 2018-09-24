using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

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

    using Color = UnityEngine.Color;

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
        private TextMeshProUGUI FGCWallet;

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
            Assertion.AssertNotNull(FGCWallet);
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
                .Where(_ => _.GetData<FGCCurrency>().currency.id.Equals(RGCConst.POINT_ID))
                .Subscribe(_ => ShowCurrencyDetails(_.GetData<FGCCurrency>()))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.PLAYER_DATA))
                .Subscribe(_ => ShowPlayerInfo(_.GetData<PlayerIDContainer>()))
                .AddTo(this);

            IQueryRequest request = QuerySystem.Start(WalletRequest.HAS_CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.POINT_ID);

            if (QuerySystem.Complete<bool>())
            {
                request = QuerySystem.Start(WalletRequest.CONVERSION_RATE_KEY);
                request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.POINT_ID);
                ShowCurrencyDetails(QuerySystem.Complete<FGCCurrency>());
            }
        }

        private void ShowFGCWallet(FGCWallet wallet)
        {
            FGCWallet.text = string.Format("FGCWallet value (tickets): {0}", wallet.amount.ToString().RichTextColorize(Color.green));
        }

        private void ShowCurrencyDetails(FGCCurrency wallet)
        {
            CurrencyId.text = string.Format("Currency Id (jolens): {0}", wallet.currency.id.RichTextColorize(Color.green));
            CurrencyName.text = string.Format("Currency Name (jolens): {0}", wallet.currency.slug.RichTextColorize(Color.green));
            CurrencyValue.text = string.Format("Currency Value (jolens): {0}", wallet.amount.ToString().RichTextColorize(Color.green));
            CurrencyRate.text = string.Format("Currency Rate: {0}", wallet.currency.exchange_rate.ToString().RichTextColorize(Color.green));
        }

        private void ShowPlayerInfo(PlayerIDContainer info)
        {
            Id.text = string.Format("Id: {0}", info.id.RichTextColorize(Color.green));
            DeviceId.text = string.Format("Device Id: {0}", info.device_id.RichTextColorize(Color.green));
            FBId.text = string.Format("FB Id: {0}", info.facebook_id.RichTextColorize(Color.green));
            GSId.text = string.Format("GS Id: {0}", info.gamesparks_id.RichTextColorize(Color.green));
        }

        public void OnClickedOnline()
        {
            GameResultInfo data = GameResultFactory.CreateDefault();

            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(data) });
        }

        public void OnClickedOffline()
        {
            GameResultInfo data = GameResultFactory.CreateDefault();

            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOffline, PopupData = new PopupData(data) });
        }

        public void OnClickedSend()
        {
            this.Publish(new OnSendToFGCWalletSignal() { Value = 100, Event  = RGCConst.GAME_END });
        }
    }
}