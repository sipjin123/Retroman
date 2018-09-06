using System;

namespace Common.Fsm
{
    /**
	 * A pre created action class that uses delegates for Exit state
	 */
    public class EnterAction : FsmActionAdapter
    {

        public delegate void FsmActionRoutine(FsmState owner);

        private FsmActionRoutine onRoutine;

        /**
		 * Constructor with OnExit routine.
		 */
        public EnterAction(FsmState owner, FsmActionRoutine routine) : base(owner)
        {
            this.onRoutine = routine;
        }

        public override void OnEnter()
        {
            if (onRoutine != null)
            {
                onRoutine(GetOwner());
            }
        }
    }
}

