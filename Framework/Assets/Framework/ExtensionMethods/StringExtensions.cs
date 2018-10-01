using Framework.Core;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UColor = UnityEngine.Color;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of methods for String.
    /// </summary>
    public static class StringExtensions
    {
        #region Constants

        public const string REGEXPATTERN_EMAILADDRESS = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        #endregion Constants

        #region Static Methods

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string str, string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Indicates whether the specified string is NOT null or an string.Empty string.
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string sVal)
        {
            return !string.IsNullOrEmpty(sVal);
        }

        /// <summary>
        /// Indicates whether the specified string is null or an string.Empty string.
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string sVal)
        {
            return string.IsNullOrEmpty(sVal);
        }

        /// <summary>
        /// Returns true if the specified string is a valid email address.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, REGEXPATTERN_EMAILADDRESS, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns true if the specified string is a valid IPv4 Address.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidIPvAddress(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            IPAddress ipAddress;
            return IPAddress.TryParse(str, out ipAddress) && ipAddress.AddressFamily.Equals(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Returns true if the specified string is a valid IPv6 Address.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidIPv6Address(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            IPAddress ipAddress;
            return IPAddress.TryParse(str, out ipAddress) && ipAddress.AddressFamily.Equals(AddressFamily.InterNetworkV6);
        }

        /// <summary>
        /// Returns true if the specified string is a valid http or https url.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            Uri uri;
            return Uri.TryCreate(str, UriKind.Absolute, out uri) && (uri.Scheme.Equals(Uri.UriSchemeHttp) || uri.Scheme.Equals(Uri.UriSchemeHttps));
        }

        /// <summary>
        /// Gets the Levenshtein Distance between string and /value/.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(this string str, string value)
        {
            return FMath.LevenshteinDistance(str, value);
        }

        /// <summary>
        /// Parses string to decimal, returns /defaultValue/ when parsing fails.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ParseToDecimalOrDefault(this string str, decimal defaultValue = default(decimal))
        {
            decimal value;

            return (decimal.TryParse(str, out value)) ? value : defaultValue;
        }

        /// <summary>
        /// Parses string to double, returns /defaultValue/ when parsing fails.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ParseToDoubleOrDefault(this string str, double defaultValue = default(double))
        {
            double value;

            return (double.TryParse(str, out value)) ? value : defaultValue;
        }

        /// <summary>
        /// Parses string to float, returns /defaultValue/ when parsing fails.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ParseToFloatOrDefault(this string str, float defaultValue = default(float))
        {
            float value;

            return (float.TryParse(str, out value)) ? value : defaultValue;
        }

        /// <summary>
        /// Parses string to int, returns /defaultValue/ when parsing fails.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ParseToIntOrDefault(this string str, int defaultValue = default(int))
        {
            int value;

            return (int.TryParse(str, out value)) ? value : defaultValue;
        }

        /// <summary>
        /// Parses string to long, returns /defaultValue/ when parsing fails.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ParseToLongOrDefault(this string str, long defaultValue = default(long))
        {
            long value;

            return (long.TryParse(str, out value)) ? value : defaultValue;
        }

        /// <summary>
        /// Formats a string with Markup '&lt;b&gt;/str/&lt;b&gt;'.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RichTextBolden(this string str)
        {
            return string.Format("<b>{0}</b>", str);
        }

        /// <summary>
        /// Formats a string with Markup '&lt;color=/color/&gt;/str/&lt;/color&gt;'.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string RichTextColorize(this string str, UColor color)
        {
            return string.Format("<color={0}>{1}</color>", color.ToHex(), str);
        }

        /// <summary>
        /// Formats a string with Markup '&lt;i&gt;/str/&lt;i&gt;'.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RichTextItalicize(this string str)
        {
            return string.Format("<i>{0}</i>", str);
        }

        /// <summary>
        /// Formats a string with Markup '&lt;size=/size/&gt;/str/&lt;/size&gt;'.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string RichTextResize(this string str, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, str);
        }

        /// <summary>
        /// Converts specified string to boolean.
        /// Returns true if string is equal to either "1", "true", or "yes", returns false otherwise.
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string sVal)
        {
            if (sVal.IsNullOrEmpty())
                return false;

            string sValLower = sVal.ToLower();

            return (sValLower == "1") || (sValLower == "true") || (sValLower == "yes");
        }

        /// <summary>
        /// Strictly converts specified string to boolean.
        /// <para/>Returns true if string is equal to either "1", "true", or "yes".
        /// <para/>Returns false if string is equal to either "0", "false", or "no".
        /// <para/>Returns defaultValue otherwise.
        /// </summary>
        /// <param name="sVal"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBooleanStrict(this string sVal, bool defaultValue)
        {
            string sValLower = sVal.ToLower();

            if (sValLower == "0")
                return false;

            if (sValLower == "1")
                return true;

            if (sValLower == "false")
                return false;

            if (sValLower == "true")
                return true;

            if (sValLower == "no")
                return false;

            if (sValLower == "yes")
                return true;

            return defaultValue;
        }

        /// <summary>
        /// Converts string to enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str) where T : struct
        {
            return (T)Enum.Parse(typeof(T), str, true);
        }

        /// <summary>
        /// Converts the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Creates a new Uri from a string.
        /// </summary>
        /// <param name="sVal"></param>
        /// <param name="uriKind"></param>
        /// <returns></returns>
        public static Uri ToUri(this string sVal, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            return new Uri(sVal, uriKind);
        }

        /// <summary>
        /// Shortens string down to /length/.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="addEllipsis"></param>
        /// <param name="ellipsis"></param>
        /// <returns></returns>
        public static string Truncate(this string str, int length, bool addEllipsis = true, string ellipsis = "...")
        {
            if (length <= 0)
                throw new ArgumentException("Cannot be less than or equal to zero.", "length");

            if (string.IsNullOrEmpty(str) || str.Length <= length)
                return str;

            if (addEllipsis && length <= ellipsis.Length)
                addEllipsis = false;

            string truncated = str.Substring(0, Mathf.Clamp((length - (addEllipsis ? ellipsis.Length : 0)), 0, str.Length)) + (addEllipsis ? ellipsis : string.Empty);

            return truncated;
        }

        #endregion Static Methods
    }
}