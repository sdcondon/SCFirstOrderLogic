using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a quantification sentence of first order logic.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class FOLQuantification<TElement> : FOLComplexSentence<TElement>
    {
        protected FOLQuantification(FOLVariableTerm<TElement> variable, FOLSentence<TElement> sentence) => (Variable, Sentence) = (variable, sentence);

        /// <summary>
        /// Gets the variable declared by this quantification.
        /// </summary>
        public FOLVariableTerm<TElement> Variable { get; }

        /// <summary>
        /// Gets the sentence that this quantification applies to.
        /// </summary>
        public FOLSentence<TElement> Sentence { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            return FOLUniversalQuantification<TElement>.TryCreate(lambda, out sentence)
                || FOLExistentialQuantification<TElement>.TryCreate(lambda, out sentence);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is FOLQuantification<TElement> quantification
                && Variable.Equals(quantification.Variable)
                && Sentence.Equals(quantification.Sentence);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Variable, Sentence);
    }
}
