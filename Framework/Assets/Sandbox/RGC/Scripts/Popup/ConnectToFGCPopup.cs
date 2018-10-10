using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

using Sandbox.GraphQL;
using Sandbox.Popup;

namespace Sandbox.RGC
{
    using TMPro;

    using Toggle = UnityEngine.UI.Toggle;

    public class ConnectToFGCPopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private Toggle TogglePopup;

        protected override void Awake()
        {
            base.Awake();

            Assertion.AssertNotNull(TogglePopup);

            TogglePopup
                .OnValueChangedAsObservable()
                .Subscribe(status => this.Publish(new OnToggleFGCPopup() { Show = !status  }))
                .AddTo(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.Publish(new OnToggleFGCPopup() { Show = !TogglePopup.isOn });
        }
    }
}