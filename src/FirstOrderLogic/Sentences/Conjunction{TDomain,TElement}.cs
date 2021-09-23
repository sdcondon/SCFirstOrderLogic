using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a conjunction sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ∧ {sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{expression} {&amp;&amp; or &amp;} {expression}</code>
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Conjunction<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Conjunction{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="left">The left side of the conjunction.</param>
        /// <param name="right">The right side of the conjunction.</param>
        public Conjunction(Sentence<TDomain, TElement> left, Sentence<TDomain, TElement> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the conjunction.
        /// </summary>
        public Sentence<TDomain, TElement> Left { get; }

        /// <summary>
        /// Gets the right side of the conjunction.
        /// </summary>
        public Sentence<TDomain, TElement> Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Conjunction<TDomain, TElement> otherConjunction))
            {
                return false;
            }

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
