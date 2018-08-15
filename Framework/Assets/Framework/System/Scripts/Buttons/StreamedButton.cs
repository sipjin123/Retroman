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
        private double StreamThreshold_MS = 250;

        private UniRx.IObservable<Unit> ClickStream;
        private CompositeDisposable Disposables;

        protected override void Start()
        {
            base.Start();

            Debug.LogErrorFormat(D.LOG + "StreamedButton::Start Button:{0}\n", gameObject.name);
            Disposables = new CompositeDisposable();
            ClickStream = GetComponent<UButton>().OnClickAsObservable();

            /*
            ClickStream
                .Buffer(ClickStream.Throttle(TimeSpan.FromMilliseconds(StreamThreshold_MS)))
                .Where(_ => _.Count >= 1)
                //.ObserveOnMainThread()
                .Subscribe(_ => OnClickedButton())
                .AddTo(Disposables);
            //*/

            ClickStream
                .Take(1)
                .Subscribe(_ =>
                {
                    Debug.LogErrorFormat(D.LOG + "StreamedButton::Start::OnClicked Button:{0}\n", gameObject.name);
                    OnClickedButton();
                })
                .AddTo(Disposables);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Debug.LogErrorFormat(D.LOG + "StreamedButton::OnDestroy Button:{0}\n", gameObject.name);
            Disposables.Clear();
        }
    }
}