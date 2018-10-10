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

    [Serializable]
    public class PlayerProfileData : IJson
    {
        [JProp("first_name")] public string FirstName;
        [JProp("middle_name")] public string MiddleName;
        [JProp("last_name")] public string LastName;
        [JProp("birthdate")] public string Birthdate;
        [JProp("gender")] public string Gender;
        [JProp("address")] public string Address;
        [JProp("city")] public string City;
        [JProp("mobile_number")] public string Mobile;
        [JProp("email")] public string Email;
    }
    
    [Serializable]
    public class PlayerProfileContainer : IJson
    {
        [JProp("value")] public string Value;
    }

    [Serializable]
    public class PlayerProfileRequestData : IJson
    {
        [JProp("id")] public string Id;
        [JProp("first_name")] public string FirstName;
        [JProp("middle_name")] public string MiddleName;
        [JProp("last_name")] public string LastName;
        [JProp("mobile_number")] public string Mobile;
        [JProp("gender")] public string Gender;
        [JProp("email")] public string Email;
        [JProp("birthdate")] public string Birthdate;
        [JProp("address")] public string Address;
    }

    public class PlayerProfileRequest : UnitRequest
    {
        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                 .Subscribe(_ => RetrievePlayerData(_.GetData<string>()))
                 .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.UPDATE_PROFILE)
                 .Subscribe(_ => RetrievePlayerData(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), true))
                 .AddTo(this);
        }

        #region Request
        public void RetrievePlayerData(string token, bool updateProfile = false)
        {
            Debug.LogErrorFormat(D.CHECK + Time.time + "PlayerProfileRequest::RetrievePlayerData Token:{0}\n", token);

            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("player_getStat");
            func.AddString("token", token);
            func.AddString("key", "profile");
            Return ret = builder.CreateReturn("value");

            if (!updateProfile)
            {
                ProcessRequest(GraphInfo, builder.ToString(), GetPlayerProfile);
            }
            else
            {
                ProcessRequest(GraphInfo, builder.ToString(), UpdatePlayerProfile);
            }
        }

        public void RetrieveLeaderboardPlayerData(string token, string playerID)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("player_getStat");
            func.AddString("token", token);
            func.AddString("key", "profile");
            func.AddString("id", playerID);
            Return ret = builder.CreateReturn("value");
            ProcessRequest(GraphInfo, builder.ToString(), GetLeaderboardPlayerProfile);
        }
        #endregion

        #region Parser
        public void GetPlayerProfile(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAYER_PROFILE });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_PROFILE, Data = result.Result.Data.PlayerProfile });
            }
        }

        public void UpdatePlayerProfile(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAYER_PROFILE });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_PROFILE, Data = result.Result.Data.PlayerProfile });
            }
        }

        public void GetLeaderboardPlayerProfile(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LEADERBOARD_PLAYER_PROFILE });
            }
            else
            {
                PlayerProfileContainer container = result.Result.Data.PlayerProfile;
                
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LEADERBOARD_PLAYER_PROFILE, Data = container.Value });
            }
        }
        #endregion

        #region Debug
        [Button(ButtonSizes.Medium)]
        public void GetPlayerProfile()
        {
            RetrievePlayerData(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN));
        }
        #endregion
    }
}
