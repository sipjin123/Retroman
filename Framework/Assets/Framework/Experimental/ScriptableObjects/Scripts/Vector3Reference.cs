using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class Vector3Reference : Reference<Vector3>
    {
        [SerializeField, HideInInspector]
        private Vector3Variable variable;

        public override Variable<Vector3> Variable
        {
            get { return variable; }
            set { variable = (Vector3Variable)value; }
        }
    }
}