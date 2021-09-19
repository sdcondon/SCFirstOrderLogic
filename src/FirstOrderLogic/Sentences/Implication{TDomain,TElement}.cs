using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a material implication sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇒ {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Symbols.If({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Symbols;</c> to make this a little shorter)
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Implication<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        private static readonly (Module Module, int MetadataToken) IfMethod;

        static Implication()
        {
            var ifMethod = typeof(Operators).GetMethod(nameof(Operators.If));
            IfMethod = (ifMethod.Module, ifMethod.MetadataToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Implication{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence.</param>
        /// <param name="consequent">The consequent sentence.</param>
        public Implication(Sentence<TDomain, TElement> antecedent, Sentence<TDomain, TElement> consequent) => (Antecedent, Consequent) = (antecedent, consequent);

        /// <summary>
        /// Gets the antecedent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Antecedent { get; }

        /// <summary>
        /// Gets the consequent sentence.
        /// </summary>
        public Sentence<TDomain, TElement> Consequent { get; }

        internal static new bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == IfMethod
                && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(methodCallExpr.Arguments[0]), out var antecedent)
                && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(methodCallExpr.Arguments[1]), out var consequent))
            {
                sentence = new Implication<TDomain, TElement>(antecedent, consequent);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Implication<TDomain, TElement> otherImplication && Antecedent.Equals(otherImplication.Antecedent);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
    }
}
