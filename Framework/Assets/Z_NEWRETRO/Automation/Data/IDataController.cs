using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IDataController
{
    void InitializeData(GameObject obj,MessageBroker broker);
    void WriteToFile();
}