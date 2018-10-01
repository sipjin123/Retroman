using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Query;
using Common.Utils;

using Framework;

using Sandbox.Services;

namespace Sandbox.Network
{
    public struct OnPollSignal
    {

    }

    public class PollService : BaseService
    {
        private TimeSpan POLL = TimeSpan.FromSeconds(5);
        
        public override void InitializeService()
        {
            Observable.Timer(POLL)
               .RepeatSafe()
               .Subscribe(_ => TriggerPoll())
               .AddTo(this);

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            Observable.Timer(POLL)
                .RepeatSafe()
                .Subscribe(_ => TriggerPoll())
                .AddTo(this);

            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        private void TriggerPoll()
        {
            this.Publish(new OnPollSignal());
        }
    }
}