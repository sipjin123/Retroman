using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.FGCAutomation.Data;

namespace Sandbox.FGCAutomation.Interfaces
{
    public interface IProcessor
    {
        SessionData SessionData { get; }

        void Reset();
        void Process(TaskData data);
        SessionData AggregateAndFinalize();
    }
}
