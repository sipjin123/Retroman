using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime FROM_JAN_1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long FromJan1970(this DateTime date)
        {
            return (long)(date - FROM_JAN_1970).TotalMilliseconds;
        }
    }
}
