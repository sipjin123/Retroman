using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Sandbox.ButtonSandbox
{
    [Serializable]
    public partial class ButtonType
    {
        public string Type { get; private set; }
        public int Value { get; private set; }

        public ButtonType(string type, int value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type;
        }
    }

    public partial class ButtonType
    {
        // TODO: +AS:20180903 Autogenerate this along the Enum Classes
        public static ValueDropdownList<string> ButtonList;

        static ButtonType()
        {
            ButtonList = GenerateButtonList();
        }

        // HACK +AS:20180903 Temporary fixes for Build and Editor errors
        public static ValueDropdownList<string> GenerateButtonList()
        {
            List<string> buttons = ButtonTypeMap.Keys.ToList();
            ValueDropdownList<string> dropdown = new ValueDropdownList<string>();
            buttons.ForEach(s => dropdown.Add(s));

            return dropdown;
        }
        
        public ValueDropdownList<string> GetButtons()
        {
            ButtonList = ButtonList ?? GenerateButtonList();
            return ButtonList;
        }

        private void UpdateButton()
        {
            UpdateButton(Type);
        }

        private void UpdateButton(string type)
        {
            Assertion.Assert(ButtonTypeList.Exists(p => p.Type.Equals(type)));

            Type = type;
            Value = ButtonTypeList.Find(p => p.Type.Equals(type)).Value;
        }

        private void UpdateButton(int value)
        {
            Assertion.Assert(ButtonTypeList.Exists(p => p.Value.Equals(value)));

            Type = ButtonTypeList.Find(p => p.Value.Equals(value)).Type;
            Value = value;
        }
    }
}
