using Framework;
using Framework.Common.Routines;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using Zenject;

namespace Sandbox.RGC
{
    public class FGCAutomator : SceneObject
    {
        #region Fields

        [Inject(Id = "FGCAutomationMasterRoutine")]
        [ShowInInspector]
        [ReadOnly]
        private MonoRoutine Routine;

        #endregion Fields

        #region Unity Life Cycle

        protected override void Start()
        {
            base.Start();

            Assert.IsNotNull(Routine);

            Run();
        }

        #endregion Unity Life Cycle

        #region Methods

        public void Run()
        {
            Routine?.Run();
        }

        public void Stop()
        {
            var stopRoutine = Routine?.Stop();

            stopRoutine?.Done(OnEnded);
        }

        #endregion Methods

        #region Protected Methods

        protected void OnEnded()
        {
        }

        #endregion Protected Methods

        #region Private Methods

        [Button("Run", ButtonSizes.Large)]
        private void InspectorRun() => Run();

        [Button("Stop", ButtonSizes.Large)]
        private void InspectorStop() => Routine?.Stop();

        #endregion Private Methods
    }
}