using Framework.Common.Editor;
using UnityEditor;

namespace Framework
{
    public class SceneTypeEditor : EnumGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Scene Types")]
        private static void GetWindow()
        {
            var window = GetWindow<SceneTypeEditor>(
                utility: true,
                title: "Scene Types Window",
                focus: true);

            window.Initialize();

            window.Show();
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
                "Max",
                "Home",
                "Title",
                "Shop",
                "Result"
            };

        #endregion Properties
    }
}