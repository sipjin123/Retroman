using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework;
using UniRx;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public class JoinEventData
	{
		public string announcement_id;

		public string ToJsonString()
		{
			return JsonUtility.ToJson(this);
		}
	}

	public class JoinEventResultData
	{
		public string id;
	}

	public class JoinEventRequest : UnitRequest {

        private ObscuredString Token;

        public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LOGIN)
				.Subscribe(_ => Token = _.GetData<ObscuredString>())
				.AddTo(this);

			this.Receive<JoinEventData>()
				.Subscribe(_ => JoinEvent(Token.GetDecrypted(), _))
				.AddTo(this);
			
		}

		#region Request
		public void JoinEvent(string token, JoinEventData data)
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