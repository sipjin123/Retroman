using UnityEngine;

namespace Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class EnumerableExtensions
    {
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

        public static List<N> ToList<O, N>(this List<O> source) where O : MonoBehaviour
        {
            List<O> filter = source.FindAll(s => s.GetComponent<N>() != null);
            List<N> filtered = new List<N>(filter.Count);
            filter.ForEach(f => filtered.Add(f.GetComponent<N>()));

            return filtered;
        }
    }
}
