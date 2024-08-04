// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing
{
    internal static class IComparerExtensions
    {
        public static T[] Sort<T>(this IComparer<T> comparer, IEnumerable<T> set)
        {
            var elements = set.ToArray();
            Array.Sort(elements, comparer);

            return elements;
        }

        public static T[] SortAndValidateUnambiguousOrdering<T>(this IComparer<T> comparer, IEnumerable<T> key)
        {
            var keyElements = key.ToArray();
            Array.Sort(keyElements, comparer);

            if (HasComparisonsOfZero(comparer, key))
            {
                throw new ArgumentException(
                    "Key contains at least one element pair for which the element comparer gives a comparison of zero. " +
                    "Either this pair are duplicates (meaning the passed value is not a valid set), or the element comparer is unsuitable for use by a set trie.",
                    nameof(key));
            }

            return keyElements;
        }

        private static bool HasComparisonsOfZero<T>(IComparer<T> comparer, IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return false;
            }

            var lastElement = enumerator.Current;
            while (enumerator.MoveNext())
            {
                if (comparer.Compare(lastElement, enumerator.Current) == 0)
                {
                    return true;
                }

                lastElement = enumerator.Current;
            }

            return false;
        }
    }
}
