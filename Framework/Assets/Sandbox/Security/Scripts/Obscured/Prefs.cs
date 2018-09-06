using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;

namespace Sandbox.Security
{
    using CodeStage.AntiCheat.ObscuredTypes;
    
    public class Prefs
    {
        public static float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue)
        {
            return ObscuredPrefs.GetFloat(key, defaultValue);
        }
       
        public static float GetFloat(string key)
        {
            return ObscuredPrefs.GetFloat(key);
        }

        public static int GetInt(string key, [DefaultValue("0")] int defaultValue)
        {
            return ObscuredPrefs.GetInt(key, defaultValue);
        }

        public static int GetInt(string key)
        {
            return ObscuredPrefs.GetInt(key);
        }

        public static string GetString(string key, [DefaultValue("\"\"")] string defaultValue)
        {
            return ObscuredPrefs.GetString(key, defaultValue);
        }

        public static string GetString(string key)
        {
            return ObscuredPrefs.GetString(key);
        }

        public static bool HasKey(string key)
        {
            return ObscuredPrefs.HasKey(key);
        }

        public static void SetFloat(string key, float value)
        {
            ObscuredPrefs.SetFloat(key, value);
        }

        public static void SetInt(string key, int value)
        {
            ObscuredPrefs.SetInt(key, value);
        }

        public static void SetString(string key, string value)
        {
            ObscuredPrefs.SetString(key, value);
        }

        public static void Save()
        {
            ObscuredPrefs.Save();
        }
    }
}