using Common.Fsm;
using Common.Query;
using Framework;
using Sandbox.Facebook;
using Sandbox.GraphQL;
using Sandbox.Popup;
using System;
using UnityEngine;

namespace Sandbox.RGC
{
    using UniRx;

    /// <summary>
    /// Guest -> FB -> SendScore -> (Convert | Claim | SendScore) Loop
    /// </summary>
    public class FGCUITestAppRoutine : FGCUIRoutine
    {
        public OnAutoConnectToFGC GuestLoginSignal = new OnAutoConnectToFGC() { Type = AccountType.Guest };
        public OnAutoConnectToFGC FBLoginSignal = new OnAutoConnectToFGC() { Type = AccountType.Facebook };
        public OnTestSendScore SendScoreSignal = new OnTestSendScore() { Score = 2 };

        #region Protected Methods

        protected override void OnEnterIdleState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterIdleState\n");

            WaitForSecondsRealtimePromise(.5f)
                .Then(() => this.Publish(GuestLoginSignal));
        }

        protected override void OnEnterGuestState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterGuestState\n");

            WaitForSecondsRealtimePromise(5f)
                .Then(() => this.Publish(FBLoginSignal));
        }

        protected override void OnEnterFbState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterFbState\n");

            Func<bool> fbLogin = () => QuerySystem.Query<bool>(RGCService.HAS_FB_TOKEN) && QuerySystem.Query<bool>(FBID.HasLoggedInUser);

            WaitUntilPromise(fbLogin)
                .Then(() => TransitionToState(StateSend));
        }

        protected override void OnEnterSendState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterSendState\n");

            // Send Score
            this.Publish(SendScoreSignal);

            if (CanConvert() || CanClaim())
            {
                WaitForSecondsRealtimePromise(5f)
                    .Then(() => TransitionToState(StateConvert));
            }
            else
            {
                WaitForSecondsRealtimePromise(5f)
                    .Then(() => TransitionToState(StateSend));
            }
        }

        protected override void OnEnterConvertState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterConvertState\n");
            
            if (CanClaim())
            {
                WaitForSecondsRealtimePromise(5f)
                    .Then(() => this.Publish(new OnGetSynertix()))
                    .Then(_ => WaitForSecondsRealtimePromise(5f))
                    .Then(() => this.Publish(new OnCloseActivePopup() { All = true }))
                    .Then(() => TransitionToState(StateClaim));
            }
            else if (CanConvert())
            {
                WaitForSecondsRealtimePromise(5f)
                    .Then(() => this.Publish(new OnGetSynertix()))
                    .Then(_ => WaitForSecondsRealtimePromise(5f))
                    .Then(() => this.Publish(new OnCloseActivePopup() { All = true }))
                    .Then(() => ConvertPoints());
            }
            else
            {
                FetchCurrencies();
            }
        }

        protected override void OnEnterClaimState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterClaimState\n");

            WaitForSecondsRealtimePromise(0.1f)
                .Then(() => this.Publish(new OnClaimStamps()))
                .Then(_ => WaitForSecondsRealtimePromise(10f))
                .Then(() => this.Publish(new OnCloseActivePopup() { All = true }))
                .Then(_ => WaitForSecondsRealtimePromise(5f))
                .Then(() => FetchCurrencies());
        }

        protected override void OnEnterFetchState(FsmState owner)
        {
            DebugLog("FGCUITestAppRoutine::OnEnterFetchState\n");
        }

        #endregion Protected Methods

        #region Helper Methods

        private bool CanConvert()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            return currency.Amount >= (1f / currency.CurrencyInfo.Rate);
        }

        private bool CanClaim()
        {
            float stamp = QuerySystem.Query<float>(WalletRequest.FGC_WALLET_KEY).Floor();

            return stamp >= 1f;
        }

        private void ConvertPoints()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            string currencySlug = currency.CurrencyInfo.CurrencySlug;

            int rate = (1f / currency.CurrencyInfo.Rate).Floor().ToInt();
            int score = currency.Amount;
            int claimable = ((score - (score % rate)) / rate);
            int amount = claimable * rate;

            Builder builder;
            Function func;
            Return ret;
            OnHandleGraphRequestSignal signal;

            // Fetch currencies
            Action fetchCurrencies = () =>
            {
                Debug.LogFormat(D.FGC + "FGCUITestAppRoutine::ConvertPoints OnFechCurrencies.\n");

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
                    TransitionToState(StateSend);
                };

                this.Publish(signal);
            };

            // Fetch wallet
            Action fetchWallet = () =>
            {
                Debug.LogFormat(D.FGC + "FGCUITestAppRoutine::ConvertPoints OnFetchWallet.\n");

                builder = Builder.Query();
                builder.CreateFunction("fgc_wallet").AddString("token", token);
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

        private void FetchCurrencies()
        {
            IQueryRequest request = QuerySystem.Start(WalletRequest.CURRENCY_KEY);
            request.AddParameter(WalletRequest.CURRENCY_PARAM, RGCConst.SCORE_SLUG);
            FGCCurrency currency = QuerySystem.Complete<FGCCurrency>();

            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            string currencySlug = currency.CurrencyInfo.CurrencySlug;

            int rate = currency.CurrencyInfo.Rate.ToInt();
            int score = currency.Amount;
            int claimable = ((score - (score % rate)) / rate);
            int amount = claimable * rate;

            Builder builder;
            Function func;
            Return ret;
            OnHandleGraphRequestSignal signal;

            // Fetch currencies
            Action fetchCurrencies = () =>
            {
                Debug.LogFormat(D.FGC + "FGCUITestAppRoutine::ConvertPoints OnFechCurrencies.\n");

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
                    TransitionToState(StateSend);
                };

                this.Publish(signal);
            };

            // Fetch wallet
            Action fetchWallet = () =>
            {
                Debug.LogFormat(D.FGC + "FGCUITestAppRoutine::ConvertPoints OnFetchWallet.\n");

                builder = Builder.Query();
                builder.CreateFunction("fgc_wallet").AddString("token", token);
                builder.CreateReturn("amount", "updated_at");

                signal = new OnHandleGraphRequestSignal();
                signal.Builder = builder;
                signal.Parser = result =>
                {
                    this.Publish(new OnUpdateFGCWallet() { Result = result });
                    fetchCurrencies();
                };

                this.Publish(new OnShowPopupSignal() { Popup = PopupType.Spinner });
                this.Publish(signal);
            };

            fetchWallet();
        }
        #endregion
    }
}
