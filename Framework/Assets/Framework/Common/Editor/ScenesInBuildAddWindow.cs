using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using UScene = UnityEngine.SceneManagement.Scene;

namespace Framework.Common.Editor
{
    /// <summary>
    /// Creates a dockable window that contains all scenes included in the build with buttons for quick navigation.
    /// </summary>
    public class ScenesInBuildAddWindow : EditorWindow
    {
        private static ScenesInBuildAddWindow window = null;

        #region Static Methods

        [MenuItem("Window/Scenes in Build (Load Additive) %#`", false, 1999)]
        private static void Initialize()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            else
            {
                window = GetWindow<ScenesInBuildAddWindow>(false, "Scenes in Build (Load Additive)", true);
                window.Repaint();
            }
        }

        #endregion Static Methods

        #region Fields

        private int _EnabledIndex = -1;

        private Vector2 _ScrollPosition = Vector2.zero;

        #endregion Fields

        #region EditorWindow Methods

        /// <summary>
        ///
        /// </summary>
        protected virtual void OnGUI()
        {
            GUILayout.Label(PlayerSettings.productName + " (" + PlayerSettings.bundleVersion + ")", EditorStyles.boldLabel);

            _EnabledIndex = -1;
            _ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];

                if (scene != null)
                {
                    if (scene.enabled)
                        _EnabledIndex += 1;

                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    string buttonText = ((scene.enabled) ? ("☑ " + _EnabledIndex) : ("☒ #")) + ": " + sceneName;
                    
                    if (GUILayout.Button("Open Additively: " + buttonText, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft }))
                    {
                        UScene s = EditorSceneManager.GetSceneByPath(scene.path);

                        if (s.isLoaded)
                        {
                            EditorSceneManager.CloseScene(s, true);
                        }
                        else
                        {
                            EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion EditorWindow Methods
    }
}