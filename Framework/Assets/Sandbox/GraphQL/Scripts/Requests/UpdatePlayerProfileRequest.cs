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
    public class UpdateProfileResult : IJson
	{
	    [JProp("id")] public string Id;
	}

	public class UpdatePlayerProfileRequest : UnitRequest 
	{
	    public override void Initialze(GraphInfo info, GraphRequest request)
	    {
	        base.Initialze(info, request);

            this.Receive<PlayerProfileData>()
                .Subscribe(_ => UpdatePlayerProfile(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), _))
                .AddTo(this);
		}

		public void UpdatePlayerProfile(string token, PlayerProfileData data)
		{
			Payload payload = new Payload();
			payload.AddString("message", "Profile Update");
			payload.AddJsonString("body", data.ToJson());	

			Builder builder = Builder.Mutation();
			Function func = builder.CreateFunction("event_trigger");
			func.AddString("token", token);
			func.AddString("slug", "profile_update");		
			func.Add("payload", payload.ToString());

			Return ret = builder.CreateReturn("id");
			Debug.LogErrorFormat("builder:{0}", builder.ToString());
			ProcessRequest(GraphInfo,builder.ToString() , UpdatePlayerProfile);
		}

		public void UpdatePlayerProfile(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.UPDATE_PROFILE });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.UPDATE_PROFILE, Data = result.Result.Data.ProfileUpdate });


            }
		}

		[Button(ButtonSizes.Medium)]	
		public void TestJsonData()
		{
			PlayerProfileData data = new PlayerProfileData();
			data.Address = "8819 vkfg";
			data.Birthdate = "08261954";
			data.City = "pasay";
			data.Email = "juan.delacruz@synergy88digital.com";
			data.FirstName = "juan";
			data.Gender = "Male";
			data.LastName = "delacruz";
			data.MiddleName = "bonifacio";
			data.Mobile = "09968292812"; 	

			UpdatePlayerProfile(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), data);	
		}
	}
}