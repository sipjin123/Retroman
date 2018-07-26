using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Framework.Common.Editor
{
    /// <summary>
    /// Creates a dockable window that contains all scenes included in the build with buttons for quick navigation.
    /// </summary>
    public class ScenesInBuildWindow : EditorWindow
    {
        #region Static Methods

        [MenuItem("Window/Scenes in Build %`", false, 1998)]
        private static void Initialize()
        {
            ScenesInBuildWindow scenesInBuildWindow = GetWindow<ScenesInBuildWindow>(false, "Scenes in Build", true);
            scenesInBuildWindow.Repaint();
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

                    if (GUILayout.Button(buttonText, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft }))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(scene.path);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion EditorWindow Methods
    }
}