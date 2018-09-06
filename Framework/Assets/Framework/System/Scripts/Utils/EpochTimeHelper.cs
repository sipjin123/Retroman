using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Common.Utils;

namespace Framework
{
    public class EpochTimeHelper
    {
        /// <summary>
        /// Total epoch time in Seconds
        /// </summary>
        [SerializeField]
        private long EpochTime = 0L;

        [SerializeField]
        private string EpochTimeKey;

        public EpochTimeHelper(string key)
        {
            EpochTimeKey = key;

            if (!PREFS.HasKey(EpochTimeKey))
            {
                EpochTime = GetEpoch();
                SaveEpoch(EpochTime);
            }
            else
            {
                EpochTime = GetSavedEpoch();
            }

            Debug.LogFormat("EpochTimerHelper::Constructor Key:{0} Sec Difference {1}\n", EpochTimeKey, GetSecondsHavePassed());
        }

        public EpochTimeHelper(string key, long time)
        {
            EpochTimeKey = key;
            EpochTime = time;
            SaveEpoch(EpochTime);
        }

        ~EpochTimeHelper()
        {
            EpochTime = -1L;
            EpochTimeKey = string.Empty;
        }

        public long GetSecondsHavePassed()
        {
            EpochTime = GetSavedEpoch();
            return GetEpoch() - EpochTime;
        }

        /// <summary>
        /// Returns the current epoch time in Seconds
        /// </summary>
        /// <returns></returns>
        public long GetEpoch()
        {
            var now = DateTime.UtcNow;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((now - epoch).TotalSeconds);
        }

        /// <summary>
        /// Save the current epoch (In Seconds) to PREFS
        /// </summary>
        /// <param name="epoch"></param>
        public void SaveEpoch()
        {
            SaveEpoch(GetEpoch());
        }

        /// <summary>
        /// Save the epoch (In Seconds) to PREFS
        /// </summary>
        /// <param name="epoch"></param>
        public void SaveEpoch(long epoch)
        {
            EpochTime = epoch;
            PREFS.SetString(EpochTimeKey, EpochTime.ToString());
        }

        public long GetSavedEpoch()
        {
            string epoch = PREFS.GetString(EpochTimeKey, GetEpoch().ToString());
            return long.Parse(epoch);
        }

        public string NowDateString()
        {
            return DateTime.UtcNow.ToShortDateString();
        }

        public static bool HasTimer(string key)
        {
            return PREFS.HasKey(key);
        }

        public DateTime GetDatetime(long epoch)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(epoch).ToLocalTime();

            return date;
        }

        /// <summary>
        /// if p_time.day < now.day return true
        /// if p_time.day >= now.day return false
        /// </summary>
        /// <param name="p_time"></param>
        /// <returns></returns>
        public bool CompareDatetimeToNow(DateTime time)
        {
            DateTime now = DateTime.UtcNow;
            if ((time.Year <= now.Year)
                && (time.Month <= now.Month)
                && (time.Day < now.Day))
            {
                return true;
            }

            return false;
        }
    }

}