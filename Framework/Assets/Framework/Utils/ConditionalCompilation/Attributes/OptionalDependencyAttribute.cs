#if UNITY_EDITOR

using System;
using System.Diagnostics;

/// <summary>
/// This is necessary for CCU to pick up the right attributes.
/// Must derive from System.Attribute.
/// </summary>
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

#endif