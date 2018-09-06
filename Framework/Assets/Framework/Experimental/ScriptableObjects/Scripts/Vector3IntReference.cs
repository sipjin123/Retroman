using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class Vector3IntReference : Reference<Vector3Int>
    {
        [SerializeField, HideInInspector]
        private Vector3IntVariable variable;

        public override Variable<Vector3Int> Variable
        {
            get { return variable; }
            set { variable = (Vector3IntVariable)value; }
        }
    }
}