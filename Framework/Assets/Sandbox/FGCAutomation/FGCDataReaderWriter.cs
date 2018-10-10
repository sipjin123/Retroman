using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
using Newtonsoft.Json;
using Sandbox.FGCAutomation.Interfaces;
using UnityEngine;

namespace Sandbox.FGCAutomation
{
    public class FGCDataReaderWriter: IDataWriter, IDataReader
    {
        public string WritePath { get; private set; }
        public string WriteFileName { get; private set; }

        public string ReadPath { get; private set; }
        public string ReadFileName { get; private set; }

        public void InitializeWritePath(string path)
        {
            WritePath = path;
        }
        public void InitializeWriteFilename(string filename)
        {
            WriteFileName = filename;
        }

        public void InitializeReadPath(string path)
        {
            ReadPath = path;
        }
        public void InitializeReadFilename(string filename)
        {
            ReadFileName = filename;
        }

        public T GetData<T>()
        {
            throw new NotImplementedException();
        }

        public void WriteToDisk(object dataToSerialize)
        {
            Debug.Log($"{D.LOG} Will try to write at {WritePath}{WriteFileName}");

            if (!Directory.Exists(WritePath))
            {
                Directory.CreateDirectory(WritePath);
            }

            if (!File.Exists($"{WritePath}{WriteFileName}"))
            {
                File.Create($"{WritePath}{WriteFileName}").Close();
            }
            using (StreamWriter writer = new StreamWriter($"{WritePath}{WriteFileName}"))
            {
                try
                {
                    writer.Write(JsonConvert.SerializeObject(dataToSerialize, Formatting.Indented));
                }
                catch (Exception e)
                {
                    Debug.LogError($"{D.ERROR} {e.Message}");
                }
            }
        }
    }
}
