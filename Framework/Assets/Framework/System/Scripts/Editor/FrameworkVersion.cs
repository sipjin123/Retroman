using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Sirenix.OdinInspector;

namespace Framework
{
    /// <summary>
    /// Editor tool for exporting template modules.
    /// </summary>
    public class FrameworkVersion : SearchableEditorWindow
    {
        [MenuItem("Framework/Version")]
        public static void Init()
        {
            FrameworkVersion window = EditorWindow.GetWindow<FrameworkVersion>();
        }
        
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.TextField("v3.0.1", GUILayout.MaxWidth(position.width));
            GUILayout.EndHorizontal();
        }
    }
}
