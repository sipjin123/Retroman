namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of methods for Boolean.
    /// </summary>
    public static class BooleanExtensions
    {
        #region Constants

        private const string No = "No";

        private const string Yes = "Yes";

        #endregion Constants

        #region Static Methods

        /// <summary>
        /// Converts the specified bool into an int.
        /// <para/>Returns 1 if true.
        /// <para/>Returns 0 if false.
        /// </summary>
        /// <param name="bVal"></param>
        /// <returns></returns>
        public static int ToInt32(this bool bVal)
        {
            return bVal ? 1 : 0;
        }

        /// <summary>
        /// Converts the specified bool into a "Yes" or "No" string.
        /// </summary>
        /// <param name="bVal"></param>
        /// <returns></returns>
        public static string ToYesNoString(this bool bVal)
        {
            return bVal ? Yes : No;
        }

        #endregion Static Methods
    }
}