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
    public class PlayerIDContainer
    {
        public string id;
    }

    public class PlayerIdRequest : UnitRequest
    {
        private ObscuredString Token;

        public override void Initialze(GraphInfo info)
        {
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
               .Subscribe(_ =>
               {
                   Token = _.GetData<ObscuredString>();
                   RequestPlayerID(Token.GetDecrypted());
               }).AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.PLAYER_PROFILE)
                .Subscribe(_ => RequestPlayerID(Token.GetDecrypted()))
                .AddTo(this);
        }

        private void RequestPlayerID(string token)
        {
            Builder builder = Builder.Query();
            Function function = builder.CreateFunction("player");
            function.AddString("token", token);
            Return ret = builder.CreateReturn("id");
            ProcessRequest(GraphInfo, builder.ToString(), ProcessPlayerID);
        }

        private void ProcessPlayerID(GraphResult result)
        {
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAYER_DATA });
            }
            else
            {
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_DATA, Data = result.Result.data.player });
            }
        }

    }
}