using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a material implication sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇒ {sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Symbols.If({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Symbols;</c> to make this a little shorter)
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Implication<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Implication{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence.</param>
        /// <param name="consequent">The consequent sentence.</param>
        public Implication(Sentence<TDomain, TElement> antecedent, Sentence<TDomain, TElement> consequent) => (Antecedent, Consequent) = (antecedent, consequent);

        /// <summary>
        /// Gets the antecedent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Antecedent { get; }

        /// <summary>
        /// Gets the consequent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Consequent { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Implication<TDomain, TElement> otherImplication && Antecedent.Equals(otherImplication.Antecedent);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
    }
}
