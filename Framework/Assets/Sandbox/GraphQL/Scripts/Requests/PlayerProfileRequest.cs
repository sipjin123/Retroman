using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

using Framework;
using UniRx;
using MiniJSON;
using Sandbox.Popup;


namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;
    
    [Serializable]
    public class PlayerProfileData
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

        public string ToJsonString()
        {
            // Dictionary<object, object> playerInfo = new Dictionary<object, object>();
            // playerInfo.Add("first_name", this.first_name);
            // playerInfo.Add("middle_name", this.middle_name);
            // playerInfo.Add("last_name", this.last_name);
            // playerInfo.Add("birthdate", this.birthdate);
            // playerInfo.Add("gender", this.gender);
            // playerInfo.Add("address", this.address);
            // playerInfo.Add("city", this.city);
            // playerInfo.Add("mobile_number", this.mobile_number);
            // playerInfo.Add("email", this.email);
            string json = JsonUtility.ToJson(this);
            return json;
        }
    }

    

    [Serializable]
    public class PlayerProfileContainer
    {
        public string value;
    }

    [Serializable]
    public class PlayerProfileRequestData
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
        private ObscuredString Token;

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                 .Subscribe(_ => 
                 {
                     Token = _.GetData<ObscuredString>();
                     RetrievePlayerData(Token.GetDecrypted());
                 })
                 .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.UPDATE_PROFILE)
                 .Subscribe(_ => RetrievePlayerData(Token.GetDecrypted(), true))
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
            RetrievePlayerData(Token.GetDecrypted());
        }
        #endregion
    }
}
