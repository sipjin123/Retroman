using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class BoolReference : Reference<bool>
    {
        [SerializeField, HideInInspector]
        private BoolVariable variable;

        public override Variable<bool> Variable
        {
            get { return variable; }
            set { variable = (BoolVariable)value; }
        }
    }
}