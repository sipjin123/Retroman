using Common.Utils;
using Framework;
using Retroman;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class AIInputController :  IController
{
    const string FALL_STOPPER = "HOLE_OBJ";
    const string START_CUBE = "StartingCube"; 
    public GameObject _DetectorObject;
    float RayLength = 100;

    MessageBroker _Broker;
    bool initiated;

    PlayerControls _PlayerControls;
    Transform _PlayerTransform;

    Vector3 DefaultSetup = new Vector3(0, 1, 1.75f);
    public Transform _OwnerTransform;

    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
        InitSignals();
    }
    public void InitSignals()
    {
        Debug.LogError(D.AUTOMATION + "AI Init Signals");
        _Broker.Receive<PlayerControlSpawned>().Subscribe(_ =>
        {
            _PlayerControls = _.PlayerControls;
            _PlayerTransform = _.PlayerControls.transform;
            _DetectorObject = _OwnerTransform.transform.GetChild(0).gameObject;
            initiated = true;

        }).AddTo(_OwnerTransform);
    }

    public AIInputController(Transform ownerTransform)
    {
        _OwnerTransform = ownerTransform;
        Message();
    }
    public bool CheckIfCanJump()
    {
        if (initiated == true)
        {
            _OwnerTransform.position = _PlayerTransform.position;
            _OwnerTransform.rotation = _PlayerTransform.rotation;

            RaycastHit ray;
            Debug.DrawRay(_DetectorObject.transform.position, -_DetectorObject.transform.up * RayLength, Color.yellow);
            if (Physics.Raycast(_DetectorObject.transform.position, -_DetectorObject.transform.transform.up * RayLength, out ray))
            {
                PlatformMinion minionHit = ray.collider.gameObject.GetComponent<PlatformMinion>();
                if ((minionHit == null && ray.collider.gameObject.name != START_CUBE )|| ray.collider.gameObject.name == FALL_STOPPER)
                {
                    return true;
                }
                else
                {
                    if (minionHit)
                    {
                        switch (minionHit.PlatformRunnability)
                        {
                            case PlatformRunnability.MustJump:
                                {
                                    return true;
                                }
                                break;
                            case PlatformRunnability.CanRun:
                                {
                                    if (minionHit.transform.position.y > minionHit.PrePlatform.transform.position.y)
                                    {
                                        return true;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
            }
            return false;
        }
        return false;
    }
    
    public void EmitJumpSignal()
    {
        _Broker.Publish(new CharJumpSignal());
    }


    public void Message()
    {
        Debug.LogError(D.AUTOMATION+"This is an AI");
    }



}