﻿using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Framework.Common.Editor
{
    public abstract class EnumGeneratorWindow : OdinEditorWindow
    {
        #region Constants

        private const string DEFAULT_EDITORFILENAME = "EnumGeneratorWindow.cs";
        private const string DEFAULT_NAMESPACE = "Framework";

        private const string REGEX_ENUM = "[^aA-zZ]|[\x5B\x5C\x5D\x5E\x60]";
        private const string REGEX_VALID_NAMESPACE = "(@?[a-z_A-Z]\\w+(?:\\.@?[a-z_A-Z]\\w+)*)$";
        private const string REGEX_VALID_ENUM = "^[a-zA-Z_][a-zA-Z_0-9]*";

        private const int T_EDITORFILENAME_IDX = 2;
        private const int T_DATEGENERATED_IDX = 3;
        private const int T_NAMESPACE_IDX = 8;
        private const int T_ENUMNAME_IDX = 11;
        private const int T_ENUMS_IDX = 14;

        #endregion Constants

        #region Static Fields

        /// <summary>
        /// An array containing all C# reserved keywords.
        /// </summary>
        public static readonly string[] CSharpKeywords = new string[]
        {
            "abstract", "as", "base", "bool", "break",
            "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum",
            "event", "explicit", "extern", "false", "finally",
            "fixed", "float", "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator", "out",
            "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", "using", "static",
            "virtual", "void", "volatile", "while"
        };

        /// <summary>
        /// An array containing the template used when generating the Enum type.
        /// </summary>
        public static readonly string[] Template = new string[]
        {
            "/// <summary>",
            "/// AUTO-GENERATED",
            "/// This file has been autogenerated by {0}",
            "/// Date Generated: {0}",
            "/// </summary>",
            "",
            "using System;",
            "",
            "namespace {0}",
            "{",
            "    [Serializable]",
            "    public enum {0}",
            "    {",
            "    ",
            "        {0}{1}\n",
            "    }",
            "}"
        };

        #endregion Static Fields

        #region Static Methods

        /// <summary>
        /// Deletes the file in the path if it exists.
        /// </summary>
        /// <param name="path"></param>
        protected static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Checks if the input is a valid enum type and is not a reserved keyword.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEnum(string input)
        {
            return Regex.IsMatch(input, REGEX_VALID_ENUM) && !CSharpKeywords.Contains(input);
        }

        /// <summary>
        /// Checks if the input is a valid namespace.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidNamespace(string input)
        {
            return Regex.IsMatch(input, REGEX_VALID_NAMESPACE);
        }

        /// <summary>
        /// Creates a file from the path along with its directory.
        /// </summary>
        /// <param name="path"></param>
        protected static void SafeCreateFile(string path)
        {
            if (!File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file = File.Create(path);

                file.Close();
            }
        }

        /// <summary>
        /// Gets an array of string for each line in the file path specified.
        /// Creates the file if does not exist and returns an empty string array.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string[] SafeGetLines(string path)
        {
            if (File.Exists(path))
                return File.ReadAllLines(path);
            else
            {
                SafeCreateFile(path);

                return new string[] { };
            }
        }

        /// <summary>
        /// Writes to the .dat file in the path specified with the types given.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="types"></param>
        protected static void WriteDatFile(string path, string[] types)
        {
            DeleteFile(path);

            if (!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    types.ForEach(x => writer.WriteLine(x));
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Writes to the enum script file in the path specified with the arguments given.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="types"></param>
        /// <param name="enumTypeName"></param>
        /// <param name="editorFileName"></param>
        /// <param name="nameSpace"></param>
        protected static void WriteScriptFile(string path, string[] types, string enumTypeName, string editorFileName = DEFAULT_EDITORFILENAME, string nameSpace = DEFAULT_NAMESPACE)
        {
            DeleteFile(path);

            if (!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    for (int i = 0; i < Template.Length; i++)
                    {
                        var template = Template[i];

                        switch (i)
                        {
                            case T_EDITORFILENAME_IDX:

                                writer.WriteLine(template, editorFileName);
                                break;

                            case T_DATEGENERATED_IDX:

                                writer.WriteLine(template, DateTime.Now.ToString());
                                break;

                            case T_NAMESPACE_IDX:

                                // Use the default namespace instead if the given namespace is not valid.
                                writer.WriteLine(template, IsValidNamespace(nameSpace) ? nameSpace : DEFAULT_NAMESPACE);
                                break;

                            case T_ENUMNAME_IDX:

                                // Replaces invalid characters with an underscore in the enumTypeName provided.
                                writer.WriteLine(template, Regex.Replace(enumTypeName, REGEX_ENUM, "_"));
                                break;

                            case T_ENUMS_IDX:

                                for (int j = 0; j < types.Length; j++)
                                {
                                    writer.WriteLine(template,
                                        types[j],
                                        (j < types.Length - 1) ? "," : string.Empty);
                                }

                                break;

                            default:

                                writer.WriteLine(template);
                                break;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        #endregion Static Methods

        #region Fields

        [ShowInInspector, PropertyOrder(201)]
        private List<string> Types = new List<string>();

        #endregion Fields

        #region Abstract Properties

        [ShowInInspector, PropertyOrder(101)]
        protected abstract string DatFilePath { get; }

        [ShowInInspector, PropertyOrder(102)]
        protected abstract string ScriptFilePath { get; }

        protected abstract string EditorFileName { get; }

        protected abstract string Namespace { get; }

        protected abstract string EnumName { get; }

        protected abstract string[] Defaults { get; }

        #endregion Abstract Properties

        #region Properties

        protected string DatFileDirectory => Path.GetDirectoryName(DatFilePath);

        protected string DatFileName => Path.GetFileName(DatFilePath);

        protected string ScriptFileDirectory => Path.GetDirectoryName(ScriptFilePath);

        protected string ScriptFileName => Path.GetFileName(ScriptFilePath);

        [ShowInInspector,
            HideIf("HideInvalidEnums"),
            MultiLineProperty,
            ReadOnly,
            PropertyOrder(110)]
        protected string InvalidEnums { get; set; } = string.Empty;

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            ReadDatFile();
        }

        public void SetTypes(string[] types)
        {
            Types.Clear();
            Types.AddRange(types);

            CleanTypes();
        }

        protected void CleanTypes()
        {
            InvalidEnums = string.Join(", ", Types.Where(x => !IsValidEnum(x)).Distinct());

            Types = Types.Where(x => !string.IsNullOrEmpty(x)
                && IsValidEnum(x))
                .Distinct()
                .ToList();

            if (Types.Count == 0
                && Defaults != null && Defaults.Length > 0)
            {
                Types.AddRange(Defaults);

                CleanTypes();
            }
        }

        [Button("Read from Dat File", ButtonSizes.Large), HorizontalGroup, PropertyOrder(1001)]
        protected void ReadDatFile()
        {
            SetTypes(SafeGetLines(DatFilePath));
        }

        [Button("Write to Dat File", ButtonSizes.Large), HorizontalGroup, PropertyOrder(1002)]
        protected void WriteDatFile()
        {
            CleanTypes();

            WriteDatFile(DatFilePath, Types.ToArray());
        }

        [Button("Write to Script File", ButtonSizes.Large), HorizontalGroup, PropertyOrder(1003)]
        protected void WriteScriptFile()
        {
            CleanTypes();

            WriteScriptFile(ScriptFilePath, Types.ToArray(), EnumName, EditorFileName, Namespace);
        }

        private bool HideInvalidEnums()
        {
            return string.IsNullOrEmpty(InvalidEnums);
        }

        #endregion Methods
    }
}