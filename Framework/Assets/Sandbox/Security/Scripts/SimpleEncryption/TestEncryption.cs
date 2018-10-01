using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;

namespace Sandbox.Security
{
    [Serializable]
    public class TestData : IJson
    {
        public string DataA;
        public int DataB;
        public List<float> DataC;
        public Dictionary<string, int> DataD;
    }

    public class TestEncryption : MonoBehaviour
    {
        [SerializeField]
        private TestData TestData;

        private string Password;
        private LocalData LocalData;

        private void Start()
        {
            Password = "Security";
            LocalData = new LocalData(Password, "TestData");
        }
        
        public void Save()
        {
            LocalData.ReplaceToDisk(TestData);
        }
        
        public void Load()
        {
            TestData = LocalData.LoadFromDisk<TestData>();
        }
        
        public void SaveEncryted()
        {
            LocalData.ReplaceToDisk(TestData, true);
        }
        
        public void LoadEncrypted()
        {
            TestData = LocalData.LoadFromDisk<TestData>(true);
        }
    }
}