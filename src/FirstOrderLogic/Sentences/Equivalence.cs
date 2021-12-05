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
    public class Equivalence : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Implication"/> class.
        /// </summary>
        /// <param name="equivalent1">The first equivalent sentence.</param>
        /// <param name="equivalent2">The second equivalent sentence.</param>
        public Equivalence(Sentence equivalent1, Sentence equivalent2) => (Equivalent1, Equivalent2) = (equivalent1, equivalent2);

        /// <summary>
        /// Gets the first equivalent sentence.
        /// </summary>
        public Sentence Equivalent1 { get; }

        /// <summary>
        /// Gets the second equivalent sentence.
        /// </summary>
        public Sentence Equivalent2 { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Equivalence otherEquivalence))
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
