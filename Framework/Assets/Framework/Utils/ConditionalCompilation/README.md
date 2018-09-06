# Conditional Compilation Utility

[Original Source: Unity-Technologies/EditorXR](https://github.com/Unity-Technologies/EditorXR/blob/development/Scripts/Utilities/Editor/ConditionalCompilationUtility.cs)

The Conditional Compilation Utility (CCU) will add defines to the build settings once dependendent classes have been detected.
In order for this to be specified in any project without the project needing to include the CCU, at least one custom attribute must be created in the following form:

```C#
[Conditional("UNITY_CCU")]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class OptionalDependencyAttribute : Attribute
{
    #region Ctor

    public OptionalDependencyAttribute(string dependentClass, string define)
    {
        this.dependentClass = dependentClass;
        this.define = define;
    }

    #endregion Ctor

    #region Fields

    /// <summary>
    /// Required field specifying the fully qualified dependent class.
    /// </summary>
    public string dependentClass;

    /// <summary>
    /// Required field specifying the define to add.
    /// </summary>
    public string define;

    #endregion Fields
}
```

Then, simply specify the assembly attribute(s) you created.

```C#
[assembly: OptionalDependency("UnityEngine.InputNew.InputSystem", "USING_NEWINPUT")]
[assembly: OptionalDependency("Valve.VR.IVRSystem", "USING_STEAMVRINPUT")]
```