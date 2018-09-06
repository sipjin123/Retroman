using UnityEngine;

namespace Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class EnumerableExtensions
    {
        public static readonly Random RANDOM = new Random();

        public static T RandomFromSource<T>(IEnumerable<T> source)
        {
            return source.Random();
        }

        public static IEnumerable<T> ShuffleFromSource<T>(IEnumerable<T> source)
        {
            return source.Shuffle();
        }

        public static T Random<T>(this IEnumerable<T> source)
        {
            return source.Random(1).Single();
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static void ReplaceOrAdd<T>(this List<T> source, T item, Predicate<T> exists)
        {
            int count = source.Count();
            int index = -1;
            for (int i = 0; i < count; i++)
            {
                if (exists(source[i]))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                source[index] = item;
            }
            else
            {
                source.Add(item);
            }
        }

        public static List<N> ToList<O, N>(this List<O> source) where O : MonoBehaviour
        {
            List<O> filter = source.FindAll(s => s.GetComponent<N>() != null);
            List<N> filtered = new List<N>(filter.Count);
            filter.ForEach(f => filtered.Add(f.GetComponent<N>()));

            return filtered;
        }

        public static int GetRowLength<T>(this T[,] array)
        {
            int rows = array.GetUpperBound(1) + 1;
            return rows;
        }

        public static int GetColLength<T>(this T[,] array)
        {
            int col = array.GetUpperBound(0) + 1;
            return col;
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            int cols = array.GetColLength<T>();
            T[] result = new T[cols];
            int size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }

        public static T Random<T>(this T[,] array)
        {
            int row = RANDOM.Next(array.GetRowLength());
            int col = RANDOM.Next(array.GetColLength());
            
            return array[col, row];
        }

        public static T Random<T>(this T[] array)
        {
            int index = RANDOM.Next(array.Length);
            return array[index];
        }
    }
}
