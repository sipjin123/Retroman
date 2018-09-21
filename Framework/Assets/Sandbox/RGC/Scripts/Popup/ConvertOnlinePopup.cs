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

using TMPro;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

using Sandbox.Facebook;
using Sandbox.GraphQL;
using Sandbox.Popup;

namespace Sandbox.RGC
{
    using Button = UnityEngine.UI.Button;

    public class ConvertOnlinePopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private TMP_InputField Accumulated;
        
        [SerializeField]
        private TMP_InputField Converted;

        [SerializeField]
        private Button ConvertButton;
        private Button SeePrizesButton;

        private FGCCurrency Wallet;

        private string CurrencyId;
        private int ConvertCap;
        private int ConvertValue;

        protected override void Start()
        {
            base.Start();

            Assertion.AssertNotNull(Accumulated, D.ERROR + "ConvertOnlinePopup::Awake Accumulated text should never be null!\n");
            Assertion.AssertNotNull(Converted, D.ERROR + "ConvertOnlinePopup::Awake Converted text should never be null!\n");
            Assertion.AssertNotNull(ConvertButton, D.ERROR + "ConvertOnlinePopup::Awake ConvertButton text should never be null!\n");
            Assertion.AssertNotNull(SeePrizesButton, D.ERROR + "ConvertOnlinePopup::Awake SeePrizesButton text should never be null!\n");
            Assertion.AssertNotNull(HasPopupData(), D.ERROR + "ConvertOnlinePopup::Awake PopupData should never be null!\n");

            // TODO: +AS:09212018 Adjust result calculation
            GameResultInfo result = PopupData.GetData<GameResultInfo>();
            CurrencyId = result.CurrencyId;
            ConvertCap = result.GameScore % Wallet.currency.exchange_rate;
            ConvertValue = ConvertCap * Wallet.currency.exchange_rate;

            IQueryRequest request = QuerySystem.Start(WalletRequest.CONVERSION_RATE_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, CurrencyId);
            Wallet = QuerySystem.Complete<FGCCurrency>();
            
            Accumulated.text = string.Format("{0}", ConvertCap);
            Converted.text = string.Format("{0}", ConvertValue);

            bool hasFBLogin = QuerySystem.Query<bool>(FBID.HasLoggedInUser);
            if (hasFBLogin)
            {
                ConvertButton.gameObject.SetActive(true);
                SeePrizesButton.gameObject.SetActive(false);
            }
            else
            {
                ConvertButton.gameObject.SetActive(false);
                SeePrizesButton.gameObject.SetActive(true);
            }    
        }
        
        public void ConvertPoints()
        {
            this.Publish(new OnConvertCurrencySignal()
            {
                Token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN),
                Slug = CurrencyId,
                Amount = ConvertCap,
            });
        }

        public void SeePrizes()
        {

        }
    }
}