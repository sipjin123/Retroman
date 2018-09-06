using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;
using Common.Utils;

using Sandbox.GraphQL;

namespace Framework
{
    public static class PREFS
    {
        public static float GetFloat(string key, [DefaultValue("-1.0F")] float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static float GetFloat(string key)
        {
            return PlayerPrefs.GetFloat(key, -1f);
        }

        public static int GetInt(string key, [DefaultValue("-1")] int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key, -1);
        }

        public static string GetString(string key, [DefaultValue("\"\"")] string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(key, string.Empty);
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static bool TryGet(string key, ref float ret)
        {
            if (!HasKey(key))
            {
                return false;
            }

            ret = GetFloat(key);
            return ret >= 0f;
        }

        public static bool TryGet(string key, ref int ret)
        {
            if (!HasKey(key))
            {
                return false;
            }

            ret = GetInt(key);
            return ret >= 0;
        }

        public static bool TryGet(string key, ref string ret)
        {
            if (!HasKey(key))
            {
                return false;
            }

            ret = GetString(key);
            return !string.IsNullOrEmpty(ret);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}