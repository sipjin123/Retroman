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
    [Serializable]
    public class PlayerProfileData : IJson
    {
        public string first_name;
        public string middle_name;
        public string last_name;
        public string birthdate;
        public string gender;
        public string address;
        public string city;
        public string mobile_number;
        public string email;
    }
    
    [Serializable]
    public class PlayerProfileContainer : IJson
    {
        public string value;
    }

    [Serializable]
    public class PlayerProfileRequestData : IJson
    {
        public string id;
        public string first_name;
        public string middle_name;
        public string last_name;
        public string mobile_number;
        public string gender;
        public string email;
        public string birthdate;
        public string address;
    }

    public class PlayerProfileRequest : UnitRequest
    {
        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

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
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_PROFILE, Data = result.Result.data.player_getStat });
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
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_PROFILE, Data = result.Result.data.player_getStat });
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
                PlayerProfileContainer container = result.Result.data.player_getStat;
                
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LEADERBOARD_PLAYER_PROFILE, Data = container.value });
            }
        }
        #endregion

        #region Debug
        [Button(25)]
        public void GetPlayerProfile()
        {
            RetrievePlayerData(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN));
        }
        #endregion
    }
}
