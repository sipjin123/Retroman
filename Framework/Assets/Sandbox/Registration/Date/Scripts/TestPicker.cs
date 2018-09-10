using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;

using Framework;

namespace Sandbox.Registration
{
    public class TestPicker : MonoBehaviour
    {
        [SerializeField]
        private NativeDatePicker Picker;

        [SerializeField]
        private Text Text;

        private void Start()
        {
            Assertion.AssertNotNull(Picker);
            Assertion.AssertNotNull(Text);

            Picker.Broker.Receive<OnUpdatePicker>()
                .Subscribe(_ =>
                {
                    DateTime date = _.GetData<DateTime>();
                    Text.text = date.ToLongDateString();
                })
                .AddTo(this);

        }
    }
}