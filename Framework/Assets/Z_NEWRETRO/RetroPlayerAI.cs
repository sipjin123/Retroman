using Common.Utils;
using Retroman;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
public class RetroPlayerAI : MonoInstaller
{
    IAIController _IAIController;
    const string FALL_STOPPER = "FallStopper";
    public GameObject _DetectorObject;
    float RayLength = 15;

    [Inject]
    public RetroPlayerAI(IAIController playerCont)
    {
        _IAIController = playerCont;
    }

    PlayerControls _PlayerControls;
    Transform _PlayerTransform;
    MessageBroker _Broker;

    bool ifActive;
    Vector3 DefaultSetup = new Vector3(0,1,1.75f);
    public void InjectPlayerControls(PlayerControls playerCont)
    {
        _Broker = Factory.Get<DataManagerService>().MessageBroker;
        _PlayerControls = playerCont;
        _PlayerTransform = playerCont.transform;
        ifActive = true;
    }

    private void FixedUpdate()
    {
        if (ifActive == false)
            return;
        transform.position = _PlayerTransform.position;
        transform.rotation = _PlayerTransform.rotation;

        RaycastHit ray;
        Debug.DrawRay(_DetectorObject.transform.position, -_DetectorObject.transform.up * RayLength, Color.yellow);
        if (Physics.Raycast(_DetectorObject.transform.position,-_DetectorObject.transform.transform.up * RayLength ,out ray))
        {
            PlatformMinion minionHit = ray.collider.gameObject.GetComponent<PlatformMinion>();
            if (minionHit == null || ray.collider.gameObject.name == FALL_STOPPER)
            {
                _Broker.Publish(new CommandAIJump());
            }
            else
            {
                if (minionHit)
                {
                    switch (minionHit.PlatformRunnability)
                    {
                        case PlatformRunnability.MustJump:
                            {
                                _Broker.Publish(new CommandAIJump());
                            }
                            break;
                        case PlatformRunnability.CanRun:
                            {
                                if (minionHit.transform.position.y > minionHit.PrePlatform.transform.position.y)
                                {
                                    _Broker.Publish(new CommandAIJump());
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
