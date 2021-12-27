using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of an equality sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// </summary>
    /// <remarks>
    /// TODO: Equality is really just a particular kind of predicate with particular properties (commutativity, transitivity, reflexivity..). It should thus perhaps be a subclass of <see cref="Predicate"/>?
    /// </remarks>
    public class Equality : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Equality"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public Equality(Term left, Term right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left hand side of the equality.
        /// </summary>
        public Term Left { get; }

        /// <summary>
        /// Gets the right hand side of the equality.
        /// </summary>
        public Term Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Equality otherEquality))
            {
                return false;
            }

            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);
            (var otherLow, var otherHigh) = otherEquality.Left.GetHashCode() < otherEquality.Right.GetHashCode() ? (otherEquality.Left, otherEquality.Right) : (otherEquality.Right, otherEquality.Left);

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
