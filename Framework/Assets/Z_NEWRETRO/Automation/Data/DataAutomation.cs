using Framework;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Retroman;
public class DataAutomation : MonoBehaviour
{
    IDataController _IDataController;
    
    bool isActive;

    MessageBroker _Broker;
    [Inject]
    void Constrcut(IDataController iDataCont)
    {
        Debug.LogError(D.AUTOMATION + "Data Gather Constructed");
        _IDataController = iDataCont;
        isActive = true;
    }

    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
        InitializeData();
    }
    void InitializeData()
    {
        Debug.LogError(D.AUTOMATION + "Data Controller Initialized");
        _IDataController.InitializeData(gameObject,_Broker);

        _Broker.Receive<AutomationCommands>().Subscribe(_ => 
        {
            switch(_.TypeOfAutomationCommands)
            {
                case TypeOfAutomationCommands.WriteToData:
                    {
                        _IDataController.WriteToFile();
                    }
                    break;

            }
        }).AddTo(this);
    }
    
}
