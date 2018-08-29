﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public interface IJson
    {

    }

    public static class JsonExtensions
    {
        public static string ToJson(this IJson json)
        {
            return JsonUtility.ToJson((object)json);
        }

        public static string ToString(this IJson json)
        {
            return JsonUtility.ToJson((object)json);
        }
    }
}