using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using UniRx;
using Framework;
using Retroman;
using uPromise;
using Sandbox.Popup;

public class AutomatedController : SerializedMonoBehaviour
{
    IController _IController;
    bool initialized;
    MessageBroker _Broker;

    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
        _IController.InjectBroker(broker);
        SetSignal();
    }

    private void SetSignal()
    {
        _Broker.Receive<PlayerControlSpawned>().Subscribe(_ => { initialized = true; }).AddTo(this);
    }

    [Inject]
    public void Construct (IController icont)
    {
        Debug.LogError(D.AUTOMATION + "Construction Complete");
        _IController = icont;
    }

    private void FixedUpdate()
    {
        if (initialized)
        {
            if(_IController.CheckIfCanJump())
            {
                _IController.EmitJumpSignal();

            }
            else
            {
            }
        }
    }
}
