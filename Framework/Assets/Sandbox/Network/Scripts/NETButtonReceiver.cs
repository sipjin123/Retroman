using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Query;
using Common.Utils;

using Framework;

using Sandbox.Services;

namespace Sandbox.Network
{
    using UColor = UnityEngine.Color;

    public class NETButtonReceiver : MonoBehaviour
    {
        [SerializeField]
        private Image ConnectionButton;

        private readonly Dictionary<bool, UColor> ColorMap = new Dictionary<bool, UColor>()
        {
            { true, UColor.green },
            { false, UColor.red },
        };

        private void Start()
        {
            this.Receive<OnUpdateInternetConnectionSignal>()
                .Subscribe(_ => ConnectionButton.color = ColorMap[_.HasConnection])
                .AddTo(this);
        }
    }
}