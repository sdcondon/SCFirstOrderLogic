using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a material equivalence sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇔ {sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Operators.Iff({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Operators;</c> to make this a little shorter)
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Equivalence<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLImplication{TModel}"/> class.
        /// </summary>
        /// <param name="equivalent1">The first equivalent sentence.</param>
        /// <param name="equivalent2">The second equivalent sentence.</param>
        public Equivalence(Sentence<TDomain, TElement> equivalent1, Sentence<TDomain, TElement> equivalent2) => (Equivalent1, Equivalent2) = (equivalent1, equivalent2);

        /// <summary>
        /// Gets the first equivalent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Equivalent1 { get; }

        /// <summary>
        /// Gets the second equivalent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Equivalent2 { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Equivalence<TDomain, TElement> otherEquivalence))
            {
                return false;
            }

            (var low, var high) = Equivalent1.GetHashCode() < Equivalent2.GetHashCode() ? (Equivalent1, Equivalent2) : (Equivalent2, Equivalent1);
            (var otherLow, var otherHigh) = otherEquivalence.Equivalent1.GetHashCode() < otherEquivalence.Equivalent2.GetHashCode() ? (otherEquivalence.Equivalent1, otherEquivalence.Equivalent2) : (otherEquivalence.Equivalent2, otherEquivalence.Equivalent1);

            return low.Equals(otherLow) && high.Equals(otherHigh);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            (var low, var high) = Equivalent1.GetHashCode() < Equivalent2.GetHashCode() ? (Equivalent1, Equivalent2) : (Equivalent2, Equivalent1);

            return HashCode.Combine(low, high);
        }
    }
}
