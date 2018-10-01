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
    [SerializeField]
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


    //TEST
    public GameObject _DetectorObject;
    float RayLength = 100;
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
