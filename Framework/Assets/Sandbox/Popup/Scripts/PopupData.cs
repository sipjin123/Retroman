﻿using System;
using System.Collections;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.Popup
{
    [Serializable]
    public class PopupData : ISceneData
    {
        public object Data;
        public T GetData<T>() { return (T)Data; }
    }
}