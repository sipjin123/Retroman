using Common.Utils;
using Framework;
using Retroman;
using System;
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

    [SerializeField]
    bool _ShouldRecordStats;
    const string CachedControlType = "CachedControlType";
    const string CachedUIType = "CachedUIType";

    [SerializeField]
    AutomationUI _AutomationUI;

    public void SetControl(ControlType controlType)
    {
        _ControlType = controlType;
        PlayerPrefs.SetString(CachedControlType, _ControlType.ToString());
        Debug.LogError(D.AUTOMATION + "Updated Pref Control :: " + PlayerPrefs.GetString(CachedControlType));
    }
    public void SetUI(UIControlType automationUIType)
    {
        _UIControlType = automationUIType;
        PlayerPrefs.SetString(CachedUIType, _UIControlType.ToString());
        Debug.LogError(D.AUTOMATION + "Updated Pref UIFlow :: " + PlayerPrefs.GetString(CachedUIType));
    }
    public override void InstallBindings()
    {
        //WORKING
        //var foo = Container.InstantiatePrefabForComponent<ControlCenter>(PRefab);
        //WORKING
        //var foo = Container.InstantiateComponent<ControlCenter>(PRefab);

        _ControlType = (ControlType)Enum.Parse(typeof(ControlType), PlayerPrefs.GetString(CachedControlType, _ControlType.ToString()));
        _UIControlType = (UIControlType)Enum.Parse(typeof(UIControlType), PlayerPrefs.GetString(CachedUIType, _UIControlType.ToString()));

        _AutomationUI.InitUI(_ControlType.ToString(), _UIControlType.ToString());
        
        switch (_ControlType)
        {
            case ControlType.AIController:
                {
                    Container.Bind<IController>().
                        To<AIInputController>().
                        FromInstance(new AIInputController(_AutomatedController)).
                        AsTransient();
                }
                break;
            case ControlType.PlayerController:
                {
                    Container.Bind<IController>().
                        To<CharacterInputController>().
                        FromInstance(new CharacterInputController(_AutomatedController)).
                        AsTransient();
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
                        AsTransient();
                }
                break;
            default:
                {
                    Container.Bind<IUIController>().
                        To<DoNothingUI>().
                        FromInstance(new DoNothingUI()).
                        AsTransient();
                }
                break;
        }
        if(_ShouldRecordStats)
        {

            Container.Bind<IDataController >().
                To<BasicDataGather>().
                FromInstance(new BasicDataGather()).
                AsSingle();
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