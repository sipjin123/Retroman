using Framework.Experimental.ScriptableObjects;
using UniRx;
using UnityEngine;

namespace Assets.Framework.Experimental.ScriptableObjects.Example2
{
    public class ScriptableObjectSystemTest : MonoBehaviour
    {
        public IntReference IntReference;

        public FloatReference FloatReference;

        public StringReference StringReference;

        private void Awake()
        {
            IntReference.AsObservable()
                .Subscribe(x =>
                    Debug.LogFormat("Your IntReference object is changing its value to: {0}", x))
                .AddTo(this);

            FloatReference.AsObservable()
                 .Subscribe(x =>
                     Debug.LogFormat("Your FloatReference object is changing its value to: {0}", x))
                 .AddTo(this);

            StringReference.AsObservable()
                 .Subscribe(x =>
                     Debug.LogFormat("Your StringReference object is changing its value to: {0}", x))
                 .AddTo(this);
        }
    }
}