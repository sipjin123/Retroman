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
    public class PlayerIDContainer : IJson
    {
        [JProp("id")] public string Id;
        [JProp("device_id")] public string DeviceId;
        [JProp("facebook_id")] public string FbId;
        [JProp("gamesparks_id")] public string GSId;
    }

    public class PlayerIdRequest : UnitRequest
    {
        public static readonly string PLAYER_SERVER_DATA = "PlayerServerData";

        [SerializeField]
        private PlayerIDContainer PlayerInfo;

        private void Awake()
        {
            QuerySystem.RegisterResolver(PLAYER_SERVER_DATA, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(PlayerInfo);
            });
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(PLAYER_SERVER_DATA);
        }

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

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
                PlayerInfo = result.Result.Data.PlayerInfo;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_DATA, Data = PlayerInfo });
            }
        }

    }
}