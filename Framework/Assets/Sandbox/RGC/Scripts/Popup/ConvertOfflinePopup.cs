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
    using TMPro;
    
    public class ConvertOfflinePopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private TMP_InputField Conversion;

        protected override void Start()
        {
            base.Start();

            Assertion.AssertNotNull(Conversion, D.ERROR + "ConvertOfflinePopup::Awake Conversion text should never be null!\n");
            Assertion.AssertNotNull(HasPopupData(), D.ERROR + "ConvertOfflinePopup::Awake PopupData should never be null!\n");

            GameResultInfo result = PopupData.GetData<GameResultInfo>();

            IQueryRequest request = QuerySystem.Start(WalletRequest.CONVERSION_RATE_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, result.CurrencyId);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            Conversion.text = string.Format("{0}", Math.Max(0, currency.amount * currency.currency.exchange_rate));
        }
    }
}