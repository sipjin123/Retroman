using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.FGCAutomation.Data
{
    public struct TaskData
    {
        public object Data;

        public T GetData<T>()
        {
            return (T) Data;
        }
    }
}
