using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Advertisements;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

    public enum AdError
    {
        ADD01,
        ADD02,
        AD003,
    }
    
    public struct GraphQLGetAllAdsRequestSignal : IRequestSignal
    {
        public string token;
    }

    public struct GrapQLGetRandomAdRequestSignal : IRequestSignal
    {
        public string token;
    }

    public struct GraphQLEndAdSignal : IRequestSignal
    {
        public string token;
        public bool was_skipped;
        public float timemark;
        public AdvertisementPlay AdRequest;
    }

    public struct GraphQLPlayAdRequestSignal : IRequestSignal
    {
        public string token;
        public string ad_id;
    }

    [Serializable]
    public class Advertisement : IJson
    {
        [JProp("id")] public string Id;
        [JProp("name")] public string Name;
        [JProp("url")] public string Url;
        [JProp("type")] public string Type;
        [JProp("is_premium")] public bool IsPremium;
        [JProp("updated_at")] public string Timestamp;

        public AdType GetAdType()
        {
            return this.Type.ToEnum<AdType>();
        }
    }

    [Serializable]
    public class AdvertisementEnd : IJson
    {
        [JProp("id")] public string Id;
        [JProp("status")] public string Status;
    }

    [Serializable]
    public class AdvertisementPlay : IJson
    {
        //transaction id of ad
        [JProp("id")] public string Id;
        //[JProp("status")] public string Status;

        public AdvertisementPlay ShallowCopy()
        {
            return (AdvertisementPlay)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class AdvertisementRandom : IJson
    {
        [JProp("id")] public string Id;
        [JProp("status")] public string Status;
        [JProp("advertisement")] public Advertisement Ad;
    }

    public class AdsRequest : UnitRequest
    {
        #region Debug
        public AdvertisementRandom CurrAd;
        public List<Advertisement> AllAds;
        #endregion

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);
            //also cache user data
            #region GraphQL Requests
            this.Receive<GraphQLGetAllAdsRequestSignal>()
                .Subscribe(_ => GetAllAds(_.token))
                .AddTo(this);

            this.Receive<GrapQLGetRandomAdRequestSignal>()
                .Subscribe(_ => GetRandomAd(_.token))
                .AddTo(this);

            this.Receive<GraphQLEndAdSignal>()
                .Subscribe(_ => EndAd(_.token, _.AdRequest, _.was_skipped, _.timemark))
                .AddTo(this);

            this.Receive<GraphQLPlayAdRequestSignal>()
                .Subscribe(_ => PlayAd(_.token, _.ad_id))
                .AddTo(this);
            #endregion

            #region Debug
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
        private void GetAllAds(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisements");
            func.AddString("token", token);
            //builder.CreateReturn("id", "name", "url", "type", "is_premium", "updated_at");
            builder.CreateReturn("id", "name", "url", "type");

            ProcessRequest(GraphInfo, builder.ToString(), GetAllAds);
        }

        private void GetRandomAd(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisement_random");
            func.AddString("token", token);
            //Return ret = builder.CreateReturn("id", "status");
            Return ret = builder.CreateReturn("id");
            //ret.Add("advertisement", new Return("id", "name", "url", "type", "is_premium"));
            ret.Add("advertisement", new Return("id", "name", "url", "type"));

            ProcessRequest(GraphInfo, builder.ToString(), GetRandomAd);
        }

        private void EndAd(string token, AdvertisementPlay ad_transaction, bool was_skipped, float timemark)
        {
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("advertisement_end");
            func.AddString("token", token);
            func.AddString("id", ad_transaction.Id);
            func.Add("skipped", was_skipped.ToString().ToLower());
            func.Add("timemark", timemark);
            //builder.CreateReturn("id", "status");
            builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), EndAd, ad_transaction);
        }

        private void PlayAd(string token, string ad_id)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("advertisement_play");
            func.AddString("token", token);
            func.AddString("id", ad_id);
            //builder.CreateReturn("id", "status");
            builder.CreateReturn("id");

            ProcessRequest(GraphInfo, builder.ToString(), PlayAd);
        }
        #endregion

        #region Parsers
        private void GetAllAds(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_ALL_ADS });
            }
            else
            {
                ResultData data = result.Result;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_ALL_ADS, Data = data.Data.Ads });
            }
        }

        private void GetRandomAd(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.GET_RANDOM_AD });
            }
            else
            {
                ResultData data = result.Result;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.GET_RANDOM_AD, Data = data.Data.RandomAd });
            }
        }

        private void EndAd(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                bool CaughtAD003 = CatchError(AdError.AD003, result.Result.Errors, delegate(string errormessage, string path)
                {
                    this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.END_AD, Data = 
                    new AdvertisementEnd()
                    {
                        Id = ((AdvertisementPlay)result.Data).Id,
                        Status = "ENDED"
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
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.END_AD, Data = data.Data.EndAd });
            }
        }

        private void PlayAd(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAY_AD });
            }
            else
            {
                ResultData data = result.Result;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAY_AD, Data = data.Data.PlayAd });
            }

        }
        #endregion

        #region Debug
        [Button(ButtonSizes.Medium)]
        public void GetRandomAd()
        {
            GetRandomAd(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN));
        }

        [Button(ButtonSizes.Medium)]
        public void GetAllAds()
        {
            GetAllAds(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN));
        }
        #endregion
    }
}
