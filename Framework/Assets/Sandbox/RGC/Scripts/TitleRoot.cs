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

            Assertion.AssertNotNull(FGCWallet);
            Assertion.AssertNotNull(CurrencyId);
            Assertion.AssertNotNull(CurrencyName);
            Assertion.AssertNotNull(CurrencyValue);
            Assertion.AssertNotNull(CurrencyRate);

            AddButtonHandler(ButtonType.FGC, delegate(ButtonClickedSignal signal)
            {
                this.Publish(new OnConnectToFGCApp());
            });

            AddButtonHandler(ButtonType.ConnectToFGC, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConnectToFGC });
            });

            AddButtonHandler(ButtonType.GetSynerytix, delegate (ButtonClickedSignal signal)
            {
                this.Publish(new OnGetSynertix());
            });

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.GET_FGC_WALLET))
                .Subscribe(_ => ShowFGCWallet(_.GetData<FGCWallet>()))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type.Equals(GraphQLRequestType.GET_CURRENCY_CONVERSION_RATE))
                .Where(_ => _.GetData<WalletConversion>().currency.id.Equals(RGCConst.POINT_ID))
                .Subscribe(_ => ShowCurrencyDetails(_.GetData<WalletConversion>()))
                .AddTo(this);

            IQueryRequest request = QuerySystem.Start(WalletRequest.HAS_CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.POINT_ID);

            if (QuerySystem.Complete<bool>())
            {
                request = QuerySystem.Start(WalletRequest.CONVERSION_RATE_KEY);
                request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.POINT_ID);
                ShowCurrencyDetails(QuerySystem.Complete<WalletConversion>());
            }
        }

        private void ShowFGCWallet(FGCWallet wallet)
        {
            FGCWallet.text = string.Format("FGCWallet value (tickets): {0}", wallet.amount.ToString().RichTextColorize(Color.green));
        }

        private void ShowCurrencyDetails(WalletConversion wallet)
        {
            CurrencyId.text = string.Format("Currency Id (jolens): {0}", wallet.currency.id.RichTextColorize(Color.green));
            CurrencyName.text = string.Format("Currency Name (jolens): {0}", wallet.currency.name.RichTextColorize(Color.green));
            CurrencyValue.text = string.Format("Currency Value (jolens): {0}", wallet.amount.ToString().RichTextColorize(Color.green));
            CurrencyRate.text = string.Format("Currency Rate: {0}", wallet.currency.exchange_rate.ToString().RichTextColorize(Color.green));
        }

        public void OnClickedOnline()
        {
            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOnline, PopupData = new PopupData(RGCConst.POINT_ID) });
        }

        public void OnClickedOffline()
        {
            this.Publish(new OnShowPopupSignal() { Popup = PopupType.ConvertOffline, PopupData = new PopupData(RGCConst.POINT_ID) });
        }
    }
}