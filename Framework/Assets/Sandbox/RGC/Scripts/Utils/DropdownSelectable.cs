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
    public class DropdownSelectable : ConcreteSelectable
    {
        [SerializeField]
        private DropdownData Options;
        private Dropdown Dropdown;

        protected override void Start()
        {
            Dropdown = GetSelectable<Dropdown>();

            base.Start();
            
            Assertions();
            ResetData();

            Dropdown.captionText
                .ObserveEveryValueChanged(t => t.text)
                .Subscribe(_ => Dropdown.captionText.text = _.ToTitleCase())
                .AddTo(this);
        }

        protected override void Assertions()
        {
            Assertion.AssertNotNull(Info);
            Assertion.AssertNotNull(Selectable);
            Assertion.AssertNotNull(Options);
            Assertion.AssertNotNull(Dropdown);
        }

        public override void OnValueChanged()
        {
            StorageData.Value = Options.Items[Dropdown.value];
            Dropdown.captionText.text = StorageData.Value.ToTitleCase();

            base.OnValueChanged();
        }

        public override void SetData<T>(T data)
        {
            string d = data.ToString();
           
            List<Dropdown.OptionData> options = Dropdown.options;
            int index = options.FindIndex(o => o.text.ToLower().Equals(d.ToLower()));

            Assertion.Assert(index >= 0, "DropdownSelectable::SetData Invalid data! Data:{0}\n", d);

            Select(index);
        }
        
        [Button(ButtonSizes.Medium)]
        public override void Select()
        {
            Dropdown.Select();
            Dropdown.Show();
        }

        [Button(ButtonSizes.Medium)]
        public override void ResetData()
        {
            base.ResetData();

            string lower = StorageData.Value.ToLower();
            string title = StorageData.Value.ToTitleCase();

            Info.text = Options.Info;
            Dropdown.ClearOptions();
            Dropdown.AddOptions(Options.Items);
            Dropdown.value = Options.Items.FindIndex(i => i.ToLower().Equals(lower));
            Dropdown.captionText.text = title;
        }
        
        private void Select(int index)
        {
            if (index >= 0)
            {
                Dropdown.value = index;
                Dropdown.captionText.text = Options.Items[index].ToTitleCase();
            }
        }
    }
}