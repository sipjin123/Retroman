using System;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.Popup
{
    [Serializable]
    public class PopupData : ISceneData
    {
        [ShowInInspector]
        private object Data;
        public T GetData<T>() { return (T)Data; }

        public PopupData(object data)
        {
            Data = data;
        }
    }
}