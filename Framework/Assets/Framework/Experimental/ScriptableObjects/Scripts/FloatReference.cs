using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class FloatReference : Reference<float>
    {
        [SerializeField, HideInInspector]
        public FloatVariable variable;

        public override Variable<float> Variable
        {
            get { return variable; }
            set { variable = (FloatVariable)value; }
        }
    }
}