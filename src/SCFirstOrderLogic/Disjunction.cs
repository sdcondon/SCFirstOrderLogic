using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a disjunction sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ∨ {sentence}</code>
    /// </summary>
    public class Disjunction : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Disjunction"/> class.
        /// </summary>
        /// <param name="left">The left side of the disjunction.</param>
        /// <param name="right">The right side of the disjunction.</param>
        public Disjunction(Sentence left, Sentence right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the disjunction.
        /// </summary>
        public Sentence Left { get; }

        /// <summary>
        /// Gets the right side of the disjunction.
        /// </summary>
        public Sentence Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Disjunction otherDisjunction))
            {
                return false;
            }

            // Disjunctions are commutative - the following code ensures that A ∨ B is considered equal to B ∨ A
            // NB: Conjunction sentences with different order of evaluation - e.g. (A ∨ B) ∨ C versus A ∨ (B ∨ C) - will still be considered unequal
            // Use normalisation to CNF to account for such situations.
            // TODO: Is it really worth accounting for commutativity, given this?
            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);
            (var otherLow, var otherHigh) = otherDisjunction.Left.GetHashCode() < otherDisjunction.Right.GetHashCode() ? (otherDisjunction.Left, otherDisjunction.Right) : (otherDisjunction.Right, otherDisjunction.Left);

            return low.Equals(otherLow) && high.Equals(otherHigh);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);

            return HashCode.Combine(low, high);
        }
    }
}
