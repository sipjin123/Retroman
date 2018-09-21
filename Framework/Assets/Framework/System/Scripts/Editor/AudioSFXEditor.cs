using Framework.Common.Editor;
using System;
using UnityEditor;

namespace Framework
{
    public class AudioSFXEditor : EnumGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Audio SFX")]
        private static AudioSFXEditor GetWindow()
        {
            var window = GetWindow<AudioSFXEditor>(
                utility: true,
                title: "SFX Window",
                focus: true);

            window.Initialize();

            window.Show();

            return window;
        }

        /// <summary>
        /// Opens the editor window and adds to EScene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("CONTEXT/Audio/Add to Enum")]
        public static void AddToEnum(MenuCommand menuCommand)
        {
            var root = "Root";
            var type = menuCommand.context.GetType().Name;

            if (type.EndsWith(root, StringComparison.InvariantCultureIgnoreCase))
                type = type.Remove(type.Length - root.Length, root.Length);

            var window = GetWindow();
            window.AddToTypes(type);
        }

        #endregion Static Methods

        #region Properties

        protected override string DatFilePath => "FrameworkFiles/FrameworkAudioSFX.dat";

        protected override string ScriptFilePath => "Assets/Sandbox/Audio/Scripts/SFX.cs";

        protected override string EditorFileName => "AudioSFXEditor.cs";

        protected override string Namespace => "Framework";

        protected override string EnumName => "SFX";

        protected override string[] Defaults => new string[]
        {
            "Sfx001",
            "Sfx002",
        };

        #endregion Properties
    }
}