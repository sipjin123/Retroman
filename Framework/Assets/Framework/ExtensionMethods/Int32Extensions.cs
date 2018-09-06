using System;
using System.Linq;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of methods for Int32.
    /// </summary>
    public static class Int32Extensions
    {
        #region Static Methods

        /// <summary>
        /// Converts the specified int into an ordinal string.
        /// <para>
        /// 1 to "1st", 2 to "2nd", 11 to "11th", 21 to "21st" etc.
        /// </para>
        /// </summary>
        /// <param name="iVal"></param>
        /// <returns></returns>
        public static string ToOrdinalString(this int iVal)
        {
            string extension = "th";

            int lastDigits = iVal % 100;

            if (lastDigits < 11 || lastDigits > 13)
                switch (lastDigits % 10)
                {
                    case 1:
                        extension = "st";
                        break;

                    case 2:
                        extension = "nd";
                        break;

                    case 3:
                        extension = "rd";
                        break;
                }

            return iVal.ToString() + extension;
        }

        /// <summary>
        /// Converts the specified int into a version string.
        /// 1 to "1.0.0", 12 to "1.2.0", -143 to "1.4.3", 1337 to "1.3.3.7", etc.
        /// </summary>
        /// <param name="iVal"></param>
        /// <param name="minimumCharacters"></param>
        /// <returns></returns>
        public static string ToVersionString(this int iVal, int minimumCharacters = 3)
        {
            iVal = Math.Abs(iVal);

            char[] characters = iVal.ToString().ToCharArray();

            if (characters.Length < minimumCharacters)
                characters = characters.ToList().Concat(Enumerable.Repeat('0', minimumCharacters - characters.Length)).ToArray();

            return string.Join(".", characters.Select(x => x.ToString()).ToArray());
        }

        #endregion Static Methods
    }
}