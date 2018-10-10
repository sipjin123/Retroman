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

using Newtonsoft.Json;

using MiniJSON;

using Common.Fsm;
using Common.Query;

using Framework;

using Sandbox.RGC;
using Sirenix.Serialization;

namespace Sandbox.GraphQL
{
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

    public struct OnSendToFGCWalletSignal : IRequestSignal
    {
        /// <summary>
        /// Score/ Points earned from game session
        /// </summary>
        public int Value;

        /// <summary>
        /// GMC event of the request
        /// </summary>
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

    public struct OnUpdateFGCWallet : IRequestSignal
    {
        public string Result;
    }

    public struct OnUpdateFGCCurrency : IRequestSignal
    {
        public string Result;
    }

    [Serializable]
    public class FGCWallet : IJson
    {
        [JProp("update_at")] public string Timestamp;

        [JProp("amount")] public float Amount;
    }
    
    [Serializable]
    public class GenericWallet : IJson
    {
        [JProp("amount")] public int Amount;
    }
    
    [Serializable]
    public class FGCCurrency : IJson
    {
        [Serializable]
        public class Currency
        {
            [JProp("id")] public string Id;

            [JProp("slug")] public string CurrencySlug;

            [JProp("exchange_rate")] public float Rate;
        }

        [JProp("amount")] public int Amount;

        [JProp("updated_at")] public string Timestamp;

        [JProp("currency")] public Currency CurrencyInfo;
    }
    
    [Serializable]
    public class WalletConvert : IJson
    {
        [JProp("created_at")] public string Timestamp;

        [JProp("id")] public string Id;

        [JProp("value")] public float Value;
    }

    public class WalletRequest : UnitRequest
    {
        #region Serializable Classes
        [Serializable]
        public struct GMCError
        {
            public string Message;
        }

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
        public class CurrencyUpdate<T> : IJson
            where T : Currency
        {
            public T currencies;
        }
        #endregion

        #region Constants
        public static readonly string FGC_WALLET_KEY = "FGCWalletKey";
        public static readonly string CURRENCY_KEY = "CurrencyKey";
        public static readonly string HAS_CURRENCY_KEY = "HasCurrencyKey";
        public static readonly string CURRENCY_PARAM = "CurrencyParam";
        #endregion

        #region Fields
        [SerializeField]
        private FGCWallet Wallet;

        [SerializeField]
        private List<FGCCurrency> Currencies;
        #endregion

        #region Unity Life Cycle
        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(FGC_WALLET_KEY);
            QuerySystem.RemoveResolver(CURRENCY_KEY);
            QuerySystem.RemoveResolver(HAS_CURRENCY_KEY);
        }
        #endregion

        #region Initialization
        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            RegisterReceivers();
            RegisterResolvers();
        }

        private void RegisterReceivers()
        {
            this.Receive<OnFetchFGCWalletSignal>()
                .Subscribe(_ => FetchFGCWallet(_.Token))
                .AddTo(this);

            this.Receive<OnFetchCurrencySignal>()
                .Subscribe(_ => FetchCurrencies(_.Token, _.Slug))
                .AddTo(this);

            this.Receive<OnFetchCurrenciesSignal>()
                .Subscribe(_ => FetchWalletAndCurrencies())
                .AddTo(this);

            this.Receive<OnSendToFGCWalletSignal>()
                .Subscribe(_ => SendToFGCWallet<ScoreCurrency>(_.Value, _.Event))
                .AddTo(this);

            this.Receive<OnConvertCurrencySignal>()
                .Subscribe(_ => Convert(_.Token, _.Slug, _.Amount))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => FetchWalletAndCurrencies())
                .AddTo(this);

            this.Receive<OnUpdateFGCWallet>()
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.FGC + "WalletRequest::OnUpdateFGCWallet Result:{0}\n", _.Result);
                    //Result:{"data":{"fgc_wallet":{"amount":0,"updated_at":"2018-10-08T06:11:23.020Z"}

                    var result = new
                    {
                        data = new { fgc_wallet = new FGCWallet() },
                        errors = new[] { new GMCError() }
                    };

                    try
                    {
                        result = JsonConvert.DeserializeAnonymousType(_.Result, result);
                    }
                    catch (JsonReaderException e)
                    {
                        result = null;
                        Debug.LogErrorFormat(D.ERROR + "WalletRequest::OnUpdateFGCWallet Error:{0}\n", e.Message);
                    }
                    finally
                    {
                        if (result != null)
                        {
                            Wallet = result?.data?.fgc_wallet;
                            this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_FGC_WALLET, Data = Wallet });
                        }
                    }
                })
                .AddTo(this);

            this.Receive<OnUpdateFGCCurrency>()
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.FGC + "WalletRequest::OnUpdateFGCCurrency Result:{0}\n", _.Result);
                    //Result:{"data":{"wallet":{"amount":123,"updated_at":"2018-10-08T06:57:56.546Z","currency":{"id":"a1796450-c14d-11e8-b61d-07d71a29b111","slug":"score","exchange_rate":0.05}}}}

                    FGCCurrency wallet = null;
                    string id = null;
                    var result = new
                    {
                        data = new { wallet = new FGCCurrency() },
                        errors = new[] { new GMCError() }
                    };

                    try
                    {
                        result = JsonConvert.DeserializeAnonymousType(_.Result, result);
                    }
                    catch (JsonReaderException e)
                    {
                        result = null;
                        wallet = null;
                        id = null;
                        Debug.LogErrorFormat(D.ERROR + "WalletRequest::OnUpdateFGCCurrency Error:{0}\n", e.Message);
                    }
                    finally
                    {
                        wallet = result?.data?.wallet;
                        id = wallet?.CurrencyInfo?.Id;
                        if (!string.IsNullOrEmpty(id))
                        {
                            Currencies.ReplaceOrAdd<FGCCurrency>(wallet, r => r.CurrencyInfo.Id.Equals(id));
                            this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_CURRENCY, Data = wallet });
                        }
                    }
                })
                .AddTo(this);
        }

        private void RegisterResolvers()
        {
            QuerySystem.RegisterResolver(FGC_WALLET_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.AssertNotNull(Wallet, D.ERROR + "WalletRequest::FGCWallet wallet shoud never be null!\n");
                result.Set(Wallet.Amount);
            });

            QuerySystem.RegisterResolver(CURRENCY_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::Currencies Invalid query! query should contain param! Query:{0} Param:{1}\n", CURRENCY_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);

                FGCCurrency currency = Currencies.Find(c => c.CurrencyInfo.CurrencySlug.Equals(id));
                Assertion.AssertNotNull(currency, D.ERROR + "WalletRequest::Currencies Invalid query param! Currencies does not contain Currency:{0}\n", id);

                result.Set(currency);
            });

            QuerySystem.RegisterResolver(HAS_CURRENCY_KEY, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                Assertion.Assert(request.HasParameter(CURRENCY_PARAM), D.ERROR + "WalletRequest::Currency Invalid query! query should contain param! Query:{0} Param:{1}\n", HAS_CURRENCY_KEY, CURRENCY_PARAM);
                string id = request.GetParameter<string>(CURRENCY_PARAM);
                result.Set(Currencies.Exists(c => c.CurrencyInfo.Id.Equals(id)));
            });
        }
        #endregion

        #region Helpers methods
        private void UpdateWallet(string result)
        {
            //Wallet = wallet;
        }

        private void UpdateCurrencies(string result)
        {
            //currencies.ForEach(c => Currencies.ReplaceOrAdd(c, i => i.CurrencyInfo.Id.Equals(c.CurrencyInfo.Id)));
        }
        #endregion

        #region Requests
        /// <summary>
        /// Fetches the user's Ticket/Stamp wallet from the GMC server
        /// </summary>
        /// <param name="token">user token</param>
        private void FetchFGCWallet(string token)
        {
            Builder builder = Builder.Query();
            builder
                .CreateFunction("fgc_wallet")
                .AddString("token", token);
            builder.CreateReturn("amount", "updated_at");

            ProcessRequest(GraphInfo, builder.ToString(), FetchFGCWalletResult);
        }

        /// <summary>
        /// Fetches the user's currency wallet from the GMC server
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="currencySlug">slug of the currency</param>
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
        /// Sends the GameScore to FGC's currency
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
        /// Sends the GameScore to FGC's currency
        /// </summary>
        /// <param name="token">User Token</param>
        /// <param name="slug">Event slug</param>
        /// <param name="payload">Json data to send</param>
        private void SendToFGCWallet(string token, string slug, IJson payload)
        {
            SendToFGCWallet(token, slug, payload.ToJson());
        }

        /// <summary>
        /// Sends the GameScore to FGC's currency
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

        /// <summary>
        /// Converts user currency to Token/Stamp
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="currencySlug">slug of the currency to be converted</param>
        /// <param name="amount">the amount of currency to be converted</param>
        private void Convert(string token, string currencySlug, int amount)
        {
            Debug.LogErrorFormat("WalletRequest::Convert wallet_convert Slug:{0} Amount:{1}\n", currencySlug, amount);

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
                Wallet = result.Result.Data.FGCWallet;
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
                FGCCurrency wallet = result.Result.Data.Wallet;
                Currencies.ReplaceOrAdd<FGCCurrency>(wallet, r => r.CurrencyInfo.Id.Equals(wallet.CurrencyInfo.Id));

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
                FGCCurrency wallet = result.Result.Data.Wallet;
                Currencies.ReplaceOrAdd<FGCCurrency>(wallet, r => r.CurrencyInfo.Id.Equals(wallet.CurrencyInfo.Id));

                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.SEND_CURRENCY, Data = wallet });

                // NOTE: +AS:20180906 TEST Fetch re-calculated currencies
                FetchWalletAndCurrencies();
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
                WalletConvert wallet = result.Result.Data.Convert;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.CONVERT_CURRENCY, Data = wallet });

                // NOTE: +AS:20180906 TEST Fetch calculated currencies
                FetchWalletAndCurrencies();
            }
        }
        #endregion

        #region DEBUG
        [Button(ButtonSizes.Medium)]
        public void FetchWalletAndCurrencies()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);

            this.Publish(new OnFetchFGCWalletSignal() { Token = token });
            
            this.Publish(new OnFetchCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.SCORE_SLUG,
            });
        }

        /*
        [Button(ButtonSizes.Medium)]
        public void SendToFGCWallet()
        {
            //ConvertCurrency<Currency1>(100, RGCConst.TEST_EVENT);
            //ConvertCurrency<Currency2>(100, RGCConst.TEST_EVENT);
            //ConvertCurrency<PointCurrency>(100, RGCConst.TEST_EVENT);
            //SendToFGCWallet<ScoreCurrency>(100, RGCConst.GAME_END);
        }
        
        [Button(ButtonSizes.Medium)]
        public void ConvertCurrency()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);
            
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

            this.Publish(new OnConvertCurrencySignal()
            {
                Token = token,
                Slug = RGCConst.SCORE_SLUG,
                Amount = 100,
            });
        }
        //*/
        #endregion
    }
}
