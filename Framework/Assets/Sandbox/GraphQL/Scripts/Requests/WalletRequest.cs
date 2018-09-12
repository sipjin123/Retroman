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

namespace Sandbox.GraphQL
{
    public struct SendToFGCWalletSignal : IRequestSignal
    {
        public int Value;
    }

    public struct FetchConversionSignal : IRequestSignal
    {

    }

    public struct OnGetFGCWalletSignal : IRequestSignal
    {
        /// <summary>
        /// Player token
        /// </summary>
        public string Token;
    }

    public struct OnGetConversionRateSignal : IRequestSignal
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

    public struct TEST_OnFetchRatesSignal : IRequestSignal { }

    [Serializable]
    public class FGCWallet : IJson
    {
        public string update_at;
        public int amount;
    }
    [Serializable]
    public class GenericWallet : IJson
    {
        public int amount;
    }

    [Serializable]
    public class WalletConversion : IJson
    {
        [Serializable]
        public class Currency
        {
            public string id;
            public string name;
            public int exchange_rate;
        }

        public int amount;
        public string updated_at;
        public Currency currency;
    }

    [Serializable]
    public class WalletConvert : IJson
    {
        public string created_at;
        public string id;
        public int value;
    }

    public class WalletRequest : UnitRequest
    {
        public static readonly string FGC_WALLET_KEY = "FGCWalletKey";
        public static readonly string CONVERSION_RATE_KEY = "ConversionRateKey";
        public static readonly string HAS_CURRENCY_KEY = "HasCurrencyKey";
        public static readonly string CURRENCY_PARAM = "CurrencyParam";

        [SerializeField]
        private FGCWallet Wallet;

        [SerializeField]
        private GenericWallet GenericWallet;
        [SerializeField]
        private List<WalletConversion> ConversionRates;
        
        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(FGC_WALLET_KEY);
            QuerySystem.RemoveResolver(CONVERSION_RATE_KEY);
            QuerySystem.RemoveResolver(HAS_CURRENCY_KEY);
        }

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);


            this.Receive<SendToFGCWalletSignal>()
                .Subscribe(_ =>
                {
                    Debug.LogError(D.B + "Sending Points to Server : " + _.Value);
                    UpdateCurrency<ScoreCurrency>(_.Value);
                }).AddTo(this);

            this.Receive<FetchConversionSignal>().Subscribe(_ =>
            {

                Debug.LogError(D.B + "Requesting Conversion Data");
                FetchGameWallet();
                TestGetConversionRate();
            }).AddTo(this);

            this.Receive<OnGetFGCWalletSignal>()
                .Subscribe(_ => GetFGCWallet(_.Token))
                .AddTo(this);

            this.Receive<OnGetConversionRateSignal>()
                .Subscribe(_ => GetConversionRate(_.Token, _.Slug))
                .AddTo(this);

            this.Receive<OnConvertCurrencySignal>()
                .Subscribe(_ => Convert(_.Token, _.Slug, _.Amount))
                .AddTo(this);

            this.Receive<TEST_OnFetchRatesSignal>()
                .Subscribe(_ => TestGetConversionRate())
                .AddTo(this);

            // TEST
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => TestGetConversionRate())
                .AddTo(this);


            RegisterResolvers();
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
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::ConversionRates Invalid query! query should contain param! Query:{0} Param:{1}\n", CONVERSION_RATE_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);

                WalletConversion conversion = ConversionRates.Find(c => c.currency.id.Equals(id));
                Assertion.AssertNotNull(conversion, D.ERROR + "WalletRequest::ConversionRates Invalid query param! ConversionRates does not contain Currency:{0}\n", id);
                
                result.Set(conversion);
            });
            
            QuerySystem.RegisterResolver(HAS_CURRENCY_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::Currency Invalid query! query should contain param! Query:{0} Param:{1}\n", HAS_CURRENCY_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);
                result.Set(ConversionRates.Exists(c => c.currency.id.Equals(id)));
            });
        }

        #region Requests
        [Button]
        private void FetchGameWallet()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            Builder builder = Builder.Query();
            builder
                .CreateFunction("wallet")
                .AddString("token", token)
                .AddString("slug", "score");
            builder.CreateReturn("amount");

            ProcessRequest(GraphInfo, builder.ToString(), GetWalletResult);
        }
        private void GetFGCWallet(string token)
        {
            Builder builder = Builder.Query();
            builder
                .CreateFunction("fgc_wallet")
                .AddString("token", token);
            builder.CreateReturn("amount", "updated_at");

            ProcessRequest(GraphInfo, builder.ToString(), GetFGCWalletResult);
        }

        private void GetConversionRate(string token, string currencySlug)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("wallet");
            func.AddString("token", token);
            func.AddString("slug", currencySlug);
            Return ret = builder.CreateReturn("amount", "updated_at");
            ret.Add("currency", new Return("id", "name", "exchange_rate"));

            ProcessRequest(GraphInfo, builder.ToString(), GetConversionRateResult);
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

        /// <summary>
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="slug">Event slug</param>
        /// <param name="payload">Json data to send</param>
        private void TestUpdateCurrency(string token, string slug, IJson payload)
        {
            TestUpdateCurrency(token, slug, payload.ToJson());
        }

        /// <summary>
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="slug">Event slug</param>
        /// <param name="payload">Json data to send</param>
        private void TestUpdateCurrency(string token, string slug, string payload)
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

            ProcessRequest(GraphInfo, builder.ToString(), TestUpdateCurrencyResult);
        }
        #endregion

        #region Parsers

        private void GetWalletResult(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_WALLET });
            }
            else
            {
                GenericWallet = result.Result.data.generic_wallet;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_WALLET, Data = Wallet });
            }
        }
        private void GetFGCWalletResult(GraphResult result)
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

        private void GetConversionRateResult(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_CURRENCY_CONVERSION_RATE });
            }
            else
            {
                WalletConversion wallet = result.Result.data.wallet;
                ConversionRates.ReplaceOrAdd<WalletConversion>(wallet, r => r.currency.id.Equals(wallet.currency.id));

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_CURRENCY_CONVERSION_RATE, Data = wallet });
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
                TestGetConversionRate();
            }
        }

        private void TestUpdateCurrencyResult(GraphResult result)
        {
            Debug.LogFormat(D.FGC + "WalletRequest::TestUpdateCurrencyResult Result:{0}\n", result.RawResult);
        }
        #endregion

        #region DEBUG
        
        [Button(ButtonSizes.Medium)]
        public void TestGetConversionRate()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);

            this.Publish(new OnGetFGCWalletSignal() { Token = token });
            /*
            this.Publish(new OnGetConversionRateSignal()
            {
                Token = token,
                Slug = RGCConst.CURRENCY_1_SLUG,
            });

            this.Publish(new OnGetConversionRateSignal()
            {
                Token = token,
                Slug = RGCConst.CURRENCY_2_SLUG,
            });
            */
            this.Publish(new OnGetConversionRateSignal()
            {
                Token = token,
                Slug = RGCConst.POINT_SLUG,
            });
        }

        [Button(ButtonSizes.Medium)]
        public void TestConvertCurrent()
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
            });*/

            this.Publish(new OnConvertCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.POINT_SLUG,
                Amount = 100,
            });
        }
        
        [Serializable]
        public class Currency : IJson
        {
            public Currency(int val)
            {
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
        private class CurrencyUpdate<T> : IJson 
            where T : Currency
        {
            public T currencies;
        }

        [Button(ButtonSizes.Medium)]
        public void TestUpdateCurrency()
        {
            /*UpdateCurrency<Currency1>(100);
            UpdateCurrency<Currency2>(100);
            UpdateCurrency<PointCurrency>(100); */
            
            UpdateCurrency<ScoreCurrency>(100);
        }

        private void UpdateCurrency<T>(int currency)
            where T : Currency
        {
            CurrencyUpdate<T> cCur = new CurrencyUpdate<T>() { currencies = (T)Activator.CreateInstance(typeof(T), currency) };
            //TestUpdateCurrency(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), "test_event", cCur);
            TestUpdateCurrency(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), "game_end", cCur);
        }

        #endregion
    }
}
