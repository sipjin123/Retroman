using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Framework.Common.Editor;

using Sirenix.OdinInspector;

namespace Sandbox.ButtonSandbox
{
    public class ButtonTypeEditor : EnumClassGeneratorWindow
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

        protected override string ScriptFilePath => "Assets/Sandbox/Button/Scripts/ButtonTypePartials.cs";

        protected override string EditorFileName => "ButtonTypeEditor.cs";

        protected override string Namespace => "Sandbox.ButtonSandbox";

        protected override string ClassName => "ButtonType";

        protected override string[] Defaults => new string[]
                                                    {
                                                        "Invalid",
                                                        "Popup001",
                                                        "Popup002",
                                                        "Popup003",
                                                        "Close",
                                                        "Facebook",
                                                        "FGC",
                                                        "DownloadFGC",
                                                        "ConnectToFGC",
                                                        "GetSynerytix",
                                                        "ConvertSynertix",
                                                    };

        //[ShowInInspector, PropertyOrder(201)]
        //private List<GameObject> GameObjects = new List<GameObject>();

        #endregion Properties
    }
}