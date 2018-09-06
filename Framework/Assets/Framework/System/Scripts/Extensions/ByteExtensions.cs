using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class ByteExtensions
    {
        public static string ToBaseString(this byte[] val)
        {
            return Convert.ToString(val);
        }

        public static string ToBase64(this byte[] val)
        {
            return Convert.ToBase64String(val);
        }
    }
}
