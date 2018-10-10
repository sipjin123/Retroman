using System;
using System.Collections.Generic;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of methods for IEnumerable&lt;T&gt;.
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Static Methods

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (var e in enumerable)
            {
                action(e);
            }
        }

        #endregion Static Methods
    }
}