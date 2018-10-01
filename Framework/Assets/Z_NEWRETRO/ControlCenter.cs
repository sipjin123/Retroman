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

public class ControlCenter : SerializedMonoBehaviour
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

    float timerCount = 0;
    const float timerCap = 2;
    bool startTimer;
    private void Update()
    {
        if(startTimer)
        {
            timerCount += 1f/60f;
            if (timerCount > timerCap)
            {
                timerCount = 0;
                startTimer = false;
                ProcessSignal();
            }
        }
    }
    private void SetSignal()
    {
        _Broker.Receive<PlayerControlSpawned>().Subscribe(_ => { initialized = true; }).AddTo(this);

        _Broker.Receive<AUTOMATED_UI_STATE>().Subscribe(_ =>
        {

            startTimer = true;
            cachedEscene = (_.Scene);
        
        }).AddTo(this);
    }
    EScene cachedEscene;

    private void ProcessSignal()
    {
        switch (cachedEscene)
        {
            case EScene.TitleRoot:
                {
                    PopupCollectionRoot popRoot = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);
                    popRoot.Hide();
                    _Broker.Publish(new AUTOMATE_TRIGGER { AutomateType = AutomateType.GoToGame });
                }
                break;

            case EScene.ResultRoot:
                {
                    Debug.LogError("Publishing Reset Game");
                    _Broker.Publish(new AUTOMATE_TRIGGER { AutomateType = AutomateType.ResetGame });
                }
                break;
        }
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
