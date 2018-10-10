using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using TMPro;

namespace Sandbox.FGC
{
    public class TestConvert : MonoBehaviour
    {
        [SerializeField]
        private int Score;

        [SerializeField]
        private float Rate;

        [SerializeField]
        private float Progress;

        public TMP_InputField ScoreText;
        public TMP_InputField RateText;
        public Slider SliderProgress;

        [Button(ButtonSizes.Medium)]
        public void Convert()
        {
            ScoreText.text = Score.ToString();
            RateText.text = Rate.ToString();

            Progress = Score / (1f / Rate);
            SliderProgress.value = Progress;
        }
    }
}