using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework;
using UniRx;
using System;
using Sandbox.Popup;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;


    [Serializable]
	public class GameEventStatus
	{
		public string id;
		public string code;
	}

	[Serializable]
	public class EventsStatus
	{
		public List<GameEventStatus> eventsList;
	}
	
	public class GameEventsJoinedRequest : UnitRequest {

		private ObscuredString Token;
        private bool IsJoining = false;

		public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LOGIN)
				.Subscribe(_ => Token = _.GetData<ObscuredString>())
				.AddTo(this);

			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.ANNOUNCEMENTS)
				.Subscribe(_ => CheckGameEventStatus(Token.GetDecrypted()))
				.AddTo(this);
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.EVENT_JOIN)
                .Subscribe(_ => CheckGameEventStatus(Token.GetDecrypted(), true))
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
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS_STATUS, Data = result.Result.data.player_getStat });
            }
        }
#endregion
	}
}