using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif

/// <summary>
/// A static class used for building a Unity project through the command-line.
/// <para>
/// Usage:
/// ../{UNITY_PATH}/Unity.exe -quit -batchmode -projectPath "{PROJECT_PATH}" -executeMethod "BuildCommand.PerformBuild" -buildTarget "{BUILD_TARGET}"
/// </para>
/// <para>
/// Arguments:
/// <para>
/// -quit (no longer needed)
/// , -batchmode (required)
/// , -projectPath (required)
/// , -executeMethod (required)
/// , -buildTarget (required)
/// , -xBuildFileName (optional)
/// , -oProductName (optional)
/// , -oBundleIdentifier (optional)
/// , -oBundleVersion (optional)
/// </para>
/// <para>
/// -oAndroidIsGame (optional)
/// , -oAndroidBundleVersionCode (optional)
/// , -oAndroidKeyAliasName (required in Android)
/// , -oAndroidKeyAliasPass (required in Android)
/// , -oAndroidKeyStoreName (required in Android)
/// , -oAndroidKeyStorePass (required in Android)
/// , -oAndroidUseApkExpansionFiles (optional)
/// </para>
/// <para>
/// -oIosAppleDeveloperTeamId (optional)
/// , -oIosAppleEnabledAutomaticSigning (optional)
/// , -oIosApplicationDisplayName (optional)
/// , -oIosBuildNumber (optional)
/// , -oIosManualProvisioningProfileId (optional)
/// , -oIosManualProvisioningProfileType (optional)
/// </para>
/// Tested on Unity Versions:
/// 5.6,
/// 2017.2, 2017.4,
/// 2018.1, 2018.2
/// </para>
/// </summary>
public static class BuildCommand
{
    #region Constants

    private const string BUILD_FOLDER = "Builds";
    private const string BUILDS_PREFIX = "Build";
    private const string CONSOLE_FORMATTER = "----------> {0}";

    public const string ARGS_BUILD_TARGET = "buildTarget";
    public const string ARGS_BUILD_FILENAME = "xBuildFileName";

    public const string ARGS_PRODUCT_NAME = "oProductName";
    public const string ARGS_BUNDLE_IDENTIFIER = "oBundleIdentifier";
    public const string ARGS_BUNDLE_VERSION = "oBundleVersion";

    public const string ARGS_ANDROID_ISGAME = "oAndroidIsGame";
    public const string ARGS_ANDROID_BUNDLEVERSIONCODE = "oAndroidBundleVersionCode";
    public const string ARGS_ANDROID_KEYALIASNAME = "oAndroidKeyAliasName";
    public const string ARGS_ANDROID_KEYALIASPASS = "oAndroidKeyAliasPass";
    public const string ARGS_ANDROID_KEYSTORENAME = "oAndroidKeyStoreName";
    public const string ARGS_ANDROID_KEYSTOREPASS = "oAndroidKeyStorePass";
    public const string ARGS_ANDROID_USEAPKEXPANSIONFILES = "oAndroidUseApkExpansionFiles";

    public const string ARGS_IOS_APPLEDEVELOPERTEAMID = "oIosAppleDeveloperTeamId";
    public const string ARGS_IOS_APPLEENABLEDAUTOMATICSIGNING = "oIosAppleEnabledAutomaticSigning";
    public const string ARGS_IOS_APPLICATIONDISPLAYNAME = "oIosApplicationDisplayName";
    public const string ARGS_IOS_BUILDNUMBER = "oIosBuildNumber";
    public const string ARGS_IOS_MANUALPROVISIONINGPROFILEID = "oIosManualProvisioningProfileId";
    public const string ARGS_IOS_MANUALPROVISIONINGPROFILETYPE = "oIosManualProvisioningProfileType";

    private const string REGEX_FILENAME = "[^aA-zZ0-9]|[\x5B\x5C\x5D\x5E\x60]";

    #endregion Constants

    #region Methods

    /// <summary>
    /// Performs a /BuildPipeline.BuildPlayer(...).
    /// While looking out for custom arguments.
    /// </summary>
    public static void PerformBuild()
    {
        ConsoleWriteLine("Performing build...");

        #region Get Required Values.

        var buildTarget = GetBuildTargetArgs();
        var buildTargetGroup = buildTarget.ToBuildTargetGroup();

        #endregion Get Required Values.

        #region Get & Set Optional Argument Values.

        string oFileName;
        if (!FetchArgumentValue(ARGS_BUILD_FILENAME, out oFileName))
            oFileName = PlayerSettings.productName;

        oFileName = CleanFileName(oFileName);

        string oProductName;
        if (FetchArgumentValue(ARGS_PRODUCT_NAME, out oProductName))
        {
            PlayerSettings.productName = oProductName;
            ConsoleWriteLine("PlayerSettings.productName = {0}", oProductName);
        }

        string oBundleIdentifier;
        if (FetchArgumentValue(ARGS_BUNDLE_IDENTIFIER, out oBundleIdentifier))
        {
            PlayerSettings.SetApplicationIdentifier(buildTargetGroup, oBundleIdentifier);
            ConsoleWriteLine("PlayerSettings.SetApplicationIdentifier({0})", oBundleIdentifier);
        }

        string oBundleVersion;
        if (FetchArgumentValue(ARGS_BUNDLE_VERSION, out oBundleVersion))
        {
            PlayerSettings.bundleVersion = oBundleVersion;
            ConsoleWriteLine("PlayerSettings.bundleVersion = {0}", oBundleVersion);
        }

        #region Platform Specific

        switch (buildTargetGroup)
        {
            case BuildTargetGroup.Android:
                ProcessOptionalAndroidArguments();
                break;
            case BuildTargetGroup.iOS:
                ProcessOptionaliOSArguments();
                break;
        }

        #endregion Platform Specific

        #endregion Get & Set Optional Argument Values.

        #region Prepare Required Values

        var scenes = GetEnabledScenes();
        var buildPath = GenerateBuildPath(buildTarget, oFileName);

        #endregion Prepare Required Values

        ConsoleWriteLine(string.Format("Scenes: {0}", string.Join(",", scenes)));
        ConsoleWriteLine(string.Format("Build Target: {0}", buildTarget));
        ConsoleWriteLine(string.Format("Build Path: {0}", buildPath));

        var buildSuccess = true;
        var buildMessage = "N/A";

#if UNITY_2018
        var buildReport = DoBuild_BuildReport(scenes, buildPath, buildTarget);
        var buildSummary = buildReport.summary;
        var buildResult = buildSummary.result;

        buildSuccess = buildResult == BuildResult.Succeeded;
        buildMessage = string.Format("==================================================\n" +
                                     "{0}\n" +
                                     "- Total Warnings: {1}\n" +
                                     "- Total Errors: {2}\n" +
                                     "==================================================",
            buildResult.ToString().ToUpper(),
            buildSummary.totalWarnings,
            buildSummary.totalErrors);
#else
        var buildReport = DoBuild_String(scenes, buildPath, buildTarget);
        buildSuccess = string.IsNullOrEmpty(buildReport);
        buildMessage = string.Format("==================================================\n" +
                                     "{0}\n" +
                                     "- Output: {1}\n" +
                                     "==================================================",
            buildSuccess.ToString().ToUpper(),
            buildReport);
#endif

        ConsoleWriteLine("Done with build...\n", buildMessage);

        if (!buildSuccess)
            EditorApplication.Exit(1);
    }

    public static void DoBuild(string[] levels, string locationPathName, BuildTarget target, BuildOptions options = BuildOptions.None)
    {
        BuildPipeline.BuildPlayer(levels, locationPathName, target, options);
    }

#if !UNITY_2018
    public static string DoBuild_String(string[] levels, string locationPathName, BuildTarget target, BuildOptions options = BuildOptions.None)
    {
        return BuildPipeline.BuildPlayer(levels, locationPathName, target, options);
    } 
#endif

#if UNITY_2018
    public static BuildReport DoBuild_BuildReport(string[] levels, string locationPathName, BuildTarget target, BuildOptions options = BuildOptions.None)
    {
        return BuildPipeline.BuildPlayer(levels, locationPathName, target, options);
    }
#endif

    private static void ProcessOptionalAndroidArguments()
    {
        if (FetchArgumentFlag(ARGS_ANDROID_ISGAME))
        {
            PlayerSettings.Android.androidIsGame = true;
            ConsoleWriteLine("PlayerSettings.Android.androidIsGame = {0}", true);
        }

        string oAndroidBundleVersionCode;
        if (FetchArgumentValue(ARGS_ANDROID_BUNDLEVERSIONCODE, out oAndroidBundleVersionCode))
        {
            int androidBundleVersionCode;
            if (int.TryParse(oAndroidBundleVersionCode, out androidBundleVersionCode))
            {
                PlayerSettings.Android.bundleVersionCode = androidBundleVersionCode;
                ConsoleWriteLine("PlayerSettings.Android.bundleVersionCode = {0}", androidBundleVersionCode);
            }
        }

        string oAndroidKeyAliasName;
        if (FetchArgumentValue(ARGS_ANDROID_KEYALIASNAME, out oAndroidKeyAliasName))
        {
            PlayerSettings.Android.keyaliasName = oAndroidKeyAliasName;
            ConsoleWriteLine("PlayerSettings.Android.keyaliasName = {0}", oAndroidKeyAliasName);
        }

        string oAndroidKeyAliasPass;
        if (FetchArgumentValue(ARGS_ANDROID_KEYALIASPASS, out oAndroidKeyAliasPass))
        {
            PlayerSettings.Android.keyaliasPass = oAndroidKeyAliasPass;
            ConsoleWriteLine("PlayerSettings.Android.keyaliasPass = {0}", oAndroidKeyAliasPass);
        }

        string oAndroidKeyStoreName;
        if (FetchArgumentValue(ARGS_ANDROID_KEYSTORENAME, out oAndroidKeyStoreName))
        {
            PlayerSettings.Android.keystoreName = oAndroidKeyStoreName;
            ConsoleWriteLine("PlayerSettings.Android.keystoreName = {0}", oAndroidKeyStoreName);
        }

        string oAndroidKeyStorePass;
        if (FetchArgumentValue(ARGS_ANDROID_KEYSTOREPASS, out oAndroidKeyStorePass))
        {
            PlayerSettings.Android.keystorePass = oAndroidKeyStorePass;
            ConsoleWriteLine("PlayerSettings.Android.keystorePass = {0}", oAndroidKeyStorePass);
        }

        if (FetchArgumentFlag(ARGS_ANDROID_USEAPKEXPANSIONFILES))
        {
            PlayerSettings.Android.useAPKExpansionFiles = true;
            ConsoleWriteLine("PlayerSettings.Android.useAPKExpansionFiles = {0}", true);
        }
    }

    private static void ProcessOptionaliOSArguments()
    {
        string oIosAppleDeveloperTeamId;
        if (FetchArgumentValue(ARGS_IOS_APPLEDEVELOPERTEAMID, out oIosAppleDeveloperTeamId))
        {
            PlayerSettings.iOS.appleDeveloperTeamID = oIosAppleDeveloperTeamId;
            ConsoleWriteLine("PlayerSettings.iOS.appleDeveloperTeamID = {0}", oIosAppleDeveloperTeamId);
        }

        if (FetchArgumentFlag(ARGS_IOS_APPLEENABLEDAUTOMATICSIGNING))
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            ConsoleWriteLine("PlayerSettings.iOS.appleEnableAutomaticSigning = {0}", true);
        }

        string oIosApplicationDisplayName;
        if (FetchArgumentValue(ARGS_IOS_APPLICATIONDISPLAYNAME, out oIosApplicationDisplayName))
        {
            PlayerSettings.iOS.applicationDisplayName = oIosApplicationDisplayName;
            ConsoleWriteLine("PlayerSettings.iOS.applicationDisplayName = {0}", oIosApplicationDisplayName);
        }

        string oIosBuildNumber;
        if (FetchArgumentValue(ARGS_IOS_BUILDNUMBER, out oIosBuildNumber))
        {
            PlayerSettings.iOS.buildNumber = oIosBuildNumber;
            ConsoleWriteLine("PlayerSettings.iOS.buildNumber = {0}", oIosBuildNumber);
        }

        string oIosManualProvisioningProfileID;
        if (FetchArgumentValue(ARGS_IOS_MANUALPROVISIONINGPROFILEID, out oIosManualProvisioningProfileID))
        {
            PlayerSettings.iOS.iOSManualProvisioningProfileID = oIosManualProvisioningProfileID;
            ConsoleWriteLine("PlayerSettings.iOS.iOSManualProvisioningProfileID = {0}", oIosManualProvisioningProfileID);
        }

#if UNITY_2018
        string oIosManualProvisioningProfileType;
        if (FetchArgumentValue(ARGS_IOS_MANUALPROVISIONINGPROFILETYPE, out oIosManualProvisioningProfileType))
        {
            ProvisioningProfileType provisioningProfileType = oIosManualProvisioningProfileType.ToEnum(ProvisioningProfileType.Automatic);
            PlayerSettings.iOS.iOSManualProvisioningProfileType = provisioningProfileType;
            ConsoleWriteLine("PlayerSettings.iOS.iOSManualProvisioningProfileID = {0}", provisioningProfileType.ToString());
        }
#endif
    }

    /// <summary>
    /// Gets all the included & enabled scenes in the build.
    /// Throws an exception if it is null or empty.
    /// </summary>
    /// <returns></returns>
    private static string[] GetEnabledScenes()
    {
        var enabledScenes = EditorBuildSettings.scenes
            .Where(x => x.enabled
            && !string.IsNullOrEmpty(x.path))
            .Select(x => x.path)
            .ToArray();

        if (enabledScenes == null || enabledScenes.Length == 0)
            throw new Exception("Found no scenes included in build.");

        return enabledScenes;
    }

    /// <summary>
    /// Gets the build target in the /ARGS_BUILD_TARGET/ args and performs additional checks and catches.
    /// Throws an exception if /ARGS_BUILD_TARGET/ was not used.
    /// </summary>
    /// <returns></returns>
    private static BuildTarget GetBuildTargetArgs()
    {
        var buildTargetString = GetArgument(ARGS_BUILD_TARGET);

        if (string.IsNullOrEmpty(buildTargetString))
            throw new ArgumentNullException(ARGS_BUILD_TARGET);

#if UNITY_2018 || UNITY_2017
#else
        /*
        * A bugfix that occurs in Unity versions 5.6 and lower.
        * Issue: https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
        */
        if (buildTargetString.ToLower() == "android")
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
#endif

        return ToEnum(buildTargetString, BuildTarget.NoTarget);
    }

    #endregion Methods

    #region MenuItem Methods

    private static void PerformBuild_Target(BuildTarget buildTarget)
    {
        UnityWriteLine("Performing build...");

        #region Prepare Required Values

        var fileName = CleanFileName(PlayerSettings.productName);
        var scenes = GetEnabledScenes();
        var buildPath = GenerateBuildPath(buildTarget, fileName);

        #endregion Prepare Required Values

        var trimTo = 10;

        var displayScenes = scenes.Length < trimTo
            ? string.Join(", ", scenes.Select(Path.GetFileNameWithoutExtension).ToArray())
            : string.Join(", ", scenes.Take(trimTo).Select(Path.GetFileNameWithoutExtension).ToArray()) + "... +" + (scenes.Length - 10) + " more.";

        var message = string.Format(
            "Scenes: {0}\n"
            + "Build Target: {1}\n"
            + "Build Path: {2}",
            displayScenes,
            buildTarget,
            buildPath);

        var proceed = UnityDisplayDialog("Perform Build", message, "Proceed", "Cancel");

        if (proceed)
            DoBuild(scenes, buildPath, buildTarget);

        UnityWriteLine("Done with build...");
    }

    [MenuItem("Framework/Build Automation/Android")]
    public static void PerformBuild_Android()
    {
        PerformBuild_Target(BuildTarget.Android);
    }

    [MenuItem("Framework/Build Automation/iOS")]
    public static void PerformBuild_iOS()
    {
        PerformBuild_Target(BuildTarget.iOS);
    }

    #endregion MenuItem Methods

    #region Tool Methods

    private static string CleanFileName(string fileName)
    {
        return Regex.Replace(fileName, "", "");
    }

    /// <summary>
    /// Generates a build path from the /buildTarget/ specified.
    /// Throws an exception if it is null or empty.
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string GenerateBuildPath(BuildTarget buildTarget, string fileName)
    {
        var modifier = string.Empty;
        var fileExtension = string.Empty;

        var buildPath = string.Empty;

        switch (buildTarget)
        {
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                modifier = "_Linux";
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneLinux: fileExtension = ".x86"; break;
                    case BuildTarget.StandaloneLinux64: fileExtension = ".x64"; break;
                    case BuildTarget.StandaloneLinuxUniversal: fileExtension = ".x86_64"; break;
                }
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX:
#else
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
#endif

                modifier = "_OSX";
                fileExtension = ".app";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                modifier = "_Windows";
                fileExtension = ".exe";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.Android:
                modifier = "_Android";
                fileExtension = ".apk";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.iOS:
                modifier = "_Xcode";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier;
                break;
        }

        if (string.IsNullOrEmpty(buildPath))
            throw new Exception("Unable to generate a build path.");

        return buildPath;
    }

    /// <summary>
    /// Fetches an Argument.
    /// </summary>
    /// <param name="argumentName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool FetchArgumentValue(string argumentName, out string value)
    {
        value = GetArgument(argumentName);

        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Fetches an Argument Flag.
    /// </summary>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    private static bool FetchArgumentFlag(string argumentName)
    {
        return GetArgumentFlag(argumentName);
    }

    /// <summary>
    /// Gets the value associated to the argument /name/ provided.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string GetArgument(string name)
    {
        var args = Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; i++)
            if (args[i].Contains(name))
                return args[i + 1];

        return null;
    }

    /// <summary>
    /// Gets the flag value associated to the argument /name/ provided.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool GetArgumentFlag(string name)
    {
        var args = Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; i++)
            if (args[i].Contains(name))
                return true;

        return false;
    }

    /// <summary>
    /// Gets the BuildTargetGroup associated to a BuildTarget.
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <returns></returns>
    private static BuildTargetGroup ToBuildTargetGroup(this BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return BuildTargetGroup.Android;

            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;

            case BuildTarget.N3DS:
                return BuildTargetGroup.N3DS;

            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;

            case BuildTarget.PSP2:
                return BuildTargetGroup.PSP2;

            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:

#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX:
#else
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
#endif
                return BuildTargetGroup.Standalone;

            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;

#if !UNITY_2017_3_OR_NEWER
            case BuildTarget.Tizen:
                return BuildTargetGroup.Tizen;
#endif

            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;

            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;

            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;

            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;

            default:
                return BuildTargetGroup.Unknown;
        }
    }

    /// <summary>
    /// Converts a string to its TEnum equivalent.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private static TEnum ToEnum<TEnum>(this string str, TEnum defaultValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), str))
            return defaultValue;

        return (TEnum)Enum.Parse(typeof(TEnum), str);
    }

    /// <summary>
    /// Writes to UnityEngine.Debug with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="str"></param>
    private static void UnityWriteLine(string str)
    {
        Debug.Log(str);
    }

    /// <summary>
    /// Writes to UnityEngine.Debug with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg"></param>
    private static void UnityWriteLine(string format, params object[] arg)
    {
        Debug.LogFormat(format, arg);
    }

    /// <summary>
    /// Displays a Unity Dialog.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="ok"></param>
    /// <param name="cancel"></param>
    private static bool UnityDisplayDialog(string title, string message, string ok, string cancel = "")
    {
        if (!string.IsNullOrEmpty(cancel))
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        else
        {
            EditorUtility.DisplayDialog(title, message, ok);
            return true;
        }
    }

    /// <summary>
    /// Writes to Console with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="str"></param>
    private static void ConsoleWriteLine(string str)
    {
        Console.WriteLine(CONSOLE_FORMATTER, str);
    }

    /// <summary>
    /// Writes to Console with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg"></param>
    private static void ConsoleWriteLine(string format, params object[] arg)
    {
        Console.WriteLine(CONSOLE_FORMATTER, string.Format(format, arg));
    }

    #endregion Tool Methods
}