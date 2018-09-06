using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;

namespace Framework
{
    public class AnimHash
    {
        public string Name { get; }
        public int Value { get; }

        public AnimHash(string name)
        {
            Name = name;
            Value = Animator.StringToHash(name);
        }
    }
}