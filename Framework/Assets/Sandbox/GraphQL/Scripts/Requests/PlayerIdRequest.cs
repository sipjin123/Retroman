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
    [Serializable]
    public class PlayerIDContainer : IJson
    {
        public string id;
        public string device_id;
        public string facebook_id;
        public string gamesparks_id;
    }

    public class PlayerIdRequest : UnitRequest
    {
        [SerializeField]
        private PlayerIDContainer PlayerInfo;

        public override void Initialze(GraphInfo info)
        {
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
               .Subscribe(_ => RequestPlayerID(_.GetData<string>()))
               .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.PLAYER_PROFILE)
                .Subscribe(_ => RequestPlayerID(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN)))
                .AddTo(this);
        }

        private void RequestPlayerID(string token)
        {
            Builder builder = Builder.Query();
            Function function = builder.CreateFunction("player");
            function.AddString("token", token);
            Return ret = builder.CreateReturn("id", "device_id", "facebook_id", "gamesparks_id");

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
                PlayerInfo = result.Result.data.player;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_DATA, Data = PlayerInfo });
            }
        }

    }
}