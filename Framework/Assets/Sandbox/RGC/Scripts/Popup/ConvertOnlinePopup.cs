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
    using Sandbox.ButtonSandbox;

    using Button = UnityEngine.UI.Button;

    public class ConvertOnlinePopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private TMP_InputField Accumulated;
        
        [SerializeField]
        private TMP_Text TextProgress;

        [SerializeField]
        private TMP_Text TextStamp;

        [SerializeField]
        private TMP_Text TextClaim;

        [SerializeField]
        private Button ConvertButton;
        
        [SerializeField]
        private Button ClaimButton;

        [SerializeField]
        private Slider Progress;

        private FGCCurrency Wallet;

        [SerializeField]
        private string CurrencyId;

        [SerializeField]
        private float Rate;

        [SerializeField]
        private float Stamp;

        [SerializeField]
        private int ClaimableStamp;

        [SerializeField]
        private int Score;
        
        protected override void Start()
        {
            base.Start();

            Assertion.AssertNotNull(Accumulated, D.ERROR + "ConvertOnlinePopup::Awake Accumulated text should never be null!\n");
            Assertion.AssertNotNull(TextProgress, D.ERROR + "ConvertOnlinePopup::Awake TextProgress text should never be null!\n");
            Assertion.AssertNotNull(TextClaim, D.ERROR + "ConvertOnlinePopup::Awake TextProgress text should never be null!\n");
            Assertion.AssertNotNull(TextStamp, D.ERROR + "ConvertOnlinePopup::Awake TextProgress text should never be null!\n");
            Assertion.AssertNotNull(ConvertButton, D.ERROR + "ConvertOnlinePopup::Awake ConvertButton text should never be null!\n");
            Assertion.AssertNotNull(ClaimButton, D.ERROR + "ConvertOnlinePopup::Awake ClaimButton text should never be null!\n");
            Assertion.AssertNotNull(Progress, D.ERROR + "ConvertOnlinePopup::Awake Progress should never be null!\n");
            Assertion.AssertNotNull(HasPopupData(), D.ERROR + "ConvertOnlinePopup::Awake PopupData should never be null!\n");
            
            // TODO: +AS:09212018 Adjust result calculation
            GameResultInfo result = PopupData.GetData<GameResultInfo>();
            CurrencyId = result.CurrencyId;
            Score = result.GameScore;
            Rate = (1f / result.Rate).Floor();
            Stamp = QuerySystem.Query<float>(WalletRequest.FGC_WALLET_KEY).Floor();

            int rate = Rate.ToInt();
            ClaimableStamp = ((Score - (Score % rate)) / rate);

            Progress.value = Score / Rate;
            Accumulated.text = string.Format("{0}", Score);
            TextProgress.text = string.Format("{0} / {1}", Score, Rate);
            TextStamp.text = GetStampDisplay(Stamp.ToInt());
            TextClaim.text = GetClaimMessage(Stamp.ToInt());

            bool canClaim = Stamp >= 1f;
            if (canClaim)
            {
                ConvertButton.gameObject.SetActive(!canClaim);
                ClaimButton.gameObject.SetActive(canClaim);
            }
            else
            {
                ConvertButton.gameObject.SetActive(!canClaim);
                ConvertButton.interactable = Score >= Rate.ToInt();
                ClaimButton.gameObject.SetActive(canClaim);
            }
        }
        
        public void ConvertPoints()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            string currencySlug = CurrencyId;
            int amount = ClaimableStamp * Rate.ToInt();
            
            Builder builder;
            Function func;
            Return ret;
            OnHandleGraphRequestSignal signal;

            // Fetch currencies
            Action fetchCurrencies = () =>
            {
                Debug.LogFormat(D.FGC + "TitleRoot::OnClickedSend OnFechCurrencies.\n");

                builder = Builder.Query();
                func = builder.CreateFunction("wallet");
                func.AddString("token", token);
                func.AddString("slug", currencySlug);
                ret = builder.CreateReturn("amount", "updated_at");
                ret.Add("currency", new Return("id", "slug", "exchange_rate"));

                signal = new OnHandleGraphRequestSignal();
                signal.Builder = builder;
                signal.Parser = result =>
                {
                    this.Publish(new OnUpdateFGCCurrency() { Result = result });
                    this.Publish(new OnCloseActivePopup());
                };

                this.Publish(signal);
            };

            // Fetch wallet
            Action fetchWallet = () =>
            {
                Debug.LogFormat(D.FGC + "TitleRoot::OnClickedSend OnFetchWallet.\n");

                builder = Builder.Query();
                builder
                    .CreateFunction("fgc_wallet")
                    .AddString("token", token);
                builder.CreateReturn("amount", "updated_at");

                signal = new OnHandleGraphRequestSignal();
                signal.Builder = builder;
                signal.Parser = result =>
                {
                    this.Publish(new OnUpdateFGCWallet() { Result = result });
                    fetchCurrencies();
                };

                this.Publish(signal);
            };

            // Convert request
            builder = Builder.Mutation();
            builder.CreateReturn("created_at", "id", "value");

            func = builder.CreateFunction("wallet_convert");
            func.AddString("token", token);
            func.AddString("slug", currencySlug);
            func.AddNumber("amount", amount);

            signal = new OnHandleGraphRequestSignal();
            signal.Builder = builder;
            signal.Parser = result => fetchWallet();

            this.Publish(new OnCloseActivePopup());
            this.Publish(new OnShowPopupSignal() { Popup = PopupType.Spinner });
            this.Publish(signal);
        }

        private string GetStampDisplay(int stamps)
        {
            if (stamps >= 1)
            {
                return string.Format("{0} STAMPS", stamps);
            }

            return string.Format("{0} STAMP", stamps);
        }

        private string GetClaimMessage(int claimables)
        {
            if (claimables >= 2)
            {
                return string.Format("CLAIM {0} STAMP", claimables);
            }

            return string.Format("CLAIM {0} STAMPS", claimables);
        }
    }
}