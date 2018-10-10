using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.Utilities;

namespace Sandbox.FGCAutomation.Data
{
    public class SessionData
    {
        public SessionData()
        {
            CurrencyAverageMap = new Dictionary<string, TrackedData>();
            GameMatches = new List<GameMatch>();
        }

        public SessionData(SessionData other): this()
        {
            other.CurrencyAverageMap.ForEach(_ =>
            {
                CurrencyAverageMap.Add(_.Key, _.Value);
            });

            other.GameMatches.ForEach(_ =>
            {
                GameMatches.Add(new GameMatch(_));
            });

            SessionStart = other.SessionStart;
            SessionEnd = other.SessionEnd;
        }

        public DateTime SessionStart;
        public DateTime SessionEnd;
        public Dictionary<string, TrackedData> CurrencyAverageMap;
        public List<GameMatch> GameMatches;
        
    }
}
