using Common.Utils;
using Retroman;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class AutomatedTestService : MonoInstaller
{
    [SerializeField]
    private GameObject _PlayerAIObj;
    MessageBroker _Broker;

    public override void InstallBindings()
    {

        Container.Bind<ALALA>().AsSingle();
        Container.Bind<IControllere>().To<Controllere>().AsSingle();
    }

    private void Awake()
    {
        Factory.Register<AutomatedTestService>(this);
    }
    public void InjectBroker(MessageBroker broker)
    {
        _Broker = broker;


        _Broker.Receive<PlayerControlSpawned>().Subscribe(_ =>
        {
            //_PlayerAIObj.GetComponent<RetroPlayerAI>().InjectPlayerControls(_.PlayerControls);
        }).AddTo(this);
    }
}

public class Controllere : IControllere
{
    IControllere icont;
    public Controllere (IControllere ico)
    {
        icont = ico;
    }

}
public interface IControllere
{

}

public class ALALA
{

}
