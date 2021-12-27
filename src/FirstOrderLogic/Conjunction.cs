using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a conjunction sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ∧ {sentence}</code>
    /// </summary>
    public class Conjunction : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Conjunction"/> class.
        /// </summary>
        /// <param name="left">The left side of the conjunction.</param>
        /// <param name="right">The right side of the conjunction.</param>
        public Conjunction(Sentence left, Sentence right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the conjunction.
        /// </summary>
        public Sentence Left { get; }

        /// <summary>
        /// Gets the right side of the conjunction.
        /// </summary>
        public Sentence Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Conjunction otherConjunction))
            {
                return false;
            }

            // Disjunctions are commutative - the following code ensures that A ∧ B is considered equal to B ∧ A
            // NB: Disjunction sentences with different associativity - e.g. (A ∧ B) ∧ C versus A ∧ (B ∧ C) - will still be considered unequal
            // Use normalisation to CNF to account for such situations.
            // TODO: Is it really worth accounting for commutativity
            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);
            (var otherLow, var otherHigh) = otherConjunction.Left.GetHashCode() < otherConjunction.Right.GetHashCode() ? (otherConjunction.Left, otherConjunction.Right) : (otherConjunction.Right, otherConjunction.Left);
            
            return low.Equals(otherLow) && high.Equals(otherHigh);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            (var lowSentence, var highSentence) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);

            return HashCode.Combine(lowSentence, highSentence);
        }
    }
}
