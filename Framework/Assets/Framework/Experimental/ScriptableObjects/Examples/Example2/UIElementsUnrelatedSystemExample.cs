using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

namespace Framework.Experimental.ScriptableObjects.Example2
{
    public class UIElementsUnrelatedSystemExample : MonoBehaviour
    {
        public Text Text;

        public StringVariable StringVariable;

        private void Awake()
        {
            StringVariable.AsObservable()
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => Text.text = x)
                .AddTo(this);
        }
    }
}