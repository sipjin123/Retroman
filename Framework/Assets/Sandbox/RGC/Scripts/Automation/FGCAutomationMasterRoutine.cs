using Framework.Common.Routines;
using Sirenix.OdinInspector;
using Zenject;

namespace Sandbox.RGC
{
    public class FGCAutomationMasterRoutine : MonoRoutine
    {
        #region Enums

        public enum SubRoutineType
        {
            Null,

            RegularFlow,
        }

        #endregion Enums

        #region Fields

        [ShowInInspector]
        private SubRoutineType CurrentSubRoutineType = SubRoutineType.RegularFlow;

        private IRoutine CurrentRoutine;

        #endregion Fields

        #region Injected Fields
        
        [Inject(Id = "FGCUITestAppRoutine")]
        private FGCUITestAppRoutine UIRoutine;

        #endregion Injected Fields

        #region Protected Methods

        protected override void DoStart()
        {
            base.DoStart();

            CurrentRoutine = GetSubRoutineOfType(CurrentSubRoutineType);

            StartCurrentSubRoutine();
        }

        protected override void DoStop()
        {
            if (CurrentRoutine == null)
                base.DoStop();
            else
            {
                // Wait for the current sub-routine to stop first before resolving the Start and Stop promises.
                CurrentRoutine.Stop()
                    .Done(() => base.DoStop());
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private SubRoutineType GetNextSubRoutineType(SubRoutineType type)
        {
            switch (type)
            {
                default:
                case SubRoutineType.RegularFlow:
                    // Use as is temporarilly
                    return SubRoutineType.RegularFlow;
            }
        }

        private IRoutine GetSubRoutineOfType(SubRoutineType type)
        {
            switch (type)
            {
                default:
                case SubRoutineType.RegularFlow:
                    return UIRoutine;
            }
        }

        private void StartCurrentSubRoutine()
        {
            if (!IsRunning || CurrentRoutine == null)
            {
                CurrentSubRoutineType = SubRoutineType.Null;
                Stop();
            }
            else
            {
                CurrentRoutine.Run().Then(
                    () =>
                    {
                        // After the current sub-routine has finished decide on the next sub-routine.
                        CurrentSubRoutineType = GetNextSubRoutineType(CurrentSubRoutineType);

                        // Set the next sub-routine.
                        CurrentRoutine = GetSubRoutineOfType(CurrentSubRoutineType);

                        // Start the next sub-routine.
                        StartCurrentSubRoutine();
                    });
            }
        }

        #endregion Private Methods
    }
}