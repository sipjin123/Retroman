using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;

using Framework;

namespace Sandbox.RGC
{
    public class InputSelectable : ConcreteSelectable
    {
        [SerializeField]
        private InputData InputData;
        private InputField InputField;

        protected override void Start()
        {
            InputField = GetSelectable<InputField>();

            base.Start();
            
            ResetData();
        }
        
        protected override void Assertions()
        {
            Assertion.AssertNotNull(Info);
            Assertion.AssertNotNull(Placeholder);
            Assertion.AssertNotNull(Selectable);
            Assertion.AssertNotNull(InputData);
            Assertion.AssertNotNull(InputField);
        }

        public override void OnValueChanged()
        {
            StorageData.Value = InputField.text;

            base.OnValueChanged();
        }

        [Button(ButtonSizes.Medium)]
        public override void ResetData()
        {
            base.ResetData();
            
            Info.text = InputData.Info;
            Placeholder.text = StorageData.Value.ToTitleCase();
            InputField.contentType = InputData.ContentType;
        }
    }
}