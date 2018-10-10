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

    public struct LeaderboardDataUpdateRequestSignal : IRequestSignal
    {

    }

    [Serializable]
	public class LeaderboardPlayerDetail : IJson
    {
		[JProp("id")] public string Id;
        [JProp("data")] public PlayerProfileRequestData Profile;
	}

	[Serializable]
	public class LeaderboardStanding : IJson
    {
		[JProp("value")] public string Value;
        [JProp("standing")] public string Standing;
        [JProp("player")] public LeaderboardPlayerDetail Player;
	}
    
	public class GetLeaderboardStanding : UnitRequest
    {
        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LEADERBOARD_TOP)
				.Subscribe(_ => GetLeaderboardAround(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN)))
				.AddTo(this);

			this.Receive<LeaderboardDataUpdateRequestSignal>()
				.Subscribe(_sig => GetLeaderboardAround(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), 2))
				.AddTo(this);
		}

        #region Request
        private void GetLeaderboardAround(string token, int margin = 1)
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
        private void UpdateLeaderboardStanding(GraphResult result)
		{
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LEADERBOARD_AROUND });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LEADERBOARD_AROUND, Data = result.Result.Data.LeaderboardStandings });
            }
		} 
		#endregion
	}
}