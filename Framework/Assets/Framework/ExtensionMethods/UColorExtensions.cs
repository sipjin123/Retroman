using System.Globalization;
using UnityEngine;
using UColor = UnityEngine.Color;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods for UnityEngine.Color.
	/// </summary>
    public static class UColorExtensions
    {
        #region Static Methods

        /// <summary>
        /// Converts color to hex string.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHex(this UColor color)
        {
            Color32 color32 = color;

            return ("#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2") + color32.a.ToString("X2"));
        }

        /// <summary>
        /// Converts hex string to color.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UColor ToColor(this string str)
        {
            str = str.TrimStart('#');

            int[] c = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < 8; i++)
                c[i] = (i < str.Length && int.TryParse(str[i].ToString(), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out c[i])) ? (c[i]) : (15);

            float r = Mathf.Clamp01(((c[0] * 16.0f) + c[1]) / 255.0f);
            float g = Mathf.Clamp01(((c[2] * 16.0f) + c[3]) / 255.0f);
            float b = Mathf.Clamp01(((c[4] * 16.0f) + c[5]) / 255.0f);
            float a = Mathf.Clamp01(((c[6] * 16.0f) + c[7]) / 255.0f);

            return new UColor(r, g, b, a);
        }

        /// <summary>
        /// Shorthand method for 'new Color(color.r, color.g, color.b, Mathf.Clamp01(/alpha/ / 255.0f))'.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static UColor WithAlpha(this UColor color, int alpha)
        {
            return new UColor(color.r, color.g, color.b, Mathf.Clamp01(alpha / 255.0f));
        }

        /// <summary>
        /// Shorthand method for 'new Color(color.r, color.g, color.b, Mathf.Clamp01(/alpha/))'.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static UColor WithAlpha(this UColor color, float alpha)
        {
            return new UColor(color.r, color.g, color.b, Mathf.Clamp01(alpha));
        }

        #endregion Static Methods
    }
}