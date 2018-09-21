using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.GraphQL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    // alias
    using UScene = UnityEngine.SceneManagement.Scene;

    public class TestBuilder : SceneObject
    {
        protected override void Awake()
        {
            base.Awake();

            string guest = "guest";
            string credentials = "device_id";
            string deviceId = "abcd1234";
            string gameSlug = "gnt";
            string token = "token";
            
            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("player_login");
            func.Add("type", guest);
            func.AddString("credential", credentials);
            func.AddString("device_id", deviceId);
            func.AddString("game_slug", gameSlug);
            Return ret = builder.CreateReturn(token);

            Debug.LogError("A " + builder.ToString() + "\n");
        }
    }
}

