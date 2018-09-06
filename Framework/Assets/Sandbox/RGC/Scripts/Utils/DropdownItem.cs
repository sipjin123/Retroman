using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;

using Framework;

namespace Sandbox.RGC
{
    using UColor = UnityEngine.Color;

    public class DropdownItem : MonoBehaviour
    {
        [SerializeField]
        private Image Background;

        [SerializeField]
        private Text Text;

        private void Awake()
        {
            Assertion.AssertNotNull(Background);
            Assertion.AssertNotNull(Text);

            Text.ObserveEveryValueChanged(t => t.text)
                .Subscribe(_ => Text.text = _.ToTitleCase())
                .AddTo(this);
        }

        private void OnEnable()
        {
            Background.gameObject.SetActive(transform.GetSiblingIndex() % 2 == 0);
        }
    }
}