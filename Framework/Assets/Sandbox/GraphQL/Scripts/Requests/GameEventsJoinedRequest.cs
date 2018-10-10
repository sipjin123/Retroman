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
	public class GameEventStatus : IJson
    {
		[JProp("id")] public string Id;
        [JProp("code")] public string Code;
	}

	[Serializable]
	public class EventsStatus : IJson
    {
        [JProp("eventsList")] public List<GameEventStatus> Events;
	}
	
	public class GameEventsJoinedRequest : UnitRequest
    {
        private bool IsJoining = false;

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.ANNOUNCEMENTS)
				.Subscribe(_ => CheckGameEventStatus(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN)))
				.AddTo(this);
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.EVENT_JOIN)
                .Subscribe(_ => CheckGameEventStatus(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), true))
                .AddTo(this);
        }
#region Request
		private void CheckGameEventStatus(string token, bool joinedEvent = false)
		{
            IsJoining = joinedEvent;
			Builder builder = Builder.Query();
			Function func = builder.CreateFunction("player_getStat");
			func.AddString("token", token);
			func.AddString("key", "events");
			Return ret = builder.CreateReturn("value");

			ProcessRequest(GraphInfo, builder.ToString(), GameAnnouncements);
		}
#endregion

#region Parser
        public void GameAnnouncements(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
			// Debug.LogError(result.RawResult);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS_STATUS });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS_STATUS, Data = result.Result.Data.PlayerProfile });
            }
        }
#endregion
	}
}