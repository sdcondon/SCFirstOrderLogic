﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a complex sentence of first order logic - that is, one that is composed of one or more sub-sentences.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    /// <remarks>
    /// NB: This is a largely pointless mid-level abstract class because it adds no members - it exists only because the BNF representation
    /// I'm working from makes the distinction between complex and atomic sentences - which to be fair is useful distinction to make when learning
    /// FOL.
    /// </remarks>
    public abstract class FOLComplexSentence<TElement> : FOLSentence<TElement>
    {
        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            return FOLNegation<TElement>.TryCreate(lambda, out sentence)
                || FOLConjunction<TElement>.TryCreate(lambda, out sentence)
                || FOLDisjunction<TElement>.TryCreate(lambda, out sentence)
                || FOLQuantification<TElement>.TryCreate(lambda, out sentence);
        }
    }
}
