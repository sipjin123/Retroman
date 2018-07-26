
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework;
using UniRx;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

	public class SignalGetEventPlayers
	{
		public string announcement_id;

		public string ToJsonString()
		{
			return JsonUtility.ToJson(this);
		}
	}

	public class EventsPlayerCount : UnitRequest 
	{
		private ObscuredString Token;

		public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LOGIN)
				.Subscribe(_ => Token = _.GetData<ObscuredString>())
				.AddTo(this);
			
			this.Receive<SignalGetEventPlayers>()
				.Subscribe(_ => GetEventPlayerCount(Token, _))
				.AddTo(this);
		}

		private void GetEventPlayerCount(string token, SignalGetEventPlayers eventID)
		{
			Payload load = new Payload();
			load.AddString("message", "Events Players");
			load.AddJsonString("body", eventID.ToJsonString());

			Builder builder = Builder.Mutation();
			Function function = builder.CreateFunction("event_trigger");
			function.AddString("token", token);
			function.AddString("slug", "events_players");
			function.Add("payload", load.ToString());
			Return ret = builder.CreateReturn("id");
			ProcessRequest(GraphInfo, builder.ToString(), EventsPlayerData);
		}

		private void EventsPlayerData(GraphResult result)
		{
			if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.EVENT_PLAYER_COUNT });
            }
            else
            {
                // Debug.LogError("Success");
            }
		}

	}
}