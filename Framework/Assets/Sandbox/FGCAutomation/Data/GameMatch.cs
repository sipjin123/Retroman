using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.Utilities;

namespace Sandbox.FGCAutomation.Data
{
    public class GameMatch
    {
        public GameMatch()
        {
            MatchNumber = -1;
            CurrencyValueMap = new Dictionary<string, TrackedData>();
        }

        public GameMatch(GameMatch other): this()
        {
            other.CurrencyValueMap.ForEach(_ =>
            {
                CurrencyValueMap.Add(_.Key, _.Value);
            });
            MatchNumber = other.MatchNumber;
        }

        public int MatchNumber;
        public Dictionary<string, TrackedData> CurrencyValueMap;
    }
}