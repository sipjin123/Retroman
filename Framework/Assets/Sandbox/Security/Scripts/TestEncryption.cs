using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

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
        [SerializeField, ShowInInspector]
        private TestData TestData;

        private string Password;
        private LocalData LocalData;

        private void Start()
        {
            Password = "Security";
            LocalData = new LocalData(Password, "TestData");
        }

        [Button(25)]
        public void Save()
        {
            LocalData.ReplaceToDisk(TestData);
        }

        [Button(25)]
        public void Load()
        {
            TestData = LocalData.LoadFromDisk<TestData>();
        }

        [Button(25)]
        public void SaveEncryted()
        {
            LocalData.ReplaceToDisk(TestData, true);
        }

        [Button(25)]
        public void LoadEncrypted()
        {
            TestData = LocalData.LoadFromDisk<TestData>(true);
        }
    }
}