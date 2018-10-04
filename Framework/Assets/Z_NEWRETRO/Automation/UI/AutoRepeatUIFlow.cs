using Framework;
using Retroman;
using Sandbox.Popup;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AutoRepeatUIFlow : IUIController
{
    float timerCount = 0;
    const float timerCap = 2;
    bool startTimer;
    EScene _CachedEscene;

    MessageBroker _Broker;
    PopupCollectionRoot popRoot;
    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;
    }

    public void SetSignal(GameObject owner)
    {

        _Broker.Receive<AutomatedUIState>().Subscribe(_ =>
        {

            startTimer = true;
            _CachedEscene = (_.Scene);

        }).AddTo(owner);
        popRoot = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);

    }

    public void UpdateData()
    {
        if (startTimer)
        {
            timerCount += 1f / 60f;
            if (timerCount > timerCap)
            {
                timerCount = 0;
                startTimer = false;
                ProcessSignal();
            }
        }
    }

    private void ProcessSignal()
    {
        switch (_CachedEscene)
        {
            case EScene.TitleRoot:
                {
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
}

public class DoNothingUI : IUIController
{
    public void InjectBroker(MessageBroker broker)
    {
        //throw new System.NotImplementedException();
    }

    public void SetSignal(GameObject obj)
    {
        //throw new System.NotImplementedException();
    }

    public void UpdateData()
    {
        //throw new System.NotImplementedException();
    }
}