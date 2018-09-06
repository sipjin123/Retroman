using System.Collections.Generic;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of methods for ICollection&lt;T&gt;.
    /// </summary>
    public static class ICollectionExtensions
    {
        #region Static Methods

        /// <summary>
        /// Returns true if an ICollection{T} is NOT null or empty, returns false otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cVal"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this ICollection<T> cVal)
        {
            return cVal != null && cVal.Count > 0;
        }

        /// <summary>
        /// Returns true if an ICollection{T} is null or empty, returns false otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cVal"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> cVal)
        {
            return cVal == null || cVal.Count == 0;
        }

        #endregion Static Methods
    }
}