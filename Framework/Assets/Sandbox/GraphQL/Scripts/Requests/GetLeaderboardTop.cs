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
    public struct GraphQLLeaderboardRequestSignal : IRequestSignal
    {

	}

	public class GetLeaderboardTop : UnitRequest
	 {
		public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLLeaderboardRequestSignal>()
				.Subscribe(_ => GetLeaderboardTopRank(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN)))
				.AddTo(this);
		}

		#region Request 
		public void GetLeaderboardTopRank(string token)
		{
			Builder builder = Builder.Query();
			Function function = builder.CreateFunction("leaderboard_players");
			function.AddString("token", token);
			function.AddString("slug", "top_scorers");
			function.AddNumber("top", 50);
			Return ret = builder.CreateReturn("value", "standing");
			ret.Add("player", new Return("id"));

			ProcessRequest(GraphInfo, builder.ToString(), UpdateLeaderboardTopRanking);
		}
		#endregion

		#region Parser
		public void UpdateLeaderboardTopRanking(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LEADERBOARD_TOP });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LEADERBOARD_TOP, Data = result.Result.data.leaderboard_players });
            }
		} 
		#endregion

	}
}