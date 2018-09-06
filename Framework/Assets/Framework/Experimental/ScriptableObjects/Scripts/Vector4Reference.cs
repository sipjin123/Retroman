using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class Vector4Reference : Reference<Vector4>
    {
        [SerializeField, HideInInspector]
        private Vector4Variable variable;

        public override Variable<Vector4> Variable
        {
            get { return variable; }
            set { variable = (Vector4Variable)value; }
        }
    }
}