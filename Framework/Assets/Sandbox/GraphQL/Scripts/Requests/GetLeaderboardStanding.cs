using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UniRx;
using System;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    [Serializable]
	public class LeaderboardPlayerDetail
	{
		public string id;
		public PlayerProfileRequestData data;
	}

	[Serializable]
	public class LeaderboardStanding
	{
		public string value;
        public string standing;
        public LeaderboardPlayerDetail player;
	}

	public struct RequestLeaderboardDataUpdate
	{

	}

	public class GetLeaderboardStanding : UnitRequest {

        private ObscuredString Token;

        public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LOGIN)
				.Subscribe(_ => Token = _.GetData<ObscuredString>())
				.AddTo(this);

			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LEADERBOARD_TOP)
				.Subscribe(_ => GetLeaderboardAround(Token.GetDecrypted()))
				.AddTo(this);

			this.Receive<RequestLeaderboardDataUpdate>()
				.Subscribe(_sig =>GetLeaderboardAround(Token.GetDecrypted(), 2))
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