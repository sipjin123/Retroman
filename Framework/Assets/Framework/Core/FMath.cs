using System;
using System.Linq;
using UnityEngine;

namespace Framework.Core
{
    /// <summary>
    /// A collection of math functions.
    /// </summary>
    public static class FMath
    {
        #region Static Methods

        /// <summary>
        /// Returns true if the difference between /a/ and /b/ is less than an epsilon thereby considered almost equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualTo(float a, float b)
        {
            return Mathf.Abs(a - b) < Mathf.Epsilon;
        }

        /// <summary>
        /// Returns true if float value /a/ is greater than or almost equal to float value /b/.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool GreaterThanOrEqualTo(float a, float b)
        {
            return (a > b) || EqualTo(a, b);
        }

        /// <summary>
        /// Returns true if float value /a/ is less than or almost equal to float value /b/.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool LessThanOrEqualTo(float a, float b)
        {
            return (a < b) || EqualTo(a, b);
        }

        /// <summary>
        /// Returns the number of combinations where /n/ is the number of things to choose from, /r/ is the number of things we choose among them, and the order does not matter.
        /// </summary>
        /// <param name="n">Number of things to choose from.</param>
        /// <param name="r">Number of things chosen.</param>
        /// <param name="includeRepetitions">Include repetitions</param>
        /// <returns></returns>
        public static int Combinations(int n, int r, bool includeRepetitions = false)
        {
            if (includeRepetitions)
                return Factorial(r + n - 1) / ((Factorial(r)) * (Factorial(n - 1)));
            else
                return Factorial(n) / ((Factorial(r)) * (Factorial(n - r)));
        }

        /// <summary>
        /// Returns the factorial of /i/.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int Factorial(int i)
        {
            if (i <= 1)
                return 1;
            else if (i == 2)
                return 2;
            else
                return i * Factorial(i - 1);
        }

        /// <summary>
        /// Returns /a/ mod /b/.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Modulo(int a, int b)
        {
            if (b == 0)
                throw new ArgumentOutOfRangeException("b", 0, "Cannot be zero.");

            return a - (Mathf.FloorToInt((float)a / b) * b);
        }

        /// <summary>
        /// Returns /a/ mod /b/.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Modulo(float a, float b)
        {
            if (EqualTo(b, 0))
                throw new ArgumentOutOfRangeException("b", b, "Cannot be zero.");

            return a - (UnityEngine.Mathf.Floor(a / b) * b);
        }

        /// <summary>
        /// Returns the number of permutations where /n/ is the number of things to choose from, /r/ is the number of things we choose among them, and the order matters.
        /// </summary>
        /// <param name="n">Number of things to choose from.</param>
        /// <param name="r">Number of things chosen.</param>
        /// <param name="includeRepetitions">Include repetitions</param>
        /// <returns></returns>
        public static int Permutations(int n, int r, bool includeRepetitions = false)
        {
            if (includeRepetitions)
                return Mathf.FloorToInt(Mathf.Pow(n, r));
            else
                return Factorial(n) / (Factorial(n - r));
        }

        /// <summary>
        /// Wraps integer value /i/ between /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Wrap(int i, int min, int max)
        {
            if (min > max)
                throw new ArgumentException("min" + " is greater than " + "max" + ".", "min");

            if (min == max)
                return min;

            return Modulo((i - min), max - min + 1) + min;
        }

        /// <summary>
        /// Wraps float value /f/ between /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Wrap(float f, float min, float max)
        {
            if (min > max)
                throw new ArgumentException("min" + " is greater than " + "max" + ".", "min");

            if (EqualTo(min, max))
                return min;

            return Modulo((f - min), max - min + 1.0f) + min;
        }

        /// <summary>
        /// Gets the Levenshtein Distance between /str/ and /value/.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(string str, string value)
        {
            int n = str.Length;
            int m = value.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (value[j - 1] == str[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1,
                                                d[i, j - 1] + 1),
                                       d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        /// <summary>
        /// Returns the farthest point from origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3 FarthestPoint(Vector3 origin, params Vector3[] points)
        {
            if (points == null)
                throw new ArgumentNullException("Cannot be null.", "points");

            if (points.Length == 0)
                throw new ArgumentException("Cannot be empty.", "points");

            return points.OrderByDescending(x => Vector3.Distance(origin, x)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the nearest point from origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3 NearestPoint(Vector3 origin, params Vector3[] points)
        {
            if (points == null)
                throw new ArgumentNullException("Cannot be null.", "points");

            if (points.Length == 0)
                throw new ArgumentException("Cannot be empty.", "points");

            return points.OrderBy(x => Vector3.Distance(origin, x)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the nearest point along a line starting from the origin based on the given value.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static Vector3 NearestPointOnALine(Vector3 start, Vector3 end, Vector3 point, bool strict = false)
        {
            Vector3 line = end - start;
            Vector3 vectorDirection = Vector3.Normalize(line);
            float vectorScalar = Vector3.Dot((point - start), vectorDirection) / Vector3.Dot(vectorDirection, vectorDirection);

            if (strict)
                return start + (Mathf.Clamp(vectorScalar, 0.0f, Vector3.Magnitude(line)) * vectorDirection);
            else
                return start + (vectorScalar * vectorDirection);
        }

        #endregion Static Methods
    }
}