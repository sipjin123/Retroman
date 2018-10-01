using Common.Utils;
using Framework;
using Retroman;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class AutomatedTestInstaller: MonoInstaller<AutomatedTestInstaller>
{
    public GameObject PRefab;
    MessageBroker _Broker;
    public Transform _ObjectFollower;

    [SerializeField]
    ControlType _ControlType;
    public override void InstallBindings()
    {
        //WORKING
        //var foo = Container.InstantiatePrefabForComponent<ControlCenter>(PRefab);
        //WORKING
        //var foo = Container.InstantiateComponent<ControlCenter>(PRefab);

        switch(_ControlType)
        {
            case ControlType.AIController:
                {
                    Container.Bind<IController>().
                        To<AIInputController>().
                        FromInstance(new AIInputController(_ObjectFollower)).
                        AsSingle();
                }
                break;
            case ControlType.PlayerController:
                {
                    Container.Bind<IController>().
                        To<CharacterInputController>().
                        FromInstance(new CharacterInputController(_ObjectFollower)).
                        AsSingle();
                }
                break;
        }
    }
}
public enum ControlType
{
    PlayerController,
    AIController
}

public interface IController
{
    void Message();
    void EmitJumpSignal();
    void InitSignals();
    bool CheckIfCanJump();
    void InjectBroker(MessageBroker broker);
}
