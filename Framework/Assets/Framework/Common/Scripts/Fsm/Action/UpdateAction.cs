using System;

namespace Common.Fsm
{
    /**
	 * A pre created action class that uses delegates for Exit state
	 */
    public class UpdateAction : FsmActionAdapter
    {

        public delegate void FsmActionRoutine(FsmState owner);

        private FsmActionRoutine onRoutine;

        /**
		 * Constructor with OnExit routine.
		 */
        public UpdateAction(FsmState owner, FsmActionRoutine routine) : base(owner)
        {
            this.onRoutine = routine;
        }

        public override void OnUpdate()
        {
            if (onRoutine != null)
            {
                onRoutine(GetOwner());
            }
        }
    }
}

