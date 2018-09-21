using Framework.Common.Editor;
using System;
using UnityEditor;

namespace Framework
{
    public class AudioBGMEditor : EnumGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Audio BGM")]
        private static AudioBGMEditor GetWindow()
        {
            var window = GetWindow<AudioBGMEditor>(
                utility: true,
                title: "BGM Window",
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

        protected override string DatFilePath => "FrameworkFiles/FrameworkAudioBGM.dat";

        protected override string ScriptFilePath => "Assets/Sandbox/Audio/Scripts/BGM.cs";

        protected override string EditorFileName => "AudioBGMEditor.cs";

        protected override string Namespace => "Framework";

        protected override string EnumName => "BGM";

        protected override string[] Defaults => new string[]
        {
            "Bgm001",
            "Bgm002",
        };

        #endregion Properties
    }
}