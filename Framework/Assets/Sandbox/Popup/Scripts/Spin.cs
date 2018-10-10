
using UnityEngine;

namespace Sandbox.Popup
{
    public class Spin : MonoBehaviour
    {
        private void Start()
        {
            iTween.RotateBy(gameObject, iTween.Hash("z", -1.0f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.loop, "ignoretimescale", true));
        }
    }
}

