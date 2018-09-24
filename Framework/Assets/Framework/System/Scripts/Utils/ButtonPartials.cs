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

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace Framework
{
    // alias
    using UScene = UnityEngine.SceneManagement.Scene;
    using TriggerEvent = EventTrigger.TriggerEvent;

    /// <summary>
    /// This component should be attached to a button.
    /// This publishes signals for when the button is clicked, hovered, unhovered, pressed, and released.
    /// This can be extended by buttons that need to pass data when clicked (ex. ItemButton).
    /// </summary>
    public partial class Button : MonoBehaviour
        //IPointerClickHandler, 
        //IPointerEnterHandler, 
        //IPointerExitHandler,
        //IPointerDownHandler,
        //IPointerUpHandler
    {
        public static ValueDropdownList<string> Buttonlist;

        public static ValueDropdownList<string> GenerateButtonList()
        {
            List<string> buttons = new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkButtons.dat"));
            ValueDropdownList<string> dropdown = new ValueDropdownList<string>();
            buttons.ForEach(s => dropdown.Add(s));

            return dropdown;
        }

        [SerializeField]
        [HideInInspector]
        [DisableInEditorMode]
        private string CachedButton;

        private UnityAction<BaseEventData> OnClickedAction;
        private UnityAction<BaseEventData> OnHoveredAction;
        private UnityAction<BaseEventData> OnUnhoveredAction;
        private UnityAction<BaseEventData> OnPressedAction;
        private UnityAction<BaseEventData> OnReleasedAction;

        public ValueDropdownList<string> GetButtons()
        {
            Buttonlist = Buttonlist ?? GenerateButtonList();
            return Buttonlist;
        }
        
        [Button(25)]
        public void SetupButtonEvents()
        {
#if UNITY_EDITOR
            EventTrigger trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

            Action<EventTriggerType, UnityAction<BaseEventData>> AddEvent = (EventTriggerType type, UnityAction<BaseEventData> action) =>
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = type;
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener(action);
                trigger.triggers.Add(entry);
            };
            
            OnClickedAction += _ => OnClickedButton();
            OnHoveredAction += _ => OnHoveredButton();
            OnUnhoveredAction += _ => OnUnhoveredButton();
            OnPressedAction += _ => OnPressedButton();
            OnReleasedAction += _ => OnReleasedButton();

            AddEvent(EventTriggerType.PointerClick, OnClickedAction);
            AddEvent(EventTriggerType.PointerEnter, OnHoveredAction);
            AddEvent(EventTriggerType.PointerExit, OnUnhoveredAction);
            AddEvent(EventTriggerType.PointerDown, OnPressedAction);
            AddEvent(EventTriggerType.PointerUp, OnReleasedAction);
#endif
        }

        [Button(25)]
        public void CleanupButtonEvents()
        {
#if UNITY_EDITOR
            OnClickedAction -= _ => OnClickedButton();
            OnHoveredAction -= _ => OnHoveredButton();
            OnUnhoveredAction -= _ => OnUnhoveredButton();
            OnPressedAction -= _ => OnPressedButton();
            OnReleasedAction -= _ => OnReleasedButton();

            //EventTrigger trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
#endif
        }

    }
}