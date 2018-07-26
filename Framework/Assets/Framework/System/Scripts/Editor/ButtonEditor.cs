using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Common;

namespace Framework
{
    /*
    // alias
    using CColor = Framework.Color;
    
    //[CustomEditor(typeof(Button), true)]
    public class ButtonEditor : DrawButtonEditor
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string[] HIDDEN = new string[] { "m_Script" };

        /// <summary>
        /// List of SceneTypes (Enum Representation)
        /// </summary>
        private string[] CachedButtons;
        private List<string> Buttons;
        
        private int ButtonIndex = -1;

        private SerializedProperty Button;

        /// <summary>
        /// This method is called on every focus of this gameobject (Button).
        /// This updates the Button enum automatically.
        /// </summary>
        private void Awake()
        {
            CachedButtons = File.ReadAllLines("FrameworkFiles/FrameworkButtons.dat");
            Buttons = new List<string>(CachedButtons);
            
            EditorUtility.SetDirty(this.GetTarget<Button>().gameObject);
        }

        private void OnEnable()
        {
            // Update the cache values
            CachedButtons = Buttons.ToArray();

            // Update data from editor
            Button = serializedObject.FindProperty("_Button");
            ButtonIndex = Buttons.IndexOf(Button.stringValue);

            Assertion.Assert(ButtonIndex >= 0, string.Format(ERROR + " ButtonEditor::OnEnable Invalid cached Button:{0} Button:{1}\n", Button.stringValue, GetTarget<Button>().name));
        }

        public override void OnInspectorGUI()
        {
            // Draw Script
            serializedObject.Update();
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            serializedObject.ApplyModifiedProperties();

            // Update Button from Editor cache
            UpdateButtonIndices();

            // Hides script field on Editor (Since it was already drawn above the Scene Indices)
            DrawPropertiesExcluding(serializedObject, HIDDEN);

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateButtonIndices()
        {
            // Updates Button Index based on the selected value from editor
            {
                int sceneIndex = EditorGUILayout.Popup("Button Type", ButtonIndex, CachedButtons);

                if (sceneIndex >= 0 && ButtonIndex != sceneIndex)
                {
                    ButtonIndex = sceneIndex;
                    Button.stringValue = Buttons[sceneIndex];
                    serializedObject.Update();

                    GetTarget<Button>().ButtonString = Buttons[sceneIndex];
                }
            }
        }
        
    }
    //*/
}