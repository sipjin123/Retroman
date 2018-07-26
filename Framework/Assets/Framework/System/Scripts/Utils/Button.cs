using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using Common;
using Common.Signal;

namespace Framework
{
    // alias
    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This component should be attached to a button.
    /// This publishes signals for when the button is clicked, hovered, unhovered, pressed, and released.
    /// This can be extended by buttons that need to pass data when clicked (ex. ItemButton).
    /// </summary>
	public partial class Button : MonoBehaviour
    {
        /// <summary>
        /// Do not edit! cached values for Editor.
        /// Stores the string value of EButton enum of this button.
        /// </summary>
        [SerializeField]
        [LabelText("ButtonType")]
        [ValueDropdown("Buttons")]
        [OnValueChanged("UpdateButtonTypeString")]
        private string SelectedButton;

        /// <summary>
        /// The type of button this is.
        /// </summary>
        protected EButton _ButtonType;
        public EButton ButtonType
        {
            get { return _ButtonType; }
            private set { _ButtonType = value; }
        }

        private void Awake()
        {
            // Update Button Type & Depth from Editor
            SelectedButton = CachedButton;
            ButtonType = CachedButton.ToEnum<EButton>();
        }

        private void Start()
        {
            Assertion.Assert(ButtonType != EButton.Invalid);

        }
        
        private void OnEnable()
        {
            // Update Button Type & Depth from Editor
            SelectedButton = CachedButton;
            ButtonType = CachedButton.ToEnum<EButton>();
        }

        private void Reset()
        {
            Debug.LogFormat(D.CHECK + "Button::Reset Name:{0} Setup EventTrigger!\n", gameObject.name);
            SetupButtonEvents();
        }
        
        /// <summary>
        /// This should be called when the button is clicked.
        /// This publishes ButtonClickedSignal.
        /// </summary>
        public void OnClickedButton()
        {
            this.Publish(new ButtonClickedSignal()
            {
                Button = ButtonType
            });
		}
        
        /// <summary>
        /// This should be called when the button is hovered.
        /// This publishes ButtonHoveredSignal.
        /// </summary>
        public void OnHoveredButton()
        {
            this.Publish(new ButtonHoveredSignal()
            {
                Button = ButtonType
            });
        }

        /// <summary>
        /// This should be called when the button is unhovered.
        /// This publishes ButtonUnhoveredSignal.
        /// </summary>
        public void OnUnhoveredButton()
        {
            this.Publish(new ButtonUnhoveredSignal()
            {
                Button = ButtonType
            });
        }

        /// <summary>
        /// This should be called when the button is pressed.
        /// This publishes ButtonPressedSignal.
        /// </summary>
        public void OnPressedButton()
        {
            this.Publish(new ButtonPressedSignal()
            {
                Button = ButtonType
            });
        }

        /// <summary>
        /// This should be called when the pointer is released on top of the button.
        /// This publishes ButtonReleasedSignal.
        /// </summary>
        public void OnReleasedButton()
        {
            this.Publish(new ButtonReleasedSignal()
            {
                Button = ButtonType
            });
        }
    }
}