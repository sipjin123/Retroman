using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Framework;

using Common;
using Common.Query;

using UniRx;

namespace Sandbox.GraphQL
{
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

    [Serializable]
	public class GetEventPlayersSignal : IRequestSignal, IJson
    {
		public string announcement_id;

		public string ToJsonString()
		{
			return JsonUtility.ToJson(this);
		}
	}

	public class EventsPlayerCount : UnitRequest 
	{
		public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

			this.Receive<GetEventPlayersSignal>()
				.Subscribe(_ => GetEventPlayerCount(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), _))
				.AddTo(this);
		}

		private void GetEventPlayerCount(string token, GetEventPlayersSignal eventID)
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
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.EVENT_PLAYER_COUNT });
            }
		}

	}
}