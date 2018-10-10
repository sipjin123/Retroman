using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Advertisements;

using Sirenix.OdinInspector;

using Newtonsoft.Json;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    public struct OnHandleGraphRequestSignal
    {
        public Builder Builder;
        public Action<string> Parser;
    }

    public class AnonymousRequest : UnitRequest
    {
        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);
            
            this.Receive<OnHandleGraphRequestSignal>()
                .Subscribe(OnHandleRequest)
                .AddTo(this);
        }

        #region Requests
        private void OnHandleRequest(OnHandleGraphRequestSignal signal)
        {
            ProcessStringRequest(GraphInfo, signal.Builder.ToString(), signal.Parser);
        }
        #endregion
    }
}
