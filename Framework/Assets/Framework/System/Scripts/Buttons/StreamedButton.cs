using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using UButton = UnityEngine.UI.Button;

namespace Framework
{
    public class StreamedButton : Button
    {
        [SerializeField]
        double StreamThreshold_MS = 250;

        private void Awake()
        {
            var ClickStream = GetComponent<UButton>().OnClickAsObservable();

            ClickStream
                .Buffer(ClickStream.Throttle(TimeSpan.FromMilliseconds(StreamThreshold_MS)))
                .Where(_ => _.Count >= 1)
                .ObserveOnMainThread()
                .Subscribe(_ => OnClickedButton())
                .AddTo(this);
        }
    }
}

