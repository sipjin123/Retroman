using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;
using Sandbox.Popup;

namespace Sandbox.RGC
{
    // alias
    using TMPro;

    using CColor = Framework.Color;

    public class ConvertOnlinePopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private TMP_InputField Conversion;

        [SerializeField]
        private TMP_InputField Coins;

        [SerializeField]
        private TMP_InputField CalcConveersion;

        private WalletConversion Wallet;

        protected override void Start()
        {
            base.Start();

            Assertion.AssertNotNull(Conversion, D.ERROR + "ConvertOnlinePopup::Awake Conversion text should never be null!\n");
            Assertion.AssertNotNull(Coins, D.ERROR + "ConvertOnlinePopup::Awake Coins text should never be null!\n");
            Assertion.AssertNotNull(CalcConveersion, D.ERROR + "ConvertOnlinePopup::Awake CalcConveersion text should never be null!\n");
            Assertion.AssertNotNull(HasPopupData(), D.ERROR + "ConvertOnlinePopup::Awake PopupData should never be null!\n");
            
            IQueryRequest request = QuerySystem.Start(WalletRequest.CONVERSION_RATE_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, PopupData.GetData<string>());
            Wallet = QuerySystem.Complete<WalletConversion>();

            Conversion.text = string.Format("{0}", Convert(Wallet.amount, Wallet.currency.exchange_rate));

            RegisterSignals();
        }

        private int Convert(int amount, int rate)
        {
            return Math.Max(0, amount * rate);
        }
        
        private void RegisterSignals()
        {
            Coins
                .ObserveEveryValueChanged(t => t.text)
                .Subscribe(_ =>
                {
                    int coins = Mathf.Max(0, Mathf.Min(Wallet.amount, _.ToInt()));
                    Coins.text = string.Format("{0}", coins);
                    CalcConveersion.text = string.Format("{0}", Mathf.Max(0, coins * Wallet.currency.exchange_rate));
                })
                .AddTo(this);
        }

        public void ConvertPoints()
        {
            int jolens = Coins.text.ToInt();

            this.Publish(new OnConvertCurrencySignal()
            {
                Token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN),
                Slug = RGCConst.POINT_SLUG,
                Amount = jolens,
            });
        }
    }
}