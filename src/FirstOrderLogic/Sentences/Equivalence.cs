using System;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a material equivalence sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇔ {sentence}</code>
    /// </summary>
    public class Equivalence : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Equivalence"/> class.
        /// </summary>
        /// <param name="left">The left side of the equivalence.</param>
        /// <param name="right">The right side of the equivalence.</param>
        public Equivalence(Sentence left, Sentence right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the equivalence.
        /// </summary>
        public Sentence Left { get; }

        /// <summary>
        /// Gets the right side of the equivalence.
        /// </summary>
        public Sentence Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Equivalence otherEquivalence))
            {
                return false;
            }

            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);
            (var otherLow, var otherHigh) = otherEquivalence.Left.GetHashCode() < otherEquivalence.Right.GetHashCode() ? (otherEquivalence.Left, otherEquivalence.Right) : (otherEquivalence.Right, otherEquivalence.Left);

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
