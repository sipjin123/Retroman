using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using Framework.Core;

namespace Framework
{
    public static class FloatExtensions
    {
        public static float Random(this float max)
        {
            return FRandom.Range(0f, max);
        }
        
        public static bool IsEqual(this float f1, float f2)
        {
            return f1 >= f2 - Mathf.Epsilon && f1 <= f2 + Mathf.Epsilon;
        }

        public static int ToInt(this float f)
        {
            return Mathf.CeilToInt(f);
        }

        public static float Floor(this float f)
        {
            return Mathf.Floor(f);
        }

        public static float Ceil(this float f)
        {
            return Mathf.Ceil(f);
        }
    }
}
