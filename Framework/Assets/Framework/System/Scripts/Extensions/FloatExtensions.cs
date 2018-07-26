using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class FloatExtensions
    {
        public static bool IsEqual(this float f1, float f2)
        {
            return f1 >= f2 - Mathf.Epsilon && f1 <= f2 + Mathf.Epsilon;
        }
    }
}
