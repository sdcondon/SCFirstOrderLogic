using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an negation sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>¬{sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>!{expression}</code>
    /// We also interpret <c>!=</c> as a negation of an equality.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Negation<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLNegation{TModel}"/> class.
        /// </summary>
        /// <param name="sentence">The sentence that is negated.</param>
        public Negation(Sentence<TDomain, TElement> sentence) => Sentence = sentence;

        /// <summary>
        /// Gets the sentence that is negated.
        /// </summary>
        public Sentence<TDomain, TElement> Sentence { get; }

        internal static new bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            if (lambda.Body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(unaryExpr.Operand), out var operand))
            {
                sentence = new Negation<TDomain, TElement>(operand);
                return true;
            }
            else if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.NotEqual
                && Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Left), out var left)
                && Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(binaryExpr.Right), out var right))
            {
                sentence = new Negation<TDomain, TElement>(new Equality<TDomain, TElement>(left, right));
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Negation<TDomain, TElement> negation && Sentence.Equals(negation.Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}
