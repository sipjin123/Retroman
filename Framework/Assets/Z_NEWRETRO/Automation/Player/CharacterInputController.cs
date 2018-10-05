using Common.Utils;
using Framework;
using Retroman;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class CharacterInputController :  IController
{
    MessageBroker _Broker;
    [SerializeField]
    private Transform _OwnerTransform;
    bool ifActive;

    public CharacterInputController(  Transform ownerTransform)
    {
        _OwnerTransform = ownerTransform;
        Message();
    }

    public bool CheckIfCanJump()
    {
        if (ifActive)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0))
            {
                Debug.LogError("SPACE WAS PRESSED");
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public void EmitJumpSignal()
    {
        _Broker.Publish(new CharJumpSignal());
    }

    public void InitSignals()
    {
        Debug.LogError(D.AUTOMATION + "Player Init Signals");
        _Broker.Receive<PlayerControlSpawned>().Subscribe(_ =>
        {
            ifActive = true;

        }).AddTo(_OwnerTransform);
    }

    public void Message()
    {
        Debug.LogError(D.AUTOMATION + "This is a Player");
    }

    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
        InitSignals();
    }
}