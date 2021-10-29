using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a quantification sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Quantification<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        protected Quantification(VariableDeclaration<TDomain, TElement> variable, Sentence<TDomain, TElement> sentence) => (Variable, Sentence) = (variable, sentence);

        /// <summary>
        /// Gets the variable declared by this quantification.
        /// </summary>
        public VariableDeclaration<TDomain, TElement> Variable { get; }

        /// <summary>
        /// Gets the sentence that this quantification applies to.
        /// </summary>
        public Sentence<TDomain, TElement> Sentence { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            // TODO-BUG: universal and existential quantification of same var on same sentence considered equal...
            return obj is Quantification<TDomain, TElement> quantification
                && Variable.Equals(quantification.Variable)
                && Sentence.Equals(quantification.Sentence);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Variable, Sentence);
    }
}
