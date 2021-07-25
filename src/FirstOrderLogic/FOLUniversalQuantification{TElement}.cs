using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a universal quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∀ {variable}, {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{domain}.All({variable} => {expression})</code>
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLUniversalQuantification<TElement> : FOLQuantification<TElement>
    {
        private static readonly (Module Module, int MetadataToken) AllMethod;
        
        static FOLUniversalQuantification()
        {
            var allMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.All));
            AllMethod = (allMethod.Module, allMethod.MetadataToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLUniversalQuantification{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public FOLUniversalQuantification(FOLVariableTerm<TElement> variable, FOLSentence<TElement> sentence)
            : base(variable, sentence)
        {
        }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == AllMethod
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr2(methodCallExpr.Arguments[1]), out var subSentence)
                // TODO-ROBUSTNESS: Ugh, so ugly, and might not be a lambda if they've used e.g. a method - but we should handle this more gracefully than an InvalidCast
                && FOLVariableTerm<TElement>.TryCreate(lambda.MakeSubLambda(((LambdaExpression)methodCallExpr.Arguments[1]).Parameters[0]), out FOLVariableTerm<TElement> variableTerm))
            {
                sentence = new FOLUniversalQuantification<TElement>(variableTerm, subSentence);
                return true;
            }

            sentence = null;
            return false;
        }
    }
}
