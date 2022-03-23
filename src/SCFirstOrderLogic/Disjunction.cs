using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a disjunction sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ∨ {sentence}</code>
    /// </summary>
    public sealed class Disjunction : Sentence
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
        public override bool Equals(object obj) => obj is Disjunction otherDisjunction && Left.Equals(otherDisjunction.Left) && Right.Equals(otherDisjunction.Right);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }
}
