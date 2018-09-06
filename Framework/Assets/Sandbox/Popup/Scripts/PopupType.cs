using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

namespace Sandbox.Popup
{
    using JetBrains.Annotations;

    [Serializable]
    public partial class PopupType
    {
        public string Type { get; private set; }
        public int Value { get; private set; }

        public PopupType(string type, int value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type;
        }
    }

    public partial class PopupType
    {
        // TODO: +AS:20180903 Autogenerate this along the Enum Classes
        public static ValueDropdownList<string> PopupList;

        static PopupType()
        {
            PopupList = GeneratePopupList();
        }

        // HACK +AS:20180903 Temporary fixes for Build and Editor errors
        public static ValueDropdownList<string> GeneratePopupList()
        {
            List<string> popups = PopupTypeMap.Keys.ToList();
            ValueDropdownList<string> dropdown = new ValueDropdownList<string>();
            popups.ForEach(s => dropdown.Add(s));

            return dropdown;
        }
        
        public ValueDropdownList<string> GetPopups()
        {
            PopupList = PopupList ?? GeneratePopupList();
            return PopupList;
        }

        private void UpdatePopup()
        {
            UpdatePopup(Type);
        }

        private void UpdatePopup(string type)
        {
            Assertion.Assert(PopupTypeList.Exists(p => p.Type.Equals(type)));

            Type = type;
            Value = PopupTypeList.Find(p => p.Type.Equals(type)).Value;
        }

        private void UpdatePopup(int value)
        {
            Assertion.Assert(PopupTypeList.Exists(p => p.Value.Equals(value)));

            Type = PopupTypeList.Find(p => p.Value.Equals(value)).Type;
            Value = value;
        }
    }
}
