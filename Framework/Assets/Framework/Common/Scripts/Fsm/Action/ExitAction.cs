using System;

namespace Common.Fsm
{
    /**
	 * A pre created action class that uses delegates for Exit state
	 */
    public class ExitAction : FsmActionAdapter
    {

        public delegate void FsmActionRoutine(FsmState owner);
        
        private FsmActionRoutine onExitRoutine;

        /**
		 * Constructor with OnExit routine.
		 */
        public ExitAction(FsmState owner, FsmActionRoutine onExitRoutine) : base(owner)
        {
            this.onExitRoutine = onExitRoutine;
        }
        
        public override void OnExit()
        {
            if (onExitRoutine != null)
            {
                onExitRoutine(GetOwner());
            }
        }
    }
}

