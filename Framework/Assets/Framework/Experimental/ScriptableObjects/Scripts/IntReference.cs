using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class IntReference : Reference<int>
    {
        [SerializeField, HideInInspector]
        private IntVariable variable;

        public override Variable<int> Variable
        {
            get { return variable; }
            set { variable = (IntVariable)value; }
        }
    }
}