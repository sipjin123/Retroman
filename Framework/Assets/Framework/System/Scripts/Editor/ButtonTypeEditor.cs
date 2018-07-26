using Framework.Common.Editor;
using UnityEditor;

namespace Framework
{
    public class ButtonTypeEditor : EnumGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Button Types")]
        private static void GetWindow()
        {
            var window = GetWindow<ButtonTypeEditor>(
                utility: true,
                title: "Button Types Window",
                focus: true);

            window.Initialize();

            window.Show();
        }

        #endregion Static Methods

        #region Properties

        protected override string DatFilePath => "FrameworkFiles/FrameworkButtons.dat";

        protected override string ScriptFilePath => "Assets/Framework/System/Scripts/Buttons/ButtonTypes.cs";

        protected override string EditorFileName => "ButtonTypeEditor.cs";

        protected override string Namespace => "Framework";

        protected override string EnumName => "EButton";

        protected override string[] Defaults => new string[]
            {
                "Invalid",
                "Popup001",
                "Popup002",
                "Popup003",
                "Close"
            };

        #endregion Properties
    }
}