using System;

namespace Common.Fsm
{
    public class TriggerAction : FsmActionAdapter
    {
        private string Evt;
        
        public TriggerAction(FsmState owner, string evt) : base(owner)
        {
            Evt = evt;
        }
        
        public override void OnEnter()
        {
            GetOwner().SendEvent(Evt);
        }
    }
}

