using System;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [Serializable]
    public class Vector2Reference : Reference<Vector2>
    {
        [SerializeField, HideInInspector]
        private Vector2Variable variable;

        public override Variable<Vector2> Variable
        {
            get { return variable; }
            set { variable = (Vector2Variable)value; }
        }
    }
}