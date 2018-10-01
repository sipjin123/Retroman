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
    public struct JoinEventDataSignal : IRequestSignal, IJson
	{
		public string announcement_id;

		public string ToJsonString()
		{
			return JsonUtility.ToJson(this);
		}
	}

    [Serializable]
    public class JoinEventResultData : IJson
    {
		public string id;
	}

	public class JoinEventRequest : UnitRequest
    {
        public override void Initialze(GraphInfo info)
		{
			this.Receive<JoinEventDataSignal>()
				.Subscribe(_ => JoinEvent(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), _))
				.AddTo(this);
			
		}

		#region Request
		public void JoinEvent(string token, JoinEventDataSignal data)
		{
			Payload payload = new Payload();
			payload.AddString("message", "Event Register");
			payload.AddJsonString("body", data.ToJsonString());

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
		public void JoinEventResult(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.EVENT_JOIN });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.EVENT_JOIN, Data = result.Result.data.event_join });
            }
		} 
		#endregion
	}
}