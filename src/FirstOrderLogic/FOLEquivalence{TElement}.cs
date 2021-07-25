using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a material equivalence sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇔ {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Symbols.Iff({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Symbols;</c> to make this a little shorter)
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLEquivalence<TElement> : FOLComplexSentence<TElement>
    {
        private static readonly (Module Module, int MetadataToken) IffMethod;

        static FOLEquivalence()
        {
            var iffMethod = typeof(Symbols).GetMethod(nameof(Symbols.Iff));
            IffMethod = (iffMethod.Module, iffMethod.MetadataToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLImplication{TModel}"/> class.
        /// </summary>
        /// <param name="equivalent1">The first equivalent sentence.</param>
        /// <param name="equivalent2">The second equivalent sentence.</param>
        public FOLEquivalence(FOLSentence<TElement> equivalent1, FOLSentence<TElement> equivalent2) => (Equivalent1, Equivalent2) = (equivalent1, equivalent2);

        /// <summary>
        /// Gets the first equivalent sentence.
        /// </summary>
        public FOLSentence<TElement> Equivalent1 { get; }

        /// <summary>
        /// Gets the second equivalent sentence.
        /// </summary>
        public FOLSentence<TElement> Equivalent2 { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == IffMethod
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(methodCallExpr.Arguments[0]), out var equivalent1)
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr(methodCallExpr.Arguments[1]), out var equivalent2))
            {
                sentence = new FOLEquivalence<TElement>(equivalent1, equivalent2);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is FOLEquivalence<TElement> otherEquivalence))
            {
                return false;
            }

            (var low, var high) = Equivalent1.GetHashCode() < Equivalent2.GetHashCode() ? (Equivalent1, Equivalent2) : (Equivalent2, Equivalent1);
            (var otherLow, var otherHigh) = otherEquivalence.Equivalent1.GetHashCode() < otherEquivalence.Equivalent2.GetHashCode() ? (otherEquivalence.Equivalent1, otherEquivalence.Equivalent2) : (otherEquivalence.Equivalent1, otherEquivalence.Equivalent2);

            return low.Equals(otherLow) && high.Equals(otherHigh);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            (var low, var high) = Equivalent1.GetHashCode() < Equivalent2.GetHashCode() ? (Equivalent1, Equivalent2) : (Equivalent2, Equivalent1);

            return HashCode.Combine(low, high);
        }
    }
}
