using Common.Fsm;
using Framework;
using Framework.Common.Routines;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Sandbox.RGC
{
    public abstract class FGCUIRoutine : MonoRoutine
    {
        #region Constants

        protected const string StateIdle = "Idle";
        protected const string StateGuest = "Guest";
        protected const string StateFb = "Fb";
        protected const string StateSend = "Send";
        protected const string StateConvert = "Convert";
        protected const string StateClaim = "Claim";
        protected const string StateFetch = "Fetch";

        #endregion Constants

        #region Private Fields

        [ShowInInspector]
        [ReadOnly]
        protected Fsm Fsm;

        #endregion Private Fields

        #region Protected Fields

        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

        #endregion Protected Fields

        #region Protected Methods

        protected override void OnStarting()
        {
            base.OnStarting();

            RegisterObservables();
            PrepareFsm();
        }

        protected override void OnStopping()
        {
            base.OnStopping();

            DisposeObservables();

            Fsm = null;
        }
        
        protected virtual void OnEnterIdleState(FsmState owner)
        {
        }

        protected virtual void OnEnterGuestState(FsmState owner)
        {
        }

        protected virtual void OnEnterFbState(FsmState owner)
        {
        }

        protected virtual void OnEnterSendState(FsmState owner)
        {
        }

        protected virtual void OnEnterConvertState(FsmState owner)
        {
        }

        protected virtual void OnEnterClaimState(FsmState owner)
        {
        }

        protected virtual void OnEnterFetchState(FsmState owner)
        {
        }
        
        #endregion Protected Methods

        #region Private Methods

        private void DisposeObservables()
        {
            Disposables.Clear();
        }

        private void PrepareFsm()
        {
            // Initialize Fsm
            Fsm = new Fsm(nameof(FGCUIRoutine));
        
            // Add States
            var idle = Fsm.AddState(StateIdle);
            var guest = Fsm.AddState(StateGuest);
            var fb = Fsm.AddState(StateFb);
            var send = Fsm.AddState(StateSend);
            var convert = Fsm.AddState(StateConvert);
            var claim = Fsm.AddState(StateClaim);
            var fetch = Fsm.AddState(StateFetch);

            // Add Transitions
            idle.AddTransition(StateGuest, guest);
            idle.AddTransition(StateFb, fb);

            //guest.AddTransition(StateFetchData, fetch);
            guest.AddTransition(StateFb, fb);
            guest.AddTransition(StateSend, send);
            guest.AddTransition(StateConvert, convert);

            fb.AddTransition(StateSend, send);
            fb.AddTransition(StateConvert, convert);

            send.AddTransition(StateSend, send);
            send.AddTransition(StateConvert, convert);
            send.AddTransition(StateClaim, claim);

            convert.AddTransition(StateSend, send);
            convert.AddTransition(StateClaim, claim);

            claim.AddTransition(StateSend, send);

            // Add Actions
            idle.AddAction(new FsmDelegateAction(idle, OnEnterIdleState));
            guest.AddAction(new FsmDelegateAction(guest, OnEnterGuestState));
            fb.AddAction(new FsmDelegateAction(fb, OnEnterFbState));
            send.AddAction(new FsmDelegateAction(send, OnEnterSendState));
            convert.AddAction(new FsmDelegateAction(convert, OnEnterConvertState));
            claim.AddAction(new FsmDelegateAction(claim, OnEnterClaimState));
            fetch.AddAction(new FsmDelegateAction(fetch, OnEnterFetchState));

            // Start Fsm
            Fsm.Start(StateIdle);
        }

        private void RegisterObservables()
        {
            this.Receive<OnAutoConnectToFGC>()
                .Where(x => x.Type == AccountType.Guest)
                .Subscribe(_ => TransitionToState(StateGuest))
                .AddTo(Disposables);

            this.Receive<OnAutoConnectToFGC>()
                .Where(x => x.Type == AccountType.Facebook)
                .Subscribe(_ => TransitionToState(StateFb))
                .AddTo(Disposables);
            
        }

        protected void TransitionToState(string stateName)
        {
            Debug.LogErrorFormat(D.FGC + "FGCUIRoutine::TransitionToState State:{0} Transition:{1}\n", Fsm?.GetCurrentStateName(), stateName);

            Fsm?.SendEvent(stateName);
        }

        #endregion Private Methods
    }
}