using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class FOLSentence<TElement>
    {
        ////protected FOLSentence(Expression<Predicate<IEnumerable<TModel>>> lambda) => Lambda = lambda;

        // TODO-FEATURE: Ultimately might be useful for verification - but can add this later..
        ////public Expression<Predicate<IEnumerable<TModel>>> Lambda { get; }

        /// <summary>
        /// Tries to create the <see cref="FOLSentence{TModel}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            // TODO-USABILITY: might be nice to return more info than just "false" - but can't rely on exceptions due to the way this works.
            return FOLComplexSentence<TElement>.TryCreate(lambda, out sentence)
                || FOLAtomicSentence<TElement>.TryCreate(lambda, out sentence);
        }
    }
}
