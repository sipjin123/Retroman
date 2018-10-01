using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class StringReference : Reference<string>
    {
        [SerializeField, HideInInspector]
        private StringVariable variable;

        public override Variable<string> Variable
        {
            get { return variable; }
            set { variable = (StringVariable)value; }
        }
    }
}