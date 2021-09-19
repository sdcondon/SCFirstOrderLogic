using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an equality sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{expression} == {expression}</code>
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Equality<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLEquality{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public Equality(Term<TDomain, TElement> left, Term<TDomain, TElement> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left hand side of the equality.
        /// </summary>
        public Term<TDomain, TElement> Left { get; }

        /// <summary>
        /// Gets the right hand side of the equality.
        /// </summary>
        public Term<TDomain, TElement> Right { get; }

        internal static new bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            // TODO-ROBUSTNESS: ..and Object.Equals invocation? And others? How to think about map of different types of .NET equality to FOL "equals"?
            if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal
                && Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Left), out var left)
                && Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Right), out var right))
            {
                sentence = new Equality<TDomain, TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Equality<TDomain, TElement> otherEquality))
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
