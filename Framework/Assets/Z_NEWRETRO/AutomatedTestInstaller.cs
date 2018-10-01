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
    [SerializeField]
    private Transform _AutomatedController;
    MessageBroker _Broker;

    [SerializeField]
    ControlType _ControlType;
    [SerializeField]
    UIControlType _UIControlType;
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
                        FromInstance(new AIInputController(_AutomatedController)).
                        AsSingle();
                }
                break;
            case ControlType.PlayerController:
                {
                    Container.Bind<IController>().
                        To<CharacterInputController>().
                        FromInstance(new CharacterInputController(_AutomatedController)).
                        AsSingle();
                }
                break;
        }

        switch(_UIControlType)
        {
            case UIControlType.AutoRepeatGame:
                {
                    Container.Bind<IUIController>().
                        To<AutoRepeatUIFlow>().
                        FromInstance(new AutoRepeatUIFlow()).
                        AsSingle();
                }
                break;
            default:
                {
                    Container.Bind<IUIController>().
                        To<DoNothingUI>().
                        FromInstance(new DoNothingUI()).
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
public enum UIControlType
{
    None,
    AutoRepeatGame
}
public interface IController
{
    void Message();
    void EmitJumpSignal();
    void InitSignals();
    bool CheckIfCanJump();
    void InjectBroker(MessageBroker broker);
}
public interface IUIController
{
    void InjectBroker(MessageBroker broker);
    void SetSignal(GameObject obj);
    void UpdateData();
}