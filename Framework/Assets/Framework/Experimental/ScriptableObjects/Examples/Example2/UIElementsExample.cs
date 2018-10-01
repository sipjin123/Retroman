using Framework.Experimental.ScriptableObjects;
using UniRx;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects.Example2
{
    public class UIElementsExample : MonoBehaviour
    {
        public StringReference PlayerName;

        private void Awake()
        {
            PlayerName.AsObservable()
                 .Subscribe(x =>
                     Debug.LogFormat("Player Name: {0}", x))
                 .AddTo(this);
        }
    }
}