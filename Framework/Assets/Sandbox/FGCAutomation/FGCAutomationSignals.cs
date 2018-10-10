using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.FGCAutomation.Data;
using Sandbox.GraphQL;

namespace Sandbox.FGCAutomation
{
    public struct TrackCurrency
    {
        public int MatchNumber;
        public TrackedData Value;
        public CurrencyEnum Currency;
    }

    public struct FGCTrackingMatchStart
    {

    }
}
