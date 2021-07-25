using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of an equality sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{expression} == {expression}</code>
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLEquality<TElement> : FOLAtomicSentence<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLEquality{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public FOLEquality(FOLTerm<TElement> left, FOLTerm<TElement> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left hand side of the equality.
        /// </summary>
        public FOLTerm<TElement> Left { get; }

        /// <summary>
        /// Gets the right hand side of the equality.
        /// </summary>
        public FOLTerm<TElement> Right { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            // TODO-ROBUSTNESS: ..and Object.Equals invocation? And others? How to think about map of different types of .NET equality to FOL "equals"?
            if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal
                && FOLTerm<TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Left), out var left)
                && FOLTerm<TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Right), out var right))
            {
                sentence = new FOLEquality<TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is FOLEquality<TElement> otherEquality))
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
