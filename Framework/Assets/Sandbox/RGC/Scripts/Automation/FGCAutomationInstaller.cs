using Framework.Common.Routines;
using UnityEngine;
using Zenject;

namespace Sandbox.RGC
{
    [CreateAssetMenu(fileName = "FGCAutomationInstaller", menuName = "Installers/FGCAutomationInstaller")]
    public class FGCAutomationInstaller : ScriptableObjectInstaller<FGCAutomationInstaller>
    {
        [SerializeField]
        private FGCUIRoutine UIRoutine;

        [SerializeField]
        private MonoRoutine MasterRoutine;

        public override void InstallBindings()
        {
            if (UIRoutine)
            { 
                Container.Bind<FGCUITestAppRoutine>()
                    .WithId("FGCUITestAppRoutine")
                    .FromComponentInNewPrefab(UIRoutine)
                    .WithGameObjectName("FGCUITestAppRoutine")
                    .AsCached();
            }

            if (MasterRoutine)
            { 
                Container.Bind<MonoRoutine>()
                    .WithId("FGCAutomationMasterRoutine")
                    .FromComponentInNewPrefab(MasterRoutine)
                    .WithGameObjectName("FGCAutomationMasterRoutine")
                    .AsCached();
            }
        }
    }
}