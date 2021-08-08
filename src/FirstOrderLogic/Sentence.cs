using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    public static class Sentence
    {
        /// <summary>
        /// Tries to create the <see cref="Sentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        ///  <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate<TDomain, TElement>(Expression<Predicate<TDomain>> lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            return Sentence<TDomain, TElement>.TryCreate(lambda, out sentence);
        }

        /// <summary>
        /// Tries to create the <see cref="FOLSentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        /// <remarks>
        /// This method serves as a shorthand for <see cref="TryCreate{TDomain, TElement}"/> where the domain is
        /// just <see cref="IEnumerable{TElement}"/> - which suffices when the domain contains no constants or ground
        /// predicates.
        /// </remarks>
        public static bool TryCreate<TElement>(Expression<Predicate<IEnumerable<TElement>>> lambda, out Sentence<IEnumerable<TElement>, TElement> sentence)
        {
            return Sentence<IEnumerable<TElement>, TElement>.TryCreate(lambda, out sentence);
        }
    }
}
