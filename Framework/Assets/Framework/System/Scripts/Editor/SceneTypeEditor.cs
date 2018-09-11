using Framework.Common.Editor;
using System;
using UnityEditor;

namespace Framework
{
    public class SceneTypeEditor : EnumGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Scene Types")]
        private static SceneTypeEditor GetWindow()
        {
            var window = GetWindow<SceneTypeEditor>(
                utility: true,
                title: "Scene Types Window",
                focus: true);

            window.Initialize();

            window.Show();

            return window;
        }

        /// <summary>
        /// Opens the editor window and adds to EScene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("CONTEXT/Scene/Add to Enum")]
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

        protected override string DatFilePath => "FrameworkFiles/FrameworkScenes.dat";

        protected override string ScriptFilePath => "Assets/Framework/System/Scripts/Data/SceneTypes.cs";

        protected override string EditorFileName => "SceneTypeEditor.cs";

        protected override string Namespace => "Framework";

        protected override string EnumName => "EScene";

        protected override string[] Defaults => new string[]
            {
                "Invalid",
                "System",
                "Services",
                "Audio",
                "Preloader",
                "PopupCollection",
                "Background",
                "Cleaner",
                "Max"
            };

        #endregion Properties
    }
}