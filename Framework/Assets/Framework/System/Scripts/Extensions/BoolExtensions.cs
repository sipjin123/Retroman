using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class BoolExtensions
    {
        public const string TRUE = "1";
        public const string FALSE = "0";

        public static string To01(this bool val)
        {
            return val ? TRUE : FALSE;
        }

        public static bool ToBool(this int val)
        {
            return val == 0 ? false : true;
        }
    }
}
