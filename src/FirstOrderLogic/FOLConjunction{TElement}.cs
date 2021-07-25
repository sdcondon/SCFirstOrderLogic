﻿using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a conjunction sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ∧ {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{expression} {&amp;&amp; or &amp;} {expression}</code>
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLConjunction<TElement> : FOLComplexSentence<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLConjunction{TModel}"/> class.
        /// </summary>
        /// <param name="left">The left side of the conjunction.</param>
        /// <param name="right">The right side of the conjunction.</param>
        public FOLConjunction(FOLSentence<TElement> left, FOLSentence<TElement> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the conjunction.
        /// </summary>
        public FOLSentence<TElement> Left { get; }

        /// <summary>
        /// Gets the right side of the conjunction.
        /// </summary>
        public FOLSentence<TElement> Right { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.And)
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Left), out var left)
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Right), out var right))
            {
                sentence = new FOLConjunction<TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is FOLConjunction<TElement> otherConjunction))
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
