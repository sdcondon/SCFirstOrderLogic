using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether all pairs of the Cartesian product of a sequence with itself satisfy a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element pair for a condition.</param>
        /// <returns>
        /// true if every pair of the Cartesian product passes the test in the specified predicate, or if the sequence is empty; otherwise, false.
        /// </returns>
        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> predicate)
        {
            return Enumerable.All(source, x => Enumerable.All(source, y => predicate(x, y)));
        }

        /// <summary>
        /// Determines whether all triples of the double Cartesian product of a sequence with itself satisfy a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element triple for a condition.</param>
        /// <returns>
        /// true if every triple of the double Cartesian product passes the test in the specified predicate, or if the sequence is empty; otherwise, false.
        /// </returns>
        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource, bool> predicate)
        {
            return Enumerable.All(source, x => Enumerable.All(source, y => Enumerable.All(source, z => predicate(x, y, z))));
        }

        /// <summary>
        /// Determines whether any pairs of the Cartesian product of a sequence with itself satisfy a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element pair for a condition.</param>
        /// <returns>
        /// true if any pair of the Cartesian product passes the test in the specified predicate; otherwise, false.
        /// </returns>
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> predicate)
        {
            return Enumerable.Any(source, x => Enumerable.Any(source, y => predicate(x, y)));
        }

        /// <summary>
        /// Determines whether any triples of the double Cartesian product of a sequence with itself satisfy a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element triple for a condition.</param>
        /// <returns>
        /// true if any triple of the double Cartesian product passes the test in the specified predicate; otherwise, false.
        /// </returns>
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource, bool> predicate)
        {
            return Enumerable.Any(source, x => Enumerable.Any(source, y => Enumerable.Any(source, z => predicate(x, y, z))));
        }
    }
}
