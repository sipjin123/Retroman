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
    [Serializable]
    public class SelectableData
    {
        public string Info;
    }

    [Serializable]
    public class InputData : SelectableData
    {
        public InputField.ContentType ContentType;
    }

    [Serializable]
    public class DropdownData : SelectableData
    {
        public List<string> Items;
    }
}