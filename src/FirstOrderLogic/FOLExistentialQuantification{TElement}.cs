using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a existential quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∃ {variable}, {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{domain}.Any({variable} => {expression})</code>
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLExistentialQuantification<TElement> : FOLQuantification<TElement>
    {
        private static readonly (Module Module, int MetadataToken) AnyMethod;

        static FOLExistentialQuantification()
        {
            var anyMethod = typeof(Enumerable).GetMethod(
                nameof(Enumerable.Any),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            AnyMethod = (anyMethod.Module, anyMethod.MetadataToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLExistentialQuantification{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public FOLExistentialQuantification(FOLVariableTerm<TElement> variable, FOLSentence<TElement> sentence)
            : base(variable, sentence)
        {
        }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == AnyMethod
                && FOLSentence<TElement>.TryCreate(lambda.MakeSubPredicateExpr2(methodCallExpr.Arguments[1]), out var subSentence)
                // TODO-ROBUSTNESS: Ugh, so ugly, and might not be a lambda if theyve used e.g. a method - but we should handle this more gracefully than an InvalidCast
                && FOLVariableTerm<TElement>.TryCreate(lambda.MakeSubLambda(((LambdaExpression)methodCallExpr.Arguments[1]).Parameters[0]), out FOLVariableTerm<TElement> variableTerm)) 
            {
                sentence = new FOLExistentialQuantification<TElement>(variableTerm, subSentence);
                return true;
            }

            sentence = null;
            return false;
        }
    }
}
