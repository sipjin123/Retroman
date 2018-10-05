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
    PopupCollectionRoot _PopupRoot;

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
        _PopupRoot = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);

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
                    _PopupRoot.Hide();
                    _Broker.Publish(new AUTOMATE_TRIGGER { AutomateType = AutomateType.GoToGame });
                }
                break;

            case EScene.ResultRoot:
                {
                    _Broker.Publish(new AUTOMATE_TRIGGER { AutomateType = AutomateType.ResetGame });
                }
                break;
        }
    }
}