using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Framework;

using Common.Query;

namespace Sandbox.GraphQL
{
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

    public struct GraphQLConfigureRequestSignal : IRequestSignal
    {
        public string Token;
    }
    
    [Serializable]
    public class Config : IJson
    {
        [JProp("key")] public string Key;
        [JProp("value")] public string Value;
    }

    public class ConfigurationRequest : UnitRequest
    {
        public static readonly string CONFIGS = "Configs";
        public static readonly string GRAPH_CONFIGS = "GraphConfigs";

        [SerializeField]
        private List<Config> Configs;
        private GraphConfigs GraphConfigs;

        private void Awake()
        {
            QuerySystem.RegisterResolver(CONFIGS, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Configs);
            });

            QuerySystem.RegisterResolver(GRAPH_CONFIGS, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(GraphConfigs);
            });
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(CONFIGS);
            QuerySystem.RemoveResolver(GRAPH_CONFIGS);
        }

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            GraphConfigs = new GraphConfigs();

            this.Receive<GraphQLConfigureRequestSignal>()
                .Subscribe(_ => Configure(_.Token))
                .AddTo(this);
        }

        #region Requests
        private void Configure(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("configuration");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("key", "value");
            
            ProcessRequest(GraphInfo, builder.ToString(), GameConfigurations);
        }
        #endregion

        #region Parsers
        private void GameConfigurations(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.CONFIGURE });
            }
            else
            {
                Configs = result.Result.Data.Configs;
                GraphConfigs.UpdateConfigs(Configs);
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.CONFIGURE, Data = Configs });
            }
        }
        #endregion
    }
}
