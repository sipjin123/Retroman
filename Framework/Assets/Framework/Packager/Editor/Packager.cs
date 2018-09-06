using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;

#endif

namespace Framework.Packager
{
    using UObject = UnityEngine.Object;

    [CreateAssetMenu(
        fileName = DefaultFilename,
        menuName = Menuname,
        order = Menuorder)]
    public class Packager : ScriptableObject
    {
        #region Constants

        public const string DefaultFilename = "Packager";
        public const string Menuname = "Framework/Packager";
        public const int Menuorder = 1000;

        public const string DefaultPackagename = "Module";
        public const string FormatVersion = "-(v{0})";
        public const string Fileextension = ".unitypackage";
        public const ExportPackageOptions DefaultOptions = ExportPackageOptions.Default | ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;

        public const string OutputdirectoryError = "Output Directory is not valid.";
        public const string PackagenameError = "Package Name is not valid.";

        public const string TabgroupInfo = "Package Info";
        public const string TabgroupExportsettings = "Export Settings";

        private const int T_Title = 0;
        private const int T_Version = 1;
        private const int T_Description = 3;

        #endregion Constants

        #region Static Fields

        public static readonly string[] Template = {
            "# {0}",
            "> Version: {0}",
            "",
            "{0}",
            "",
            "## How to Get Started",
            "",
            "",
            "## Bugs",
            "",
            "",
            "## To-Dos"
        };

        #endregion

        #region Fields & Properties

#if ODIN_INSPECTOR

        [PropertyOrder(100)]
        [TabGroup(TabgroupInfo)]
        [ValidateInput("ValidatePackageName",
            PackagenameError,
            InfoMessageType.Warning)]
#endif
        public string PackageName;

#if ODIN_INSPECTOR

        [PropertyOrder(101)]
        [TabGroup(TabgroupInfo)]
#endif
        public string PackageVersion;

#if ODIN_INSPECTOR

        [PropertyOrder(102)]
        [TabGroup(TabgroupInfo)]
        [MultiLineProperty]
#else
        [Multiline]
#endif
        public string PackageDescription;

#if ODIN_INSPECTOR

        [PropertyOrder(201)]
        [TabGroup(TabgroupExportsettings)]
        [ValidateInput("ValidateOutputDirectory",
            OutputdirectoryError,
            InfoMessageType.Warning)]
        [OnInspectorGUI("OnInspectorGUI")]
#endif
        public string OutputDirectory;

#if ODIN_INSPECTOR

        [PropertyOrder(202)]
        [TabGroup(TabgroupExportsettings)]
        [EnumToggleButtons]
#endif
        public ExportPackageOptions Options = DefaultOptions;

#if ODIN_INSPECTOR

        [PropertyOrder(103)]
        [TabGroup(TabgroupInfo)]
#endif
        public UObject[] Assets;

#if ODIN_INSPECTOR

        [PropertyOrder(104)]
        [TabGroup(TabgroupInfo)]
#endif
        public string[] ToExport;

#if ODIN_INSPECTOR

        [PropertyOrder(203)]
        [TabGroup(TabgroupExportsettings)]
#endif
        public string[] ExcludeStartsWith;

#if ODIN_INSPECTOR

        [PropertyOrder(204)]
        [TabGroup(TabgroupExportsettings)]
#endif
        public string[] ExcludeEndsWith;

        #endregion Fields & Properties

        #region Properties

#if ODIN_INSPECTOR

        [PropertyOrder(200)]
        [TabGroup(TabgroupExportsettings)]
        [ShowInInspector]
#endif
        public string OutputFileName
        {
            get
            {
                return string.Format("{0}{1}{2}",
                    PackageName,
                    string.IsNullOrEmpty(PackageVersion)
                        ? string.Empty
                        : string.Format(FormatVersion, PackageVersion),
                    Fileextension);
            }
        }

        #endregion Properties

        #region Methods

#if ODIN_INSPECTOR

        [PropertyOrder(300)]
        [Button("Set Defaults", ButtonSizes.Large)]
#endif
        private void Init()
        {
            OutputDirectory = GetDefaultOutputDirectory();

            if (string.IsNullOrEmpty(PackageName) || PackageName == DefaultPackagename)
            {
                PackageName = GetDefaultPackageName();
            }

            ExcludeStartsWith = new[]
            {
                "Assets/Experimental",
                "Assets/Plugins"
            };

            ExcludeEndsWith = new[]
            {
                ".unitypackage",
                ".unitypackage.meta"
            };
        }

#if ODIN_INSPECTOR

        [PropertyOrder(400)]
        [Button("Generate README.md file", ButtonSizes.Large)]
#endif
        private void GenerateReadMeFile()
        {
            if (string.IsNullOrEmpty(OutputDirectory))
                return;

            var readMeFile = Path.Combine(OutputDirectory, "README.md");

            if (File.Exists(readMeFile))
                return;

            using (var writer = new StreamWriter(readMeFile))
            {
                for (var i = 0; i < Template.Length; i++)
                {
                    var template = Template[i];

                    switch (i)
                    {
                        case T_Title:

                            writer.WriteLine(template, PackageName);
                            break;

                        case T_Version:

                            writer.WriteLine(template, PackageVersion);
                            break;

                        case T_Description:

                            writer.WriteLine(template, PackageDescription);
                            break;

                        default:

                            writer.WriteLine(template);
                            break;
                    }
                }
            }

            AssetDatabase.Refresh();
        }

#if ODIN_INSPECTOR

        [PropertyOrder(502)]
        [ButtonGroup("Export")]
        [Button(ButtonSizes.Large)]
#endif
        [ContextMenu("Export")]
        private void Export()
        {
            if (!Validate())
                return;

            var assets = Assets.Where(x => x != null)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();

            var path = Path.Combine(OutputDirectory, OutputFileName);

            AssetDatabase.ExportPackage(assets, path, Options);

            AssetDatabase.Refresh();
        }

#if ODIN_INSPECTOR

        [PropertyOrder(503)]
        [ButtonGroup("Export")]
        [Button(ButtonSizes.Large)]
#endif
        private void ExperimentalExport()
        {
            if (!Validate())
                return;

            var path = Path.Combine(OutputDirectory, OutputFileName);

            AssetDatabase.ExportPackage(ToExport, path, ExportPackageOptions.Default | ExportPackageOptions.Interactive);

            AssetDatabase.Refresh();
        }

        private string GetDefaultOutputDirectory()
        {
            var path = AssetDatabase.GetAssetPath(this);

            return string.IsNullOrEmpty(path)
                ? string.Empty
                : Path.GetDirectoryName(path);
        }

        private string GetDefaultPackageName()
        {
            return string.IsNullOrEmpty(name)
                ? DefaultPackagename
                : name;
        }

#if ODIN_INSPECTOR

        [PropertyOrder(501)]
        [ButtonGroup("Export")]
        [Button(ButtonSizes.Large)]
#endif
        private void RefreshToExport()
        {
            var dependencies = new List<string>();

            foreach (var asset in Assets)
            {
                foreach (var item in AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset), true))
                {
                    if ((File.GetAttributes(item) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        foreach (var f in GetFilesInPath(item))
                        {
                            if (!dependencies.Contains(f) &&
                                PathStartsWithExcluded(f) &&
                                PathEndsWithExcluded(f))
                                dependencies.Add(f);
                        }
                    }
                    else
                    {
                        if (!dependencies.Contains(item) &&
                            PathStartsWithExcluded(item) &&
                            PathEndsWithExcluded(item))
                            dependencies.Add(item);
                    }
                }
            }

            for (var i = 0; i < dependencies.Count; i++)
                dependencies[i] = dependencies[i].Replace("\\", "/");

            ToExport = dependencies.ToArray();
        }

        private bool Validate()
        {
            if (!ValidateOutputDirectory(OutputDirectory))
            {
                Debug.LogWarning(OutputdirectoryError);

                return false;
            }

            if (!ValidatePackageName(PackageName))
            {
                Debug.LogWarning(PackagenameError);

                return false;
            }

            return true;
        }

        private bool ValidateOutputDirectory(string outputDirectory)
        {
            return !string.IsNullOrEmpty(outputDirectory);
        }

        private bool ValidatePackageName(string packageName)
        {
            return !string.IsNullOrEmpty(packageName);
        }

        private string[] GetFilesInPath(string path)
        {
            var files = new List<string>();

            files.AddRange(Directory.GetFiles(path));

            foreach (var p in Directory.GetDirectories(path))
                files.AddRange(GetFilesInPath(p));

            return files.ToArray();
        }

        private bool PathStartsWithExcluded(string path)
        {
            if (ExcludeStartsWith == null ||
                ExcludeStartsWith.Length == 0)
                return false;

            return !ExcludeStartsWith.Any(x => path.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool PathEndsWithExcluded(string path)
        {
            if (ExcludeEndsWith == null ||
                ExcludeEndsWith.Length == 0)
                return false;

            return !ExcludeEndsWith.Any(x => path.EndsWith(x, StringComparison.InvariantCultureIgnoreCase));
        }

        private void OnInspectorGUI()
        {
            if (!ValidateOutputDirectory(OutputDirectory))
                OutputDirectory = GetDefaultOutputDirectory();
        }

        #endregion Methods

        #region Unity Life Cycle

        private void Awake()
        {
            Init();
        }

        #endregion Unity Life Cycle
    }
}