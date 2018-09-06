using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class Vector2IntReference : Reference<Vector2Int>
    {
        [SerializeField, HideInInspector]
        private Vector2IntVariable variable;

        public override Variable<Vector2Int> Variable
        {
            get { return variable; }
            set { variable = (Vector2IntVariable)value; }
        }
    }
}