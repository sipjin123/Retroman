using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.FGCAutomation.Interfaces
{
    public interface IDataWriter
    {
        string WritePath { get; }
        string WriteFileName { get; }
        void InitializeWritePath(string path);
        void InitializeWriteFilename(string filename);
        void WriteToDisk(object dataToSerialize);
    }
}
