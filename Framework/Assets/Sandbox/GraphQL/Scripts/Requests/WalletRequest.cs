using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using UnityEngine;
using UnityEngine.Advertisements;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using MiniJSON;

using Common.Fsm;
using Common.Query;

using Framework;

using Sandbox.RGC;
using Sirenix.Serialization;

namespace Sandbox.GraphQL
{
    public struct OnSendToFGCWalletSignal : IRequestSignal
    {
        /// <summary>
        /// Score/ Points earned from game session
        /// </summary>
        public int Value;

        public string Event;
    }

    public struct OnFetchFGCWalletSignal : IRequestSignal
    {
        /// <summary>
        /// Player token
        /// </summary>
        public string Token;
    }

    public struct OnFetchCurrenciesSignal : IRequestSignal
    {

    }
    
    public struct OnFetchCurrencySignal : IRequestSignal
    {
        /// <summary>
        /// Player token
        /// </summary>
        public string Token;

        /// <summary>
        /// Currency slug
        /// </summary>
        public string Slug;
    }

    public struct OnConvertCurrencySignal : IRequestSignal
    {
        /// <summary>
        /// Player token
        /// </summary>
        public string Token;

        /// <summary>
        /// Currency slug
        /// </summary>
        public string Slug;

        /// <summary>
        /// Amount of the currency to be converted
        /// </summary>
        public int Amount;
    }
    
    // TODO: +AS:09172018 Updated field name
    [Serializable]
    public class FGCWallet : IJson
    {
        public string update_at;
        
        public int amount;
    }

    // TODO: +AS:09172018 Updated field name
    [Serializable]
    public class GenericWallet : IJson
    {
        public int amount;
    }

    // TODO: +AS:09172018 Updated field name
    [Serializable]
    public class FGCCurrency : IJson
    {
        [Serializable]
        public class Currency
        {
            public string id;
            
            public string slug;
            
            public int exchange_rate;
        }
        
        public int amount;
        
        public string updated_at;
        
        public Currency currency;
    }

    // TODO: +AS:09172018 Updated field name
    [Serializable]
    public class WalletConvert : IJson
    {
        public string created_at;
        
        public string id;
        
        public int value;
    }

    public class WalletRequest : UnitRequest
    {
        [Serializable]
        public class Currency : IJson
        {
            public Currency(int val)
            {
            }
        }

        [Serializable]
        public class Currency1 : Currency
        {
            public int currency_1;

            public Currency1(int val) : base(val)
            {
                currency_1 = val;
            }
        }

        [Serializable]
        public class Currency2 : Currency
        {
            public int currency_2;

            public Currency2(int val) : base(val)
            {
                currency_2 = val;
            }
        }

        [Serializable]
        public class PointCurrency : Currency
        {
            public int point;

            public PointCurrency(int val) : base(val)
            {
                point = val;
            }
        }
        [Serializable]
        public class ScoreCurrency : Currency
        {
            public int score;

            public ScoreCurrency(int val) : base(val)
            {
                score = val;
            }
        }

        [Serializable]
        private class CurrencyUpdate<T> : IJson
            where T : Currency
        {
            public T currencies;
        }

        public static readonly string FGC_WALLET_KEY = "FGCWalletKey";
        public static readonly string CONVERSION_RATE_KEY = "ConversionRateKey";
        public static readonly string HAS_CURRENCY_KEY = "HasCurrencyKey";
        public static readonly string CURRENCY_PARAM = "CurrencyParam";

        [SerializeField]
        private FGCWallet Wallet;

        [SerializeField]
        private List<FGCCurrency> Currencies;

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(FGC_WALLET_KEY);
            QuerySystem.RemoveResolver(CONVERSION_RATE_KEY);
            QuerySystem.RemoveResolver(HAS_CURRENCY_KEY);
        }

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            RegisterReceivers();
            RegisterResolvers();
        }

        private void RegisterReceivers()
        {
            this.Receive<OnFetchFGCWalletSignal>()
                .Subscribe(_ => FetchFGCWallet(_.Token))
                .AddTo(this);

            this.Receive<OnFetchCurrenciesSignal>()
                .Subscribe(_ => FetchCurrencies())
                .AddTo(this);
            
            this.Receive<OnFetchCurrencySignal>()
                .Subscribe(_ => FetchCurrencies(_.Token, _.Slug))
                .AddTo(this);

            this.Receive<OnSendToFGCWalletSignal>()
                .Subscribe(_ => SendToFGCWallet<ScoreCurrency>(_.Value, _.Event))
                .AddTo(this);

            this.Receive<OnConvertCurrencySignal>()
                .Subscribe(_ => Convert(_.Token, _.Slug, _.Amount))
                .AddTo(this);
            
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => FetchCurrencies())
                .AddTo(this);
        }

        private void RegisterResolvers()
        {
            QuerySystem.RegisterResolver(FGC_WALLET_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.AssertNotNull(Wallet, D.ERROR + "WalletRequest::FGCWallet wallet shoud never be null!\n");
                result.Set(Wallet.amount);
            });

            QuerySystem.RegisterResolver(CONVERSION_RATE_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::Currencies Invalid query! query should contain param! Query:{0} Param:{1}\n", CONVERSION_RATE_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);

                FGCCurrency currency = Currencies.Find(c => c.currency.id.Equals(id));
                Assertion.AssertNotNull(currency, D.ERROR + "WalletRequest::Currencies Invalid query param! Currencies does not contain Currency:{0}\n", id);

                result.Set(currency);
            });

            QuerySystem.RegisterResolver(HAS_CURRENCY_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::Currency Invalid query! query should contain param! Query:{0} Param:{1}\n", HAS_CURRENCY_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);
                result.Set(Currencies.Exists(c => c.currency.id.Equals(id)));
            });
        }

        #region Requests
        private void FetchFGCWallet(string token)
        {
            Builder builder = Builder.Query();
            builder
                .CreateFunction("fgc_wallet")
                .AddString("token", token);
            builder.CreateReturn("amount", "updated_at");

            ProcessRequest(GraphInfo, builder.ToString(), FetchFGCWalletResult);
        }

        private void FetchCurrencies(string token, string currencySlug)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("wallet");
            func.AddString("token", token);
            func.AddString("slug", currencySlug);
            Return ret = builder.CreateReturn("amount", "updated_at");
            ret.Add("currency", new Return("id", "slug", "exchange_rate"));

            ProcessRequest(GraphInfo, builder.ToString(), FetchCurrenciesResult);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Currency Type</typeparam>
        /// <param name="currency">Amount to Convert</param>
        /// <param name="evt">Slug of the Event</param>
        private void SendToFGCWallet<T>(int currency, string evt)
            where T : Currency
        {
            CurrencyUpdate<T> cCur = new CurrencyUpdate<T>() { currencies = (T)Activator.CreateInstance(typeof(T), currency) };
            SendToFGCWallet(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), evt, cCur);
        }

        /// <summary>
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="slug">Event slug</param>
        /// <param name="payload">Json data to send</param>
        private void SendToFGCWallet(string token, string slug, IJson payload)
        {
            SendToFGCWallet(token, slug, payload.ToJson());
        }
        
        /// <summary>
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="slug">Event slug</param>
        /// <param name="payload">Json data to send</param>
        private void SendToFGCWallet(string token, string slug, string payload)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("event_trigger");
            func.AddString("token", token);
            func.AddString("slug", slug);

            Payload load = new Payload();
            load.AddString("message", "Events Event");
            load.AddJsonString("body", payload);
            func.Add("payload", load.ToString());

            Return ret = builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), ConvertCurrencyResult);
        }

        private void Convert(string token, string currencySlug, int amount)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("wallet_convert");
            func.AddString("token", token);
            func.AddString("slug", currencySlug);
            func.AddNumber("amount", amount);
            Return ret = builder.CreateReturn("created_at", "id", "value");
            //ret.Add("player", new Return("id"));

            ProcessRequest(GraphInfo, builder.ToString(), ConvertCurrencyResult);
        }
        #endregion

        #region Parsers
        private void FetchFGCWalletResult(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_FGC_WALLET });
            }
            else
            {
                Wallet = result.Result.data.fgc_wallet;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_FGC_WALLET, Data = Wallet });
            }
        }

        private void FetchCurrenciesResult(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_CURRENCY });
            }
            else
            {
                FGCCurrency wallet = result.Result.data.wallet;
                Currencies.ReplaceOrAdd<FGCCurrency>(wallet, r => r.currency.id.Equals(wallet.currency.id));

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_CURRENCY, Data = wallet });
            }
        }
        
        private void OnSendCurrencyResult(GraphResult result)
        {
            Debug.LogFormat(D.FGC + "WalletRequest::TestUpdateCurrencyResult Result:{0}\n", result.RawResult);

            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.SEND_CURRENCY });
            }
            else
            {
                FGCCurrency wallet = result.Result.data.wallet;
                Currencies.ReplaceOrAdd<FGCCurrency>(wallet, r => r.currency.id.Equals(wallet.currency.id));

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.SEND_CURRENCY, Data = wallet });

                // NOTE: +AS:20180906 TEST Fetch calculated currencies
                FetchCurrencies();
            }
        }

        private void ConvertCurrencyResult(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.CONVERT_CURRENCY });
            }
            else
            {
                WalletConvert wallet = result.Result.data.wallet_convert;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.CONVERT_CURRENCY, Data = wallet });

                // NOTE: +AS:20180906 TEST Fetch calculated currencies
                FetchCurrencies();
            }
        }
        #endregion

        #region DEBUG

        [Button(ButtonSizes.Medium)]
        public void FetchCurrencies()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);

            this.Publish(new OnFetchFGCWalletSignal() { Token = token });
            
            this.Publish(new OnFetchCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.POINT_SLUG,
            });
        }
        
        [Button(ButtonSizes.Medium)]
        public void SendToFGCWallet()
        {
            //ConvertCurrency<Currency1>(100, RGCConst.TEST_EVENT);
            //ConvertCurrency<Currency2>(100, RGCConst.TEST_EVENT);
            //ConvertCurrency<PointCurrency>(100, RGCConst.TEST_EVENT);
            SendToFGCWallet<ScoreCurrency>(100, RGCConst.GAME_END);
        }

        [Button(ButtonSizes.Medium)]
        public void ConvertCurrency()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);

            /*
            this.Publish(new OnConvertCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.CURRENCY_1_SLUG,
                Amount = 100,
            });

            this.Publish(new OnConvertCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.CURRENCY_2_SLUG,
                Amount = 100,
            });
            */

            this.Publish(new OnConvertCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.POINT_SLUG,
                Amount = 100,
            });
        }

        #endregion
    }
}
