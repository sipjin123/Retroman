using Framework;
using Retroman;
using Sandbox.Popup;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class AutomatedSceneFlow : SerializedMonoBehaviour
{

    MessageBroker _Broker;
    IUIController _IUIController;
    bool initialized = false;
    [Inject]
    public void Construct(IUIController uiController)
    {
        Debug.LogError(D.AUTOMATION + " UI Construction Complete");
        _IUIController = uiController;
    }
    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
        _IUIController.InjectBroker(broker);
        _IUIController.SetSignal(gameObject);
        initialized = true;
    }
    private void Update()
    {
        if(initialized)
        _IUIController.UpdateData();
    }


}
