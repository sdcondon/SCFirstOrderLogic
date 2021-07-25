using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a material implication sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇒ {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Symbols.If({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Symbols;</c> to make this a little shorter)
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLImplication<TElement> : FOLComplexSentence<TElement>
    {
        private static readonly (Module Module, int MetadataToken) IfMethod;

        static FOLImplication()
        {
            var ifMethod = typeof(Symbols).GetMethod(nameof(Symbols.If));
            IfMethod = (ifMethod.Module, ifMethod.MetadataToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLImplication{TModel}"/> class.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence.</param>
        /// <param name="consequent">The consequent sentence.</param>
        public FOLImplication(FOLSentence<TElement> antecedent, FOLSentence<TElement> consequent) => (Antecedent, Consequent) = (antecedent, consequent);

        /// <summary>
        /// Gets the antecedent sentence.
        /// </summary>
        public FOLSentence<TElement> Antecedent { get; }

        /// <summary>
        /// Gets the consequent sentence.
        /// </summary>
        public FOLSentence<TElement> Consequent { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == IfMethod
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(methodCallExpr.Arguments[0]), out var antecedent)
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(methodCallExpr.Arguments[1]), out var consequent))
            {
                sentence = new FOLImplication<TElement>(antecedent, consequent);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is FOLImplication<TElement> otherImplication && Antecedent.Equals(otherImplication.Antecedent);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
    }
}
