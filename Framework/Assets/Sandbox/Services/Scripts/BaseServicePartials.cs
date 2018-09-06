using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Framework;

namespace Sandbox.Services
{
    using Sandbox.ButtonSandbox;

    using FColor = Framework.Color;

    public partial class BaseService : SerializedMonoBehaviour, IService
    {
        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonClickedSignal>> ButtonClickedMap = new Dictionary<int, Action<ButtonClickedSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonHoveredSignal>> ButtonHoveredMap = new Dictionary<int, Action<ButtonHoveredSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonUnhoveredSignal>> ButtonUnhoveredMap = new Dictionary<int, Action<ButtonUnhoveredSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonPressedSignal>> ButtonPressedMap = new Dictionary<int, Action<ButtonPressedSignal>>();

        /// <summary>
        /// Mapping of button types and click, hover, unhover, press, and release handlers.
        /// </summary>
        //[ShowInInspector]
        [TabGroup("New Group", "Buttons")]
        protected Dictionary<int, Action<ButtonReleasedSignal>> ButtonReleasaedMap = new Dictionary<int, Action<ButtonReleasedSignal>>();

        protected void SetupButtonReceivers()
        {
            this.Receive<ButtonClickedSignal>()
                .Subscribe(sig => OnClickedButton(sig))
                .AddTo(this);

            this.Receive<ButtonHoveredSignal>()
                .Subscribe(sig => OnHoveredButton(sig))
                .AddTo(this);

            this.Receive<ButtonUnhoveredSignal>()
                .Subscribe(sig => OnUnhoveredButton(sig))
                .AddTo(this);

            this.Receive<ButtonPressedSignal>()
                .Subscribe(sig => OnPressedButton(sig))
                .AddTo(this);

            this.Receive<ButtonReleasedSignal>()
                .Subscribe(sig => OnReleasedButton(sig))
                .AddTo(this);
        }

        protected void CleanupButtonReceivers()
        {
            ClearButtonHandler<int, ButtonClickedSignal>(ButtonClickedMap);
            ClearButtonHandler<int, ButtonHoveredSignal>(ButtonHoveredMap);
            ClearButtonHandler<int, ButtonUnhoveredSignal>(ButtonUnhoveredMap);
            ClearButtonHandler<int, ButtonPressedSignal>(ButtonPressedMap);
            ClearButtonHandler<int, ButtonReleasedSignal>(ButtonReleasaedMap);
        }

        #region Buttons
        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonClickedSignal> action)
        {
            ButtonClickedMap[button.Value] = (Action<ButtonClickedSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonHoveredSignal> action)
        {
            ButtonHoveredMap[button.Value] = (Action<ButtonHoveredSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonUnhoveredSignal> action)
        {
            ButtonUnhoveredMap[button.Value] = (Action<ButtonUnhoveredSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonPressedSignal> action)
        {
            ButtonPressedMap[button.Value] = (Action<ButtonPressedSignal>)action;
        }

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(ButtonType button, Action<ButtonReleasedSignal> action)
        {
            ButtonReleasaedMap[button.Value] = (Action<ButtonReleasedSignal>)action;
        }

        private void OnClickedButton(ButtonClickedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonClickedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnClickedButton Button:{0}\n", button);
                ButtonClickedMap[button](signal);
            }
        }

        private void OnHoveredButton(ButtonHoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonHoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnHoveredButton Button:{0}\n", button);
                ButtonHoveredMap[button](signal);
            }
        }

        private void OnUnhoveredButton(ButtonUnhoveredSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonUnhoveredMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnUnhoveredButton Button:{0}\n", button);
                ButtonUnhoveredMap[button](signal);
            }
        }

        private void OnPressedButton(ButtonPressedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonPressedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnPressedButton Button:{0}\n", button);
                ButtonPressedMap[button](signal);
            }
        }

        private void OnReleasedButton(ButtonReleasedSignal signal)
        {
            int button = signal.Button.Value;

            if (ButtonReleasaedMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat(D.F + "SceneObject::OnReleasedButton Button:{0}\n", button);
                ButtonReleasaedMap[button](signal);
            }
        }

        private void ClearButtonHandler<K, V>(Dictionary<K, Action<V>> handler)
        {
            if (handler != null)
            {
                handler.Clear();
                handler = null;
            }
        }
        #endregion
    }
}
