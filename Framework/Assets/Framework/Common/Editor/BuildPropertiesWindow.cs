#if ODIN_INSPECTOR

using Framework.ExtensionMethods;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildPropertiesWindow : OdinEditorWindow
{
    #region Constants

    #region Fields

    public const string OutputFilePath = "build.properties";

    public const string ProductNameField = "ProductName";
    public const string BundleIdentifierField = "BundleIdentifier";
    public const string BundleVersionField = "BundleVersion";
    public const string BuildFileNameField = "BuildFileName";

    public const string AndroidIsGameField = "AndroidIsGame";
    public const string AndroidBundleVersionCodeField = "AndroidBundleVersionCode";
    public const string AndroidUseApkExpansionFilesField = "AndroidUseApkExpansionFiles";

    public const string AndroidKeyStorePathField = "AndroidKeyStorePath";
    public const string AndroidKeyStorePassField = "AndroidKeyStorePass";
    public const string AndroidKeyAliasNameField = "AndroidKeyAliasName";
    public const string AndroidKeyAliasPassField = "AndroidKeyAliasPass";

    public const string IosApplicationDisplayNameField = "IosApplicationDisplayName";
    public const string IosBuildNumberField = "IosBuildNumber";

    public const string IosAppleDeveloperTeamIdField = "IosAppleDeveloperTeamId";
    public const string IosAppleEnabledAutomaticSigningField = "IosAppleEnabledAutomaticSigning";
    public const string IosManualProvisioningProfileIdField = "IosManualProvisioningProfileId";
    public const string IosManualProvisioningProfileTypeField = "IosManualProvisioningProfileType";

    public const string UnityCommandOptionsAndroidField = "UnityCommandOptionsAndroid";
    public const string UnityCommandOptionsIosField = "UnityCommandOptionsIos";

    #endregion Fields

    #region Indices

    public const int ProductNameIndex = 5;
    public const int BundleIdentifierIndex = 6;
    public const int BundleVersionIndex = 7;
    public const int BuildFileNameIndex = 8;

    public const int AndroidIsGameIndex = 11;
    public const int AndroidBundleVersionCodeIndex = 12;
    public const int AndroidUseApkExpansionFilesIndex = 13;

    public const int AndroidKeyStorePathIndex = 16;
    public const int AndroidKeyStorePassIndex = 17;
    public const int AndroidKeyAliasNameIndex = 18;
    public const int AndroidKeyAliasPassIndex = 19;

    public const int IosApplicationDisplayNameIndex = 22;
    public const int IosBuildNumberIndex = 23;

    public const int IosAppleDeveloperTeamIdIndex = 26;
    public const int IosAppleEnabledAutomaticSigningIndex = 27;
    public const int IosManualProvisioningProfileIdIndex = 28;
    public const int IosManualProvisioningProfileTypeIndex = 29;

    public const int UnityCommandOptionsAndroidIndex = 32;
    public const int UnityCommandOptionsIosIndex = 33;

    #endregion Indices

    #endregion Constants

    #region Statics

    public static readonly string[] Template =
    {
        "# Job Settings (These values are set by the job)",
        "#UNITYPROJECTPATH = \"\"",
        "#BUILDTARGET = \"\"",
        "",
        "# Project Settings",
        "{0} = \"{1}\"", // Product Name : 5
        "{0} = \"{1}\"", // Bundle Identifier : 6
        "{0} = \"{1}\"", // Bundle Version : 7
        "{0} = \"{1}\"", // Build File Name : 8
        "",
        "# Android Settings",
        "{0} = \"{1}\"", // Android Is Game : 11
        "{0} = \"{1}\"", // Android Bundle Version Code : 12
        "{0} = \"{1}\"", // Android Use APK Expansion Files : 13
        "",
        "# Android Build Settings",
        "{0} = \"{1}\"", // Android Key Store Path : 16
        "{0} = \"{1}\"", // Android Key Store Pass : 17
        "{0} = \"{1}\"", // Android Key Alias Name : 18
        "{0} = \"{1}\"", // Android Key Alias Pass : 19
        "",
        "# iOS Settings",
        "{0} = \"{1}\"", // Ios Application Display Name : 22
        "{0} = \"{1}\"", // Ios Build Number : 23
        "",
        "# iOS Build Settings",
        "{0} = \"{1}\"", // Ios Apple Developer Team Id : 26
        "{0} = \"{1}\"", // Ios Apple Enabled Automatic Signing : 27
        "{0} = \"{1}\"", // Ios Manual Provisioning Profile Id : 28
        "{0} = \"{1}\"", // Ios Manual Provisioning Profile Type : 29
        "",
        "# Auto-Generated",
        "{0} = {1}", // Unity Command Options Android : 32
        "{0} = {1}" // Unity Command Options Ios : 33
    };

    #endregion Statics

    #region Fields

    private bool _bundleVersionInMmpFormat;

    private int _bundleVersionMajor;

    private int _bundleVersionMinor;

    private int _bundleVersionPatch;

    [SerializeField, PropertyTooltip(" $" + ProductNameField)]
    private string _productName;

    [SerializeField, PropertyTooltip(" $" + BundleIdentifierField)]
    private string _bundleIdentifier;

    [SerializeField, PropertyTooltip(" $" + BundleVersionField)]
    [HorizontalGroup("BundleVersion"), ValidateInput("CheckBundleVersionMmp")]
    private string _bundleVersion;

    [SerializeField, PropertyTooltip(" $" + BuildFileNameField + ". You can also use environment variables set by the job. (e.g. ${BundleVersion}, ${BUILD_NUMBER}, ${BUILD_TIMESTAMP}, ${PROJECT_NAME}), etc.")]
    private string _buildFileName;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Is Game"), PropertyTooltip(" $" + AndroidIsGameField)]
    private bool _androidIsGame;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Bundle Version Code"), PropertyTooltip(" $" + AndroidBundleVersionCodeField)]
    private string _androidBundleVersionCode;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Use APK Expansion Files"), PropertyTooltip(" $" + AndroidUseApkExpansionFilesField)]
    private bool _androidUseApkExpansionFiles;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Key Store Path"), SuffixLabel("*Required"), PropertyTooltip(" $" + AndroidKeyStorePathField + ". Path is relative to the workspace path.")]
    private string _androidKeyStorePath;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Key Store Password"), SuffixLabel("*Required"), PropertyTooltip(" $" + AndroidKeyStorePassField)]
    private string _androidKeyStorePass;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Key Alias Name"), SuffixLabel("*Required"), PropertyTooltip(" $" + AndroidKeyAliasNameField)]
    private string _androidKeyAliasName;

    [SerializeField, TabGroup("Platform", "Android"), LabelText("Key Alias Password"), SuffixLabel("*Required"), PropertyTooltip(" $" + AndroidKeyAliasPassField)]
    private string _androidKeyAliasPass;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Application Display Name"), PropertyTooltip(" $" + IosApplicationDisplayNameField)]
    private string _iosApplicationDisplayName;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Build Number"), PropertyTooltip(" $" + IosBuildNumberField)]
    private string _iosBuildNumber;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Apple Developer Team Id"), PropertyTooltip(" $" + IosAppleDeveloperTeamIdField)]
    private string _iosAppleDeveloperTeamId;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Apple Enabled Automatic Signing"), PropertyTooltip(" $" + IosAppleEnabledAutomaticSigningField)]
    private bool _iosAppleEnabledAutomaticSigning;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Manual Provisioning Profile Id"), PropertyTooltip(" $" + IosManualProvisioningProfileIdField)]
    private string _iosManualProvisioningProfileId;

    [SerializeField, TabGroup("Platform", "iOS"), LabelText("Manual Provisioning Profile Type"), PropertyTooltip(" $" + IosManualProvisioningProfileTypeField)]
    private string _iosManualProvisioningProfileType;

    [SerializeField, ReadOnly, TabGroup("Platform", "Android"), LabelText("Unity Command-Line Arguments"), MultiLineProperty(6), PropertyTooltip(" $" + UnityCommandOptionsAndroidField)]
    private string _androidUnityCommandOptions;

    [SerializeField, ReadOnly, TabGroup("Platform", "iOS"), LabelText("Unity Command-Line Arguments"), MultiLineProperty(6), PropertyTooltip(" $" + UnityCommandOptionsIosField)]
    private string _iosUnityCommandOptions;

    #endregion Fields

    #region Public Methods

    public void Initialize()
    {
        ReadPropertiesFile();
    }

    #endregion

    #region Methods

    private bool CheckBundleVersionMmp(string bundleVersion)
    {
        _bundleVersionInMmpFormat = false;

        var bundleVersionMmp = bundleVersion.Split('.');

        var major = -1;

        if (bundleVersionMmp.Length > 0 && bundleVersionMmp[0].IsNotNullOrEmpty() &&
            int.TryParse(bundleVersionMmp[0], out major))
        {
            var minor = -1;

            if (bundleVersionMmp.Length > 1 && bundleVersionMmp[1].IsNotNullOrEmpty() &&
                int.TryParse(bundleVersionMmp[1], out minor))
            {
                var patch = -1;

                if (bundleVersionMmp.Length > 2 && bundleVersionMmp[2].IsNotNullOrEmpty() &&
                    int.TryParse(bundleVersionMmp[2], out patch))
                {
                    _bundleVersionInMmpFormat = true;

                    _bundleVersionMajor = major;

                    _bundleVersionMinor = minor;

                    _bundleVersionPatch = patch;

                    _bundleVersion = string.Format("{0}.{1}.{2}", major, minor, patch);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Cleans the line.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="prefix">The prefix.</param>
    /// <param name="quoted">if set to <c>true</c> [quoted].</param>
    /// <returns></returns>
    private string CleanLine(string line, string prefix, bool quoted = true)
    {
        var prefixLength = prefix.Length + 3 + (quoted ? 1 : 0);

        return line.Substring(prefixLength, line.Length - (prefixLength + (quoted ? 1 : 0)));
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    /// <summary>
    /// Formats the bundle version MMP.
    /// </summary>
    private void FormatBundleVersionMmp()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersion = string.Format("{0}.{1}.{2}", _bundleVersionMajor, _bundleVersionMinor, _bundleVersionPatch);
    }

    /// <summary>
    /// Generates the unity command options.
    /// </summary>
    /// <returns></returns>
    private string GenerateUnityCommandOptions(BuildTargetGroup buildTargetGroup)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append("-quit");
        stringBuilder.Append(" -batchmode");
        stringBuilder.Append(" -nographics");
        stringBuilder.Append(" -projectPath $UNITYPROJECTPATH");
        stringBuilder.Append(" -executeMethod BuildCommand.PerformBuild");
        stringBuilder.Append(" -buildTarget $BUILDTARGET");

        if (_buildFileName.IsNotNullOrEmpty())
            stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_BUILD_FILENAME, BuildFileNameField));

        if (_productName.IsNotNullOrEmpty())
            stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_PRODUCT_NAME, ProductNameField));

        if (_bundleIdentifier.IsNotNullOrEmpty())
            stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_BUNDLE_IDENTIFIER, BundleIdentifierField));

        if (_bundleVersion.IsNotNullOrEmpty())
            stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_BUNDLE_VERSION, BundleVersionField));

        switch (buildTargetGroup)
        {
            case BuildTargetGroup.Android:

                if (_androidIsGame)
                    stringBuilder.Append(string.Format(" -{0}", BuildCommand.ARGS_ANDROID_ISGAME));

                if (_androidBundleVersionCode.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_ANDROID_BUNDLEVERSIONCODE, AndroidBundleVersionCodeField));

                if (_androidUseApkExpansionFiles)
                    stringBuilder.Append(string.Format(" -{0}", BuildCommand.ARGS_ANDROID_USEAPKEXPANSIONFILES));

                if (_androidKeyStorePath.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_ANDROID_KEYSTORENAME, AndroidKeyStorePathField));

                if (_androidKeyStorePass.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_ANDROID_KEYSTOREPASS, AndroidKeyStorePassField));

                if (_androidKeyAliasName.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_ANDROID_KEYALIASNAME, AndroidKeyAliasNameField));

                if (_androidKeyAliasPass.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_ANDROID_KEYALIASPASS, AndroidKeyAliasPassField));

                break;

            case BuildTargetGroup.iOS:

                if (_iosApplicationDisplayName.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_IOS_APPLICATIONDISPLAYNAME, IosApplicationDisplayNameField));

                if (_iosBuildNumber.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_IOS_BUILDNUMBER, IosBuildNumberField));

                if (_iosAppleDeveloperTeamId.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_IOS_APPLEDEVELOPERTEAMID, IosAppleDeveloperTeamIdField));

                if (_iosAppleEnabledAutomaticSigning)
                    stringBuilder.Append(string.Format(" -{0}", BuildCommand.ARGS_IOS_APPLEENABLEDAUTOMATICSIGNING));

                if (_iosManualProvisioningProfileId.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_IOS_MANUALPROVISIONINGPROFILEID, IosManualProvisioningProfileIdField));

                if (_iosManualProvisioningProfileType.IsNotNullOrEmpty())
                    stringBuilder.Append(string.Format(" -{0} ${1}", BuildCommand.ARGS_IOS_MANUALPROVISIONINGPROFILETYPE, IosManualProvisioningProfileTypeField));

                break;
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Increments the major version.
    /// </summary>
    [Button("Major")]
    [HorizontalGroup("BundleVersion", Width = 50), ShowIf("_bundleVersionInMmpFormat")]
    private void IncrementMajorVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionMajor += 1;
        _bundleVersionMinor = 0;
        _bundleVersionPatch = 0;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Resets the major version.
    /// </summary>
    [Button("R")]
    [HorizontalGroup("BundleVersion", Width = 20), ShowIf("_bundleVersionInMmpFormat")]
    private void ResetMajorVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionMajor = 0;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Increments the minor version.
    /// </summary>
    [Button("Minor")]
    [HorizontalGroup("BundleVersion", Width = 50), ShowIf("_bundleVersionInMmpFormat")]
    private void IncrementMinorVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionMinor += 1;
        _bundleVersionPatch = 0;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Resets the minor version.
    /// </summary>
    [Button("R")]
    [HorizontalGroup("BundleVersion", Width = 20), ShowIf("_bundleVersionInMmpFormat")]
    private void ResetMinorVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionMinor = 0;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Increments the patch version.
    /// </summary>
    [Button("Patch")]
    [HorizontalGroup("BundleVersion", Width = 50), ShowIf("_bundleVersionInMmpFormat")]
    private void IncrementPatchVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionPatch += 1;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Resets the patch version.
    /// </summary>
    [Button("R")]
    [HorizontalGroup("BundleVersion", Width = 20), ShowIf("_bundleVersionInMmpFormat")]
    private void ResetPatchVersion()
    {
        if (!_bundleVersionInMmpFormat)
            return;

        _bundleVersionPatch = 0;

        FormatBundleVersionMmp();
    }

    /// <summary>
    /// Reads from file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private void ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (line.StartsWith(ProductNameField))
                _productName = CleanLine(line, ProductNameField);

            if (line.StartsWith(BundleIdentifierField))
                _bundleIdentifier = CleanLine(line, BundleIdentifierField);

            if (line.StartsWith(BundleVersionField))
            {
                _bundleVersion = CleanLine(line, BundleVersionField);

                CheckBundleVersionMmp(_bundleVersion);
            }

            if (line.StartsWith(BuildFileNameField))
                _buildFileName = CleanLine(line, BuildFileNameField);

            if (line.StartsWith(AndroidIsGameField))
                _androidIsGame = CleanLine(line, AndroidIsGameField) == "true";

            if (line.StartsWith(AndroidBundleVersionCodeField))
                _androidBundleVersionCode = CleanLine(line, AndroidBundleVersionCodeField);

            if (line.StartsWith(AndroidUseApkExpansionFilesField))
                _androidUseApkExpansionFiles = CleanLine(line, AndroidUseApkExpansionFilesField) == "true";

            if (line.StartsWith(AndroidKeyStorePathField))
                _androidKeyStorePath = CleanLine(line, AndroidKeyStorePathField);

            if (line.StartsWith(AndroidKeyStorePassField))
                _androidKeyStorePass = CleanLine(line, AndroidKeyStorePassField);

            if (line.StartsWith(AndroidKeyAliasNameField))
                _androidKeyAliasName = CleanLine(line, AndroidKeyAliasNameField);

            if (line.StartsWith(AndroidKeyAliasPassField))
                _androidKeyAliasPass = CleanLine(line, AndroidKeyAliasPassField);

            if (line.StartsWith(IosApplicationDisplayNameField))
                _iosApplicationDisplayName = CleanLine(line, IosApplicationDisplayNameField);

            if (line.StartsWith(IosBuildNumberField))
                _iosBuildNumber = CleanLine(line, IosBuildNumberField);

            if (line.StartsWith(IosAppleDeveloperTeamIdField))
                _iosAppleDeveloperTeamId = CleanLine(line, IosAppleDeveloperTeamIdField);

            if (line.StartsWith(IosAppleEnabledAutomaticSigningField))
                _iosAppleEnabledAutomaticSigning = CleanLine(line, IosAppleEnabledAutomaticSigningField) == "true";

            if (line.StartsWith(IosManualProvisioningProfileIdField))
                _iosManualProvisioningProfileId = CleanLine(line, IosManualProvisioningProfileIdField);

            if (line.StartsWith(IosManualProvisioningProfileTypeField))
                _iosManualProvisioningProfileType = CleanLine(line, IosManualProvisioningProfileTypeField);
        }
    }

    /// <summary>
    /// Reads the properties file.
    /// </summary>
    [Button("Read from Properties File", ButtonSizes.Large)]
    private void ReadPropertiesFile()
    {
        ReadFromFile(OutputFilePath);
    }

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private void WriteToFile(string filePath)
    {
        DeleteFile(filePath);

        if (File.Exists(filePath))
            return;

        _androidUnityCommandOptions = GenerateUnityCommandOptions(BuildTargetGroup.Android);
        _iosUnityCommandOptions = GenerateUnityCommandOptions(BuildTargetGroup.iOS);

        using (var streamWriter = new StreamWriter(filePath))
        {
            for (var i = 0; i < Template.Length; i++)
            {
                var line = Template[i];

                switch (i)
                {
                    case ProductNameIndex: streamWriter.WriteLine(line, ProductNameField, _productName); break;
                    case BundleIdentifierIndex: streamWriter.WriteLine(line, BundleIdentifierField, _bundleIdentifier); break;
                    case BundleVersionIndex: streamWriter.WriteLine(line, BundleVersionField, _bundleVersion); break;
                    case BuildFileNameIndex: streamWriter.WriteLine(line, BuildFileNameField, _buildFileName); break;

                    case AndroidIsGameIndex: streamWriter.WriteLine(line, AndroidIsGameField, _androidIsGame.ToString().ToLower()); break;
                    case AndroidBundleVersionCodeIndex: streamWriter.WriteLine(line, AndroidBundleVersionCodeField, _androidBundleVersionCode); break;
                    case AndroidUseApkExpansionFilesIndex: streamWriter.WriteLine(line, AndroidUseApkExpansionFilesField, _androidUseApkExpansionFiles.ToString().ToLower()); break;

                    case AndroidKeyStorePathIndex: streamWriter.WriteLine(line, AndroidKeyStorePathField, _androidKeyStorePath); break;
                    case AndroidKeyStorePassIndex: streamWriter.WriteLine(line, AndroidKeyStorePassField, _androidKeyStorePass); break;
                    case AndroidKeyAliasNameIndex: streamWriter.WriteLine(line, AndroidKeyAliasNameField, _androidKeyAliasName); break;
                    case AndroidKeyAliasPassIndex: streamWriter.WriteLine(line, AndroidKeyAliasPassField, _androidKeyAliasPass); break;

                    case IosApplicationDisplayNameIndex: streamWriter.WriteLine(line, IosApplicationDisplayNameField, _iosApplicationDisplayName); break;
                    case IosBuildNumberIndex: streamWriter.WriteLine(line, IosBuildNumberField, _iosBuildNumber); break;

                    case IosAppleDeveloperTeamIdIndex: streamWriter.WriteLine(line, IosAppleDeveloperTeamIdField, _iosAppleDeveloperTeamId); break;
                    case IosAppleEnabledAutomaticSigningIndex: streamWriter.WriteLine(line, IosAppleEnabledAutomaticSigningField, _iosAppleEnabledAutomaticSigning.ToString().ToLower()); break;
                    case IosManualProvisioningProfileIdIndex: streamWriter.WriteLine(line, IosManualProvisioningProfileIdField, _iosManualProvisioningProfileId); break;
                    case IosManualProvisioningProfileTypeIndex: streamWriter.WriteLine(line, IosManualProvisioningProfileTypeField, _iosManualProvisioningProfileType); break;

                    case UnityCommandOptionsAndroidIndex: streamWriter.WriteLine(line, UnityCommandOptionsAndroidField, _androidUnityCommandOptions); break;
                    case UnityCommandOptionsIosIndex: streamWriter.WriteLine(line, UnityCommandOptionsIosField, _iosUnityCommandOptions); break;

                    default: streamWriter.WriteLine(line); break;
                }
            }
        }
    }

    /// <summary>
    /// Writes the properties file.
    /// </summary>
    [Button("Write to Properties File", ButtonSizes.Large)]
    private void WritePropertiesFile()
    {
        WriteToFile(OutputFilePath);
    }

    #endregion Methods

    #region Static Methods

    [MenuItem("Framework/Build Automation/Properties")]
    private static BuildPropertiesWindow GetWindow()
    {
        var window = GetWindow<BuildPropertiesWindow>(true, "Build Properties", true);

        window.Initialize();

        window.Show();

        return window;
    }

    #endregion Static Methods
}

#endif