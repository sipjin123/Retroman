using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Core
{
    /// <summary>
    /// A helper class for generating random data.
    /// </summary>
    public static class FRandom
    {
        #region Static Methods

        /// <summary>
        /// Randomizes an integer number between zero [inclusive] and /space/ [inclusive]. Returns true if it is less than or equal to /factor/.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static bool Chance(int factor, int space)
        {
            if (space < 0)
                throw new ArgumentException("Cannot less than zero.", "space");

            if (factor > space)
                throw new ArgumentException("factor" + " is greater than " + "space" + ".", "factor");

            return UnityEngine.Random.Range(0, space + 1) <= factor ? true : false;
        }

        /// <summary>
        /// Randomizes a float number between zero [inclusive] and /space/ [inclusive]. Returns true if it is less than or equal to /factor/.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static bool Chance(float factor, float space)
        {
            if (space < 0)
                throw new ArgumentException("Cannot less than zero.", "space");

            if (factor > space)
                throw new ArgumentException("factor" + " is greater than " + "space" + ".", "factor");

            return FMath.LessThanOrEqualTo(UnityEngine.Random.Range(0.0f, space), factor) ? true : false;
        }

        /// <summary>
        /// Randomizes between true or false.
        /// </summary>
        /// <returns></returns>
        public static bool SplitChance()
        {
            return UnityEngine.Random.Range(0, 2) <= 0 ? true : false;
        }

        /// <summary>
        /// Returns a random integer number between /min/ [inclusive] and /max/ [exclusive].
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            if (min > max)
                throw new ArgumentException("min" + " is greater than " + "max" + ".", "min");

            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// Returns a random float number between /min/ [inclusive] and /max/ [inclusive].
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            if (min > max)
                throw new ArgumentException("min" + " is greater than " + "max" + ".", "min");

            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// Randomizes an object of T from an array of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Choice<T>(params T[] array)
        {
            if (array == null)
                throw new ArgumentNullException("Cannot be null.", "array");

            if (array.Length <= 0)
                throw new ArgumentException("Cannot be empty.", "array");

            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Randomizes an object of T from an array of weighted T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="choices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(T[] choices, int[] weights)
        {
            if (choices == null)
                throw new ArgumentNullException("Cannot be null.", "choices");

            if (weights == null)
                throw new ArgumentNullException("Cannot be null.", "weights");

            if (choices.Length <= 0)
                throw new ArgumentException("Cannot be empty.", "choices");

            if (weights.Length <= 0)
                throw new ArgumentException("Cannot be empty.", "weights");

            if (choices.Length < weights.Length)
                throw new ArgumentException("choices" + " length is less than " + "weights" + " length.", "choices");

            if (choices.Length > weights.Length)
            {
                List<int> newWeights = new List<int>();
                newWeights.AddRange(weights);

                for (int i = weights.Length; i < choices.Length; i++)
                    newWeights.Add(0);

                weights = newWeights.ToArray();
            }

            int totalWeight = weights.Sum();
            int value = UnityEngine.Random.Range(0, totalWeight);

            for (int i = 0; i < choices.Length; i++)
            {
                if (value < weights[i])
                    return choices[i];

                value -= weights[i];
            }

            return choices[choices.Length - 1];
        }

        /// <summary>
        /// Randomizes an object of T from an array of weighted T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="choices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedChoice<T>(T[] choices, float[] weights)
        {
            if (choices == null)
                throw new ArgumentNullException("Cannot be null.", "choices");

            if (weights == null)
                throw new ArgumentNullException("Cannot be null.", "weights");

            if (choices.Length <= 0)
                throw new ArgumentException("Cannot be null.", "choices");

            if (weights.Length <= 0)
                throw new ArgumentException("Cannot be null.", "weights");

            if (choices.Length < weights.Length)
                throw new ArgumentException("choices" + " length is less than " + "weights" + " length.", "choices");

            if (choices.Length > weights.Length)
            {
                List<float> newWeights = new List<float>();
                newWeights.AddRange(weights);

                for (int i = weights.Length; i < choices.Length; i++)
                    newWeights.Add(0.0f);

                weights = newWeights.ToArray();
            }

            float totalWeight = weights.Sum();
            float value = UnityEngine.Random.Range(0.0f, totalWeight);

            for (int i = 0; i < choices.Length; i++)
            {
                if (value < weights[i])
                    return choices[i];

                value -= weights[i];
            }

            return choices[choices.Length - 1];
        }

        #endregion Static Methods
    }
}