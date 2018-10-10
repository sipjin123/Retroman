using System.Collections;
using System.Collections.Generic;
using Framework;
using Sandbox.FGCAutomation.Interfaces;
using UnityEngine;
using Zenject;

namespace Sandbox.FGCAutomation
{
    public class FGCAutomationZenjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //base.InstallBindings();
            Debug.LogError($"{D.ERROR} Installing bindings!");
            Container.Bind<IProcessor>().To<CurrencyProcessor>().AsSingle();
            Container.Bind<IDataWriter>().To<FGCDataReaderWriter>().AsSingle();
        }
    }
}

