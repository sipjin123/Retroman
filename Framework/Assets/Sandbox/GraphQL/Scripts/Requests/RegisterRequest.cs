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

using Common;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public struct GraphQLLoginRequestSignal
    {
        public ObscuredString unique_id;
    }

    [Serializable]
    public class PlayerLogin
    {
        public string token;
    }

    public class RegisterRequest : UnitRequest
    {
        public static readonly string PLAYER_TOKEN = "PlayerToken";

        private ObscuredString Token;

        private void Awake()
        {
            QuerySystem.RegisterResolver(PLAYER_TOKEN, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Token.GetDecrypted());
            });
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(PLAYER_TOKEN);
        }

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            this.Receive<GraphQLLoginRequestSignal>()
                //.Subscribe(_ => Register(_.unique_id))
                .Subscribe(_ => Register(string.Format("{0}Randomize{1}", UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0,10000))))
                .AddTo(this);
        }

        #region Requests
        /// <summary>
        /// mutation {
        ///    player_login(
        ///        type: guest
        ///        credential: "device_id"
        ///        device_id: "abcd1234"
        ///        game_slug: "gnt"
        ///        build: "1.0.0"
        ///    )
        ///    {
        ///        token
        ///            
        ///     }
        ///}
        /// </summary>
        /// <param name="unique_id"></param>
        public void Register(string unique_id)
        {
            string guest = "guest";
            string credentials = unique_id;
            string deviceId = unique_id;
            string gameSlug = GraphInfo.GameSlug;
            string build = GraphInfo.Build;
            string token = "token";

            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("player_login");
            func.Add("type", guest);
            func.AddString("credential", credentials);
            func.AddString("device_id", deviceId);
            func.AddString("game_slug", gameSlug);
            //func.AddQuoted("build", build);
            Return ret = builder.CreateReturn(token);
            string args = builder.ToString();

            Debug.LogErrorFormat("Request:{0}\n", args);
            
            ProcessRequest(GraphInfo, args, PlayerLogin);
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
        public void PlayerLogin(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOGIN });
            }
            else
            {
                Token = result.Result.data.player_login.token;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOGIN, Data = Token });
            }
        }
        #endregion

        #region Debug
        [Button(25)]
        public void Register()
        {
            Register(Platform.DeviceId);
        }
        #endregion
    }
}
