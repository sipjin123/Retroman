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

namespace Sandbox.GraphQL
{
    public struct GraphQLConfigureRequestSignal : IRequestSignal
    {
        public string Token;
    }
    
    [Serializable]
    public class Config : IJson
    {
        public string key;
        public string value;
    }

    public class ConfigurationRequest : UnitRequest
    {
        [SerializeField, ShowInInspector]
        private List<Config> Configs;

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            this.Receive<GraphQLConfigureRequestSignal>()
                .Subscribe(_ => Configure(_.Token))
                .AddTo(this);
        }

        #region Requests
        /// <summary>
        /// query {
        ///     configuration(token: "token_from_player_login")
        ///     {
        ///         key
        ///         value
        ///     }
        /// }
        /// </summary>
        /// <param name="unique_id"></param>
        public void Configure(string token)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("configuration");
            func.AddString("token", token);
            Return ret = builder.CreateReturn("key", "value");
            
            ProcessRequest(GraphInfo, builder.ToString(), GameConfigurations);
        }
        #endregion

        #region Parsers
        /// <summary>
        /// Sample Result
        /// {
        ///  "data": {
        ///    "player_login": {
        ///      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0eXBlIjoicGxheWVyIiwiZXh0cmEiOnsiZ2FtZV9pZCI6ImY2ZGEzZDQwLTFkZjktMTFlOC05YTIyLTdkMDcyZDE4ZTllYiIsImJ1aWxkIjoiMC4wLjEifSwiYXV0aG9yaXplciI6Imd1ZXN0IiwiaWQiOiI0MzBlZjljMC0yMmI1LTExZTgtOWNmZi1kOTRlODkyMjYxMTciLCJpYXQiOjE1MjA1MDI2NzV9.56hHsgThnC4acnlTYmX4D7pw_AbvTxQ9bIzylzoCbK4"
        ///    }
        ///  }
        /// }
        /// </summary>
        /// <param name="result"></param>
        public void GameConfigurations(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.CONFIGURE });
            }
            else
            {
                Configs = result.Result.data.configuration;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.CONFIGURE, Data = Configs });
            }
        }
        #endregion
    }
}
