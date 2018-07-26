using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class IntExtensions
    {
        public static T ToEnum<T>(this int intValue) where T : struct, IConvertible
        {
            Assertion.Assert(typeof(T).IsEnum, D.ERROR + "T:{0} is not an Enum! intValue:{1}", typeof(T), intValue);
            return (T)Enum.ToObject(typeof(T), intValue);
        }
    }
}
