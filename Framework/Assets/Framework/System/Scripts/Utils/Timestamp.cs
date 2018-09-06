using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using Common;
using Common.Signal;

namespace Framework
{
    [Serializable]
    public class Timestamp
    {
        private string Key = "Timestamp";

        private float RootTime;

        public Timestamp(string key)
        {
            Key = key;
        }

        public void Record()
        {
            RootTime = GetTime();
        }

        public void Record(int lapsed)
        {
            RootTime = GetTime() - lapsed;
        }

        public int Lapsed()
        {
            return Mathf.FloorToInt(GetTime() - RootTime);
        }

        private float GetTime()
        {
            return Time.time * 1000;
        }
    }
}