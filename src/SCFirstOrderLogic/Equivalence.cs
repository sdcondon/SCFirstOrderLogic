using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a material equivalence sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇔ {sentence}</code>
    /// </summary>
    public sealed class Equivalence : Sentence
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
        public override bool Equals(object obj) => obj is Equivalence otherEquivalence && Left.Equals(otherEquivalence.Left) && Right.Equals(otherEquivalence.Right);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }
}
