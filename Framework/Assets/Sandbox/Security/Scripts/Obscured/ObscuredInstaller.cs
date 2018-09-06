using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using CodeStage.AntiCheat.ObscuredTypes;

using Common;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;

namespace Sandbox.Security
{
    public class ObscuredInstaller : ConcreteInstaller
    {
        private readonly int INT_KEY = 434523;
        private readonly string STR_KEY = "gnt";

        public override void Install()
        {
            base.Install();

            ObscuredInt.SetNewCryptoKey(INT_KEY);
            ObscuredFloat.SetNewCryptoKey(INT_KEY);
            ObscuredDouble.SetNewCryptoKey(INT_KEY);
            ObscuredDecimal.SetNewCryptoKey(INT_KEY);
            ObscuredLong.SetNewCryptoKey(INT_KEY);
            ObscuredVector2.SetNewCryptoKey(INT_KEY);
            ObscuredVector3.SetNewCryptoKey(INT_KEY);
            ObscuredVector2Int.SetNewCryptoKey(INT_KEY);
            ObscuredVector3Int.SetNewCryptoKey(INT_KEY);
            ObscuredString.SetNewCryptoKey(STR_KEY);
        }
    }
}