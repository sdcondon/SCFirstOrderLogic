using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of an negation sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>¬{sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>!{expression}</code>
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLNegation<TElement> : FOLComplexSentence<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLNegation{TModel}"/> class.
        /// </summary>
        /// <param name="sentence">The sentence that is negated.</param>
        public FOLNegation(FOLSentence<TElement> sentence) => Sentence = sentence;

        /// <summary>
        /// Gets the sentence that is negated.
        /// </summary>
        public FOLSentence<TElement> Sentence { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(unaryExpr.Operand), out var operand))
            {
                sentence = new FOLNegation<TElement>(operand);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is FOLNegation<TElement> negation && Sentence.Equals(negation.Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}
