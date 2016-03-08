using System;

using Common.Fsm;
using Common.Time;

namespace Common.Fsm.Action {
	/**
	 * This action was introduced such that the polled time does not reset when the state is reentered.
	 */
	public class NonResettingTimedWait : FsmActionAdapter {
		
		private float waitTime;
		private readonly CountdownTimer timer;
		private readonly string timeReference;
		private readonly string finishEvent;
		
		/**
		 * Constructor
		 */
		public NonResettingTimedWait(FsmState owner, string timeReferenceName, string finishEvent) : base(owner) {
			if(string.IsNullOrEmpty(timeReferenceName)) {
				this.timer = new CountdownTimer(1); // uses default time reference
			} else {
				this.timer = new CountdownTimer(1, timeReferenceName); // dummy only, we reset on Init()
			}
			
			this.finishEvent = finishEvent;
		}
		
		/**
		 * Initializes the action. We provide this action so that we can manage instances of this class in an object pool.
		 * This function should be invoked to reset the timer
		 */
		public void Init(float waitTime) {
			this.waitTime = waitTime;
			this.timer.Reset(this.waitTime);
		}
		
		public override void OnEnter() {
			if(waitTime <= 0) {
				Finish();
			}
		}
		
		public override void OnUpdate() {
			timer.Update();
			
			if(timer.HasElapsed()) {
				Finish();
			}
		}
		
		private void Finish() {
			GetOwner().SendEvent(finishEvent);
		}
		
		/**
		 * Returns the time duration ratio.
		 */
		public float GetRatio() {
			return timer.GetRatio();
		}
		
	}
}

