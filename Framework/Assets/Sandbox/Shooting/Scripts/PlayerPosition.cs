using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class PlayerPosition : MonoBehaviour
    {
        public Slider SliderH;
        public Slider SliderV;

        [SerializeField, ShowInInspector]
        [MinMaxSlider(0f, 18f)]
        private float HorizontalThreshold = 8f;

        [SerializeField, ShowInInspector]
        [MinMaxSlider(0f, 8f)]
        private float VerticalThreshold = 8f;

        private void Start()
        {
            SliderH.OnValueChangedAsObservable()
                .Subscribe(slider =>
                {
                    Vector3 pos = transform.position;
                    pos.x = HorizontalThreshold * slider;
                    transform.position = pos;
                })
                .AddTo(this);

            SliderV.OnValueChangedAsObservable()
                .Subscribe(slider =>
                {
                    Vector3 pos = transform.position;
                    pos.z = VerticalThreshold * slider;
                    transform.position = pos;
                })
                .AddTo(this);
        }
    }
}