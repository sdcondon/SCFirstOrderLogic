using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of an equality sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// </summary>
    /// <remarks>
    /// NB: Equality is really just a predicate with particular properties (commutativity, transitivity, reflexivity).
    /// Making it a subtype of <see cref="Predicate"/> thus makes sense - and in fact makes a bunch of stuff to do with normalisation easier
    /// because it means there's a singular <see cref="Sentence"/> subtype for atomic sentences.
    /// <para/>
    /// This does of course mean that the way we render these in FoL syntax ("Equals(x, y)") doesn't match the usual FoL syntax for equality
    /// ("x = y"). Easy enough to resolve, either specifically (with a ToString overload) or in general (an IsRenderedInfix property?). Haven't done
    /// that yet because its not a big deal, is not worth the complexity, and I'm not sure how I want to deal with rendering in FoL syntax going forwards.
    /// </remarks>
    public class Equality : Predicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Equality"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public Equality(Term left, Term right)
            : base(EqualitySymbol.Instance, new[] { left, right })
        {
            Left = left;
            Right = right;
        }

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

        /// <summary>
        /// The symbol class for equality. 
        /// </summary>
        private class EqualitySymbol
        {
            public static EqualitySymbol Instance { get; } = new EqualitySymbol();

            public override string ToString() => "Equals";
        }
    }
}
