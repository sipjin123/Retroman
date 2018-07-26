using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Framework
{
    /// <summary>
    /// Editor tool for exporting template modules.
    /// </summary>
    public class FrameworkMacroConfiguration : EditorWindow
    {
        [Serializable]
        public class Macro
        {
            public string Name = string.Empty;
            public bool IsEnabled = false;
            public bool ToRemove { get; set; }

            public string ToJson()
            {
                return EditorJsonUtility.ToJson(this);
            }
        }

        private List<Macro> Macros;
        private string DEFINE = "-define:";

        [MenuItem("Framework/Macro")]
        public static void Init()
        {
            FrameworkMacroConfiguration window = EditorWindow.GetWindow<FrameworkMacroConfiguration>();
        }
        
        void OnEnable()
        {
            InitializeMacroSettings();
        }

        void OnGUI()
        {
            if (Macros == null)
            {
                GUILayout.Label("Macros list not initialized");
                return;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Insert"))
            {
                AddNewEntry();
            }

            if (GUILayout.Button("Pop"))
            {
                PopEntry();
            }

            if (GUILayout.Button("Export"))
            {
                ExportEntries();
            }
            GUILayout.EndHorizontal();

            foreach (Macro macro in Macros)
            {
                GUILayout.BeginHorizontal();
                macro.Name = GUILayout.TextField(macro.Name.ToUpper().Trim(), GUILayout.MaxWidth(position.width));
                macro.IsEnabled = GUILayout.Toggle(macro.IsEnabled, "Is Enabled");

                // remove
                if (GUILayout.Button("x"))
                {
                    macro.ToRemove = true;
                }

                GUILayout.EndHorizontal();
            }

            // remove all flagged macros
            Macros.RemoveAll(m => m.ToRemove);
        }

        private void InitializeMacroSettings()
        {
            string[] macros = File.ReadAllLines("FrameworkFiles/FrameworkMacros.dat");
            Macros = new List<Macro>();
            foreach (string macroJon in macros)
            {
                //Debug.LogWarning(moduleJson);
                Macro macro = new Macro();
                EditorJsonUtility.FromJsonOverwrite(macroJon, macro);

                if (string.IsNullOrEmpty(macro.Name))
                    continue;

                if (Macros.Exists(m => m.Name.ToUpper().Equals(macro.Name.ToUpper())))
                    continue;

                macro.Name = macro.Name.ToUpper().Trim();
                Macros.Add(macro);
            }
        }

        private void AddNewEntry()
        {
            Macros.Add(new Macro());
        }

        private void PopEntry()
        {
            if (Macros == null)
                return;

            if (Macros.Count <= 0)
                return;

            Macros.RemoveAt(Macros.Count - 1);
        }

        private void Delete(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        private void ExportEntries()
        {
            // Clear empty strings
            Macros.RemoveAll(m => string.IsNullOrEmpty(m.Name));

            using (StreamWriter file = new StreamWriter("FrameworkFiles/FrameworkMacros.dat", false))
            {
                for (int i = 0; i < Macros.Count; i++)
                {
                    file.WriteLine(Macros[i].ToJson());
                }
            }

            // clear and delete if non
            if (Macros.Count <= 0 || Macros.FindAll(m => m.IsEnabled).Count <= 0)
            {
                Delete("Assets/csc.rsp");
                Delete("Assets/mcs.rsp");
                Delete("Assets/smcs.rsp");
                AssetDatabase.Refresh();
                return;
            }
            
            using (StreamWriter file = new StreamWriter("Assets/csc.rsp", false))
            {
                for (int i = 0; i < Macros.Count; i++)
                {
                    if (Macros[i].IsEnabled)
                    {
                        file.WriteLine(string.Format("{0}{1}", DEFINE, Macros[i].Name));
                    }
                }
            }

            using (StreamWriter file = new StreamWriter("Assets/mcs.rsp", false))
            {
                for (int i = 0; i < Macros.Count; i++)
                {
                    if (Macros[i].IsEnabled)
                    {
                        file.WriteLine(string.Format("{0}{1}", DEFINE, Macros[i].Name));
                    }
                }
            }

            using (StreamWriter file = new StreamWriter("Assets/smcs.rsp", false))
            {
                for (int i = 0; i < Macros.Count; i++)
                {
                    if (Macros[i].IsEnabled)
                    {
                        file.WriteLine(string.Format("{0}{1}", DEFINE, Macros[i].Name));
                    }
                }
            }

            // recompile classes
            FrameworkEditor.Recompile();
        }

        private void Delete()
        {
            if (File.Exists("FrameworkFiles/FrameworkMacros.dat"))
            {
                File.Delete("FrameworkFiles/FrameworkMacros.dat");
            }

            File.CreateText("FrameworkFiles/FrameworkMacros.dat");
        }
    }
}
