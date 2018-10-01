using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Framework.Common.Editor;

using Sirenix.OdinInspector;

namespace Framework
{
    public class PopupTypeEditor : EnumClassGeneratorWindow
    {
        #region Static Methods

        [MenuItem("Framework/System/Popup Types")]
        private static void GetWindow()
        {
            var window = GetWindow<PopupTypeEditor>(
                utility: true,
                title: "Popup Types Window",
                focus: true);

            window.Initialize();

            window.Show();
        }

        #endregion Static Methods

        #region Properties

        protected override string DatFilePath => "FrameworkFiles/FrameworkPopup.dat";

        protected override string ScriptFilePath => "Assets/Sandbox/Popup/Scripts/PopupTypePartials.cs";

        protected override string EditorFileName => "PopupTypeEditor.cs";

        protected override string Namespace => "Sandbox.Popup";

        protected override string ClassName => "PopupType";

        protected override string[] Defaults => new string[]
            {
                "Invalid",
                "Popup001",
                "Popup002",
                "Popup003",
                "CustomAdPopupImage",
                "CustomAdPopupVideo",
                "Close"
            };

        //[ShowInInspector, PropertyOrder(201)]
        //private List<GameObject> GameObjects = new List<GameObject>();

        #endregion Properties
    }
}