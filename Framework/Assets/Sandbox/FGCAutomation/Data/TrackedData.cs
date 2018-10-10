using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Sandbox.FGCAutomation.Data
{
    public struct TrackedData
    {
        public bool IsAverage { get; set; }

        [ShowInInspector]
        public float AverageScore { get; set; }

        [ShowInInspector]
        public float AverageStamps { get; set; }

        [ShowInInspector]
        public int Score { get; set; }

        [ShowInInspector]
        public int StampsInt { get; set; }

        [ShowInInspector]
        public float StampsFloat { get; set; }

        public void Reset()
        {
            Score = 0;
            StampsInt = 0;
            StampsFloat = 0;
            AverageScore = 0.0f;
            AverageStamps = 0.0f;
        }

        public static TrackedData operator +(TrackedData a, TrackedData b)
        {
            a.Score += b.Score;
            a.StampsFloat += b.StampsFloat;
            a.StampsInt += b.StampsInt;

            return a;
        }

        public static TrackedData operator / (TrackedData a, int divisor)
        {
            a.Score /= divisor;
            a.StampsFloat /= divisor;
            a.StampsInt /= divisor;
            a.AverageScore /= divisor;
            a.AverageStamps /= divisor;

            return a;
        }

        #region Should Serialize
        public bool ShouldSerializeAverageScore()
        {
            return IsAverage;
        }

        public bool ShouldSerializeAverageStamps()
        {
            return IsAverage;
        }

        public bool ShouldSerializeIsAverage()
        {
            return false;
        }

        public bool ShouldSerializeScore()
        {
            return !IsAverage;
        }

        public bool ShouldSerializeStampsInt()
        {
            return !IsAverage;
        }

        public bool ShouldSerializeStampsFloat()
        {
            return !IsAverage;
        }
#endregion
    }
}
