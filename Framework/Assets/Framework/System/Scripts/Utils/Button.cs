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
    using Sandbox.ButtonSandbox;

    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This component should be attached to a button.
    /// This publishes signals for when the button is clicked, hovered, unhovered, pressed, and released.
    /// This can be extended by buttons that need to pass data when clicked (ex. ItemButton).
    /// </summary>
	public partial class Button : MonoBehaviour
    {
        [SerializeField]
        [LabelText("ButtonType")]
        [ValueDropdown("GetButtons")]
        protected string _Button;
        public int ButtonType
        {
            get { return _Button.ToButtonValue(); }
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
                Button = ButtonType.ToButton()
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
                Button = ButtonType.ToButton()
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
                Button = ButtonType.ToButton()
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
                Button = ButtonType.ToButton()
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
                Button = ButtonType.ToButton()
            });
        }
    }
}