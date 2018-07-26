using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework;
using UniRx;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public struct RequestGraphQLLeaderboard
	{

	}

	public class GetLeaderboardTop : UnitRequest
	 {
		private ObscuredString Token;

		public override void Initialze(GraphInfo info)
		{
			this.Receive<GraphQLRequestSuccessfulSignal>()
				.Where(_ => _.Type == GraphQLRequestType.LOGIN)
				.Subscribe(_ => Token = _.GetData<ObscuredString>())
				.AddTo(this);

			this.Receive<RequestGraphQLLeaderboard>()
				.Subscribe(_ => GetLeaderboardTopRank(Token.GetDecrypted()))
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