using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Sandbox.RGC
{
    public class FGCBindingsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAccountProvider>().To<FGCAccount>().AsSingle().NonLazy();
        }
    }
}
