using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Utils;

namespace Framework
{
    [Serializable]
    public class LocalData
    {
        private static readonly char SEP = Path.DirectorySeparatorChar;

        [SerializeField]
        private string _UserId;
        public string UserId
        {
            get { return _UserId; }
            private set { _UserId = value; }
        }

        [SerializeField]
        private string _LocalPath;
        public string LocalPath
        {
            get { return _LocalPath; }
            private set { _LocalPath = value; }
        }

        [SerializeField]
        private string _LocalFolder;
        public string LocalFolder
        {
            get { return _LocalFolder; }
            private set { _LocalFolder = value; }
        }

        private readonly SimpleEncryption Encryption;

        // TODO: +AS:20180627 Support file extension
        public LocalData(string userId, params object[] folders)
        {
            Assertion.AssertNotEmpty(userId, D.ERROR + "UserId should not be null or empty!\n");
            Assertion.Assert(folders.Length > 0, D.ERROR + "folders should never be zero!\n");

            UserId = userId;
            Encryption = new SimpleEncryption(UserId.ToUTF8Bytes(), UserId);
            LocalPath = string.Format("{0}{1}{2}", Application.persistentDataPath, SEP, UserId);
            CreateDirectory(LocalPath);

            int len = folders.Length;
            if (len >= 2)
            {
                for (int i = 0; i < len - 1; i++)
                {
                    LocalPath = string.Format("{0}{1}{2}", LocalPath, SEP, folders[i]);
                    CreateDirectory(LocalPath);
                }
            }

            LocalFolder = string.Format("{0}{1}", LocalPath, SEP);
            LocalPath = string.Format("{0}{1}{2}", LocalPath, SEP, folders.LastOrDefault());
        }

        public void CreateDirectory(string folder)
        {
            Directory.CreateDirectory(folder);
        }

        public string GetPath()
        {
            return LocalPath;
        }

        public bool Exists()
        {
            return File.Exists(LocalPath);
        }

        public void Delete()
        {
            if (Exists()) 
            {
                File.Delete(LocalPath);
            }
        }

#if ENCRYPT_LOCAL_DATA
        public void AppendToDisk<T>(T data, bool encrypt = true) where T : IJson
#else
        public void AppendToDisk<T>(T data, bool encrypt = false) where T : IJson
#endif
        {
            using (StreamWriter writer = new StreamWriter(LocalPath, true))
            {
                if (!encrypt)
                {
                    writer.WriteLine(data.ToJson());
                }
                else
                {
                    writer.WriteLine(Encryption.Encrypt(data.ToJson()));
                }
            }
        }

#if ENCRYPT_LOCAL_DATA
        public void AppendToDisk<T>(List<T> data, bool encrypt = true) where T : IJson
#else
        public void AppendToDisk<T>(List<T> data, bool encrypt = false) where T : IJson
#endif
        {
            using (StreamWriter writer = new StreamWriter(LocalPath, true))
            {
                if (!encrypt)
                {
                    data.ForEach(d => writer.WriteLine(d.ToJson()));
                }
                else
                {
                    data.ForEach(d => writer.WriteLine(Encryption.Encrypt(d.ToJson())));
                }
            }
        }

#if ENCRYPT_LOCAL_DATA
        public void ReplaceToDisk<T>(T data, bool encrypt = true) where T : IJson
#else
        public void ReplaceToDisk<T>(T data, bool encrypt = false) where T : IJson
#endif
        {
            using (StreamWriter writer = new StreamWriter(LocalPath, false))
            {
                if (!encrypt)
                {
                    writer.WriteLine(data.ToJson());
                }
                else
                {
                    writer.WriteLine(Encryption.Encrypt(data.ToJson()));
                }
            }
        }

#if ENCRYPT_LOCAL_DATA
        public void ReplaceToDisk<T>(List<T> data, bool encrypt = true) where T : IJson
#else
        public void ReplaceToDisk<T>(List<T> data, bool encrypt = false) where T : IJson
#endif
        {
            using (StreamWriter writer = new StreamWriter(LocalPath, false))
            {
                if (!encrypt)
                {
                    data.ForEach(d => writer.WriteLine(d.ToJson()));
                }
                else
                {
                    data.ForEach(d => writer.WriteLine(Encryption.Encrypt(d.ToJson())));
                }
            }
        }

#if ENCRYPT_LOCAL_DATA
        public T LoadFromDisk<T>(bool isEncrypted = true) where T : IJson
#else
        public T LoadFromDisk<T>(bool isEncrypted = false) where T : IJson
#endif
        {
            T t = default(T);
            using (StreamReader sr = new StreamReader(LocalPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!isEncrypted)
                    {
                        t = line.ToJson<T>();
                    }
                    else
                    {
                        t = Encryption.Decrypt(line).ToJson<T>();
                    }

                    break;
                }
            }

            return t;
        }

#if ENCRYPT_LOCAL_DATA
        public List<T> LoadSetFromDisk<T>(bool isEncrypted = true) where T : IJson
#else
        public List<T> LoadSetFromDisk<T>(bool isEncrypted = false) where T : IJson
#endif
        {
            List<T> set = new List<T>();
            T t = default(T);
            using (StreamReader sr = new StreamReader(LocalPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!isEncrypted)
                    {
                        t = line.ToJson<T>();
                    }
                    else
                    {
                        t = Encryption.Decrypt(line).ToJson<T>();
                    }

                    set.Add(t);
                }
            }

            return set;
        }
    }
}