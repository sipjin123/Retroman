using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public enum AdError
    {
        ADD01,
        ADD02,
        AD003,
    }


    public struct GraphQLGetAllAdsRequestSignal
    {
        public ObscuredString token;
    }

    public struct GrapQLGetRandomAdRequestSignal
    {
        public ObscuredString token;
    }

    public struct GraphQLEndAdSignal
    {
        public ObscuredString token;
        public ObscuredBool was_skipped;
        public ObscuredFloat timemark;
        public AdvertisementPlay AdRequest;
    }

    public struct GraphQLPlayAdRequestSignal
    {
        public ObscuredString token;
        public ObscuredString ad_id;
    }

    [Serializable]
    public class Advertisement
    {
        public string id;
        public string name;
        public string url;
        public string type;
        public bool is_premium;
        public string updated_at;

        public AdType GetAdType()
        {
            return type.ToEnum<AdType>();
        }
    }

    [Serializable]
    public class AdvertisementEnd
    {
        public string id;
        public string status;
    }

    [Serializable]
    public class AdvertisementPlay
    {
        public string id;//transaction id of ad
        public string status;

        public AdvertisementPlay ShallowCopy()
        {
            return (AdvertisementPlay)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class AdvertisementRandom
    {
        public string id;
        public string status;
        public Advertisement advertisement;
    }

    public class AdsRequest : UnitRequest
    {
        #region Debug
        private ObscuredString Token;
        public AdvertisementRandom CurrAd;
        public List<Advertisement> AllAds;
        #endregion

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);
            //also cache user data
            #region GraphQL Requests
            this.Receive<GraphQLGetAllAdsRequestSignal>()
                .Subscribe(_ => GetAllAds(_.token.GetDecrypted()))
                .AddTo(this);

            this.Receive<GrapQLGetRandomAdRequestSignal>()
                .Subscribe(_ => GetRandomAd(_.token.GetDecrypted()))
                .AddTo(this);

            this.Receive<GraphQLEndAdSignal>()
                .Subscribe(_ => EndAd(_.token.GetDecrypted(), _.AdRequest, _.was_skipped, _.timemark))
                .AddTo(this);

            this.Receive<GraphQLPlayAdRequestSignal>()
                .Subscribe(_ => PlayAd(_.token.GetDecrypted(), _.ad_id))
                .AddTo(this);
            #endregion

            #region Debug
            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                 .Subscribe(_ => Token = _.GetData<ObscuredString>())
                 .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.GET_ALL_ADS)
                 .Subscribe(_ => AllAds = _.GetData<List<Advertisement>>())
                 .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.GET_RANDOM_AD)
                 .Subscribe(_ => CurrAd = _.GetData<AdvertisementRandom>())
                 .AddTo(this);
            #endregion
        }

        #region Requests
        public void GetAllAds(string token)
        {
            /*
            string graphArgs =
                        "{\"query\":\"query{" +
                            "advertisements(" +
                                "token:\\\"" + token + "\\\"" +
                            ")" +
                            "{" +
                                "id " +
                                "name " +
                                "url " +
                                "type " +
                                "is_premium " +
                                "updated_at " +
                            "}" +
                        "}\"}";

            ProcessRequest(GraphInfo, graphArgs, GetAllAds);
            //*/
            
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisements");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("id", "name", "url", "type", "is_premium", "updated_at");

            ProcessRequest(GraphInfo, builder.ToString(), GetAllAds);
        }

        public void GetRandomAd(string token)
        {
            /*
            string graphArgs =
            "{\"query\":\"query{" +
                "advertisement_random(" +
                    "token:\\\"" + token + "\\\"" +
                ")" +
                "{ " +
                    "id " +
                    "status " +
                    "advertisement " +
                        "{ " +
                            "id " +
                            "name " +
                            "url " +
                            "type " +
                            "is_premium " +
                        "}" +

                "}" +
            "}\"}";

            ProcessRequest(GraphInfo, graphArgs, GetRandomAd);
            //*/

            /* Sample 001
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisement_random");
            func.AddQuoted("token", token);
            Return ads = new Return();
            ads.Name = "advertisement";
            ads.Add("id");
            ads.Add("name");
            ads.Add("url");
            ads.Add("type");
            ads.Add("is_premium");

            Return ret = builder.CreateReturn("id", "status");
            ret.Add(ads);
            //*/

            //* Sample 002
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisement_random");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("id", "status");
            ret.Add("advertisement", new Return("id", "name", "url", "type", "is_premium"));
            //*/

            ProcessRequest(GraphInfo, builder.ToString(), GetRandomAd);
        }

        public void EndAd(string token, AdvertisementPlay ad_transaction, bool was_skipped, float timemark)
        {
            /*
            string graphArgs =
                    "{\"query\":\"mutation{" +
                        "advertisement_end(" +
                            "token:\\\"" + token + "\\\" " +
                            "id:\\\"" + ad_transaction.id + "\\\" " +
                            "skipped: " + was_skipped.ToString().ToLower() + " " +
                            "timemark: " + timemark + " " +
                        ")" +
                        "{" +
                            "id " +
                            "status " +
                        "}" +
                    "}\"}";

            ProcessRequest(GraphInfo, graphArgs, EndAd, ad_transaction);
            //*/
            
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("advertisement_end");
            func.AddString("token", token);
            func.AddString("id", ad_transaction.id);
            func.Add("skipped", was_skipped.ToString().ToLower());
            func.Add("timemark", timemark);
            Return ret = builder.CreateReturn("id", "status");
            
            ProcessRequest(GraphInfo, builder.ToString(), EndAd, ad_transaction);
        }

        public void PlayAd(string token, string ad_id)
        {
            /*
            string graphArgs =
            "{\"query\":\"query {" +
                "advertisement_play(" +
                    "token:\\\"" + token + "\\\", " +
                    "id:\\\"" + ad_id + "\\\", " +
                ")" +
                "{id status}" +
            "}\"}";

            ProcessRequest(GraphInfo, graphArgs, PlayAd);
            //*/

            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisement_play");
            func.AddString("token", token);
            func.AddString("id", ad_id);
            Return ret = builder.CreateReturn("id", "status");

            ProcessRequest(GraphInfo, builder.ToString(), PlayAd);
        }
        #endregion

        #region Parsers
        public void GetAllAds(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);

            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_ALL_ADS });
            }
            else
            {
                ResultData data = result.Result;
                if (data.data.advertisements == null || !string.IsNullOrEmpty(data.error.message))
                {
                    Debug.LogError(data.error.message);
                    this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_ALL_ADS });
                }
                else
                {
                    this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_ALL_ADS, Data = data.data.advertisements });
                }
            }
        }

        /// <summary>
        /// Sample Result
        ///{
        ///  "data": {
        ///    "advertisement_random": {
        ///      "id": "d4f82d60-36f2-11e8-b3bb-77b436031119",
        ///      "status": "SERVED",
        ///      "advertisement": {
        ///        "id": "c6a0ec10-1df8-11e8-9a22-7d072d18e9eb",
        ///        "name": "Ad2",
        ///        "url": "https://s3-ap-southeast-1.amazonaws.com/gmc-uploads/c6a0ec10-1df8-11e8-9a22-7d072d18e9eb.jpg",
        ///        "type": "image",
        ///        "is_premium": false
        ///      }
        ///    }
        ///  }
        ///}                                                                                                                                                                                                   
        /// </summary>
        /// <param name="result"></param>
        public void GetRandomAd(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);

            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_RANDOM_AD });
            }
            else
            {
                ResultData data = result.Result;

                if (data.data.advertisement_random == null || !string.IsNullOrEmpty(data.error.message))
                {
                    Debug.LogError(data.error.message);
                    this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_RANDOM_AD });
                }
                else
                {
                    this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_RANDOM_AD, Data = data.data.advertisement_random });
                }
            }
        }

        public void EndAd(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                bool CaughtAD003 = CatchError(AdError.AD003,result.Result.errors,delegate(string errormessage, string path)
                {
                        this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.END_AD, Data = 
                        new AdvertisementEnd()
                        {
                            id = ((AdvertisementPlay)result.Data).id,
                            status = "ENDED"
                        } });      
                });
                if(!CaughtAD003)
                {
                    this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.END_AD, Data = result.Data });                
                }
           
            }
            else
            {
                ResultData data = result.Result;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.END_AD, Data = data.data.advertisement_end });
                // if (data.data.advertisement_end == null || !string.IsNullOrEmpty(data.error.message))
                // {
                //     Debug.LogError(data.error.message);
                //     this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.END_AD, Data = result.Data });
                // }
                // else
                // {
                //     this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.END_AD, Data = data.data.advertisement_end });                
                // }
            }
        }

        public void PlayAd(GraphResult result)
        {
            // Assertion.Assert(result.Status == Status.SUCCESS, result.Result);

            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAY_AD });
            }
            else
            {
                ResultData data = result.Result;

                if (data.data.advertisement_play.id == null || !string.IsNullOrEmpty(data.error.message))
                {
                    Debug.LogError(data.error.message);
                    this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAY_AD });
                }
                else
                {
                    this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAY_AD, Data = data.data.advertisement_play });
                }
            }

        }
        #endregion

        #region Debug
        [Button(25)]
        public void GetRandomAd()
        {
            GetRandomAd(Token.GetDecrypted());
        }

        [Button(25)]
        public void GetAllAds()
        {
            GetAllAds(Token.GetDecrypted());
        }
        #endregion
    }
}
