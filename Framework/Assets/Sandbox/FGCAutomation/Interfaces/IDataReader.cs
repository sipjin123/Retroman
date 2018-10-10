using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.FGCAutomation.Interfaces
{
    public interface IDataReader
    {
        string ReadPath { get; }
        string ReadFileName { get; }
        void InitializeReadPath(string path);
        void InitializeReadFilename(string filename);
        T GetData<T>();
    }
}
