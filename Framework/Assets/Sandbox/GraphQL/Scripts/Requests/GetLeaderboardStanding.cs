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
    public struct LeaderboardDataUpdateRequestSignal : IRequestSignal
    {

    }

    [Serializable]
	public class LeaderboardPlayerDetail : IJson
    {
		public string id;
		public PlayerProfileRequestData data;
	}

	[Serializable]
	public class LeaderboardStanding : IJson
    {
		public string value;
        public string standing;
        public LeaderboardPlayerDetail player;
	}
    
	public class GetLeaderboardStanding : UnitRequest
    {
        public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LEADERBOARD_TOP)
				.Subscribe(_ => GetLeaderboardAround(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN)))
				.AddTo(this);

			this.Receive<LeaderboardDataUpdateRequestSignal>()
				.Subscribe(_sig => GetLeaderboardAround(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), 2))
				.AddTo(this);
		}

		#region Request
		public void GetLeaderboardAround(string token, int margin = 1)
		{
			Builder builder = Builder.Query();
			Function function = builder.CreateFunction("leaderboard_standing");
			function.AddString("token", token);
			function.AddString("slug", "top_scorers");
			function.AddNumber("margin", margin);
			Return ret = builder.CreateReturn("value", "standing");
			ret.Add("player", new Return("id"));

			ProcessRequest(GraphInfo, builder.ToString(), UpdateLeaderboardStanding);
		}
		#endregion

		#region Parser
		public void UpdateLeaderboardStanding(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LEADERBOARD_AROUND });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LEADERBOARD_AROUND, Data = result.Result.data.leaderboard_standing });
            }
		} 
		#endregion
	}
}