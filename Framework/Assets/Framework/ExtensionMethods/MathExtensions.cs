using Framework.Core;
using UnityEngine;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// A collection of extension methods related to math operations.
    /// </summary>
    public static class EMMath
    {
        #region Static Methods

        /// <summary>
        /// Returns true if the difference between this float and /value/ is less than an epsilon thereby considered almost equal.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualTo(this float f, float value)
        {
            return FMath.EqualTo(f, value);
        }

        /// <summary>
        /// Returns true if this float is greater than or almost equal to /value/.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool GreaterThanOrEqualTo(this float f, float value)
        {
            return FMath.GreaterThanOrEqualTo(f, value);
        }

        /// <summary>
        /// Returns true if this float is less than or almost equal to /value/.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool LessThanOrEqualTo(this float f, float value)
        {
            return FMath.LessThanOrEqualTo(f, value);
        }

        /// <summary>
        /// Is value an even number.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsEven(int i)
        {
            return Mathf.Abs(i % 2) == 0;
        }

        /// <summary>
        /// Is value an odd number.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsOdd(int i)
        {
            return Mathf.Abs(i % 2) != 0;
        }

        /// <summary>
        /// Is value a perfect square.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsPerfectSquare(this int i)
        {
            if (i < 0)
                return false;

            return (Mathf.Sqrt(i) % 1).EqualTo(0.0f);
        }

        /// <summary>
        /// Is value inside /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange(this int i, int min, int max)
        {
            return (i >= min) && (i <= max);
        }

        /// <summary>
        /// Is value inside /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange(this float f, float min, float max)
        {
            return f.GreaterThanOrEqualTo(min) && f.LessThanOrEqualTo(max);
        }

        /// <summary>
        /// Clamps value between /min/ [inclusive] and /max/ [inclusive] and returns value.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(this int i, int min, int max)
        {
            return Mathf.Clamp(i, min, max);
        }

        /// <summary>
        /// Clamps value between /min/ [inclusive] and /max/ [inclusive] and returns value.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(this float f, float min, float max)
        {
            return Mathf.Clamp(f, min, max);
        }

        /// <summary>
        /// Returns the factorial value.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int Factorial(this int i)
        {
            return FMath.Factorial(i);
        }

        /// <summary>
        /// Returns the value mod /divisor/.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int Modulo(this int i, int divisor)
        {
            return FMath.Modulo(i, divisor);
        }

        /// <summary>
        /// Returns the value mod /divisor/.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static float Modulo(this float f, float divisor)
        {
            return FMath.Modulo(f, divisor);
        }

        /// <summary>
        /// Wraps value between /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Wrap(this int i, int min, int max)
        {
            return FMath.Wrap(i, min, max);
        }

        /// <summary>
        /// Wraps value between /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Wrap(this float f, float min, float max)
        {
            return FMath.Wrap(f, min, max);
        }

        /// <summary>
        /// Rounds value to a specified number of fractional digits.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static float RoundToNearest(this float f, int decimals)
        {
            return (float)System.Math.Round(f, decimals);
        }

        /// <summary>
        /// Returns the farthest point from value.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3 FarthestPoint(this Vector3 v, params Vector3[] points)
        {
            return FMath.FarthestPoint(v, points);
        }

        /// <summary>
        /// Returns the nearest point from origin.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3 NearestPoint(this Vector3 v, params Vector3[] points)
        {
            return FMath.NearestPoint(v, points);
        }

        #endregion Static Methods
    }
}