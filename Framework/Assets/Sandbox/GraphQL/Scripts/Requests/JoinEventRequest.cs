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
    public struct JoinEventDataSignal : IRequestSignal, IJson
	{
	    [JProp("announcement_id")] public string Id;
	}

    [Serializable]
    public class JoinEventResultData : IJson
    {
        [JProp("id")] public string Id;
	}

	public class JoinEventRequest : UnitRequest
    {
        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            this.Receive<JoinEventDataSignal>()
				.Subscribe(_ => JoinEvent(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), _))
				.AddTo(this);
			
		}

		#region Request
		private void JoinEvent(string token, JoinEventDataSignal data)
		{
			Payload payload = new Payload();
			payload.AddString("message", "Event Register");
			payload.AddJsonString("body", data.ToJson());

			Builder builder = Builder.Mutation();
			Function function = builder.CreateFunction("event_trigger");
			function.AddString("token", token);
			function.AddString("slug", "event_register");
			function.Add("payload", payload.ToString());
			Return ret = builder.CreateReturn("id");

			ProcessRequest(GraphInfo, builder.ToString(), JoinEventResult);
		}
		#endregion

		#region Parser
		private void JoinEventResult(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.EVENT_JOIN });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.EVENT_JOIN, Data = result.Result.Data.JoinEventResult });
            }
		} 
		#endregion
	}
}