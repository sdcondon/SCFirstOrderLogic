using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a universal quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∀ {variable}, {sentence}</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{domain}.All({variable} => {expression})</code>
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class UniversalQuantification<TDomain, TElement> : Quantification<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        private static readonly MethodInfo AllMethod;
        private static readonly MethodInfo ShorthandAllMethod2;
        private static readonly MethodInfo ShorthandAllMethod3;

        static UniversalQuantification()
        {
            AllMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.All));
            ShorthandAllMethod2 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            ShorthandAllMethod3 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLUniversalQuantification{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public UniversalQuantification(Variable<TDomain, TElement> variable, Sentence<TDomain, TElement> sentence)
            : base(variable, sentence)
        {
        }

        internal static new bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            // TODO-MAINTAINABILITY: Ick. This is horrible. Can we recurse or something to make it more graceful without losing any more perf than we need to?

            if (lambda.Body is MethodCallExpression methodCallExpr)
            {
                if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, AllMethod)
                    && methodCallExpr.Arguments[1] is LambdaExpression allLambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(allLambda), out var allSubSentence)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(allLambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(variableTerm, allSubSentence);
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod2)
                    && methodCallExpr.Arguments[1] is LambdaExpression all2Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all2Lambda), out var all2SubSentence)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all2Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of2)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all2Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of2))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        variableTerm1Of2,
                        new UniversalQuantification<TDomain, TElement>(variableTerm2Of2, all2SubSentence));
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod3)
                    && methodCallExpr.Arguments[1] is LambdaExpression all3Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && Sentence<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all3Lambda), out var all3SubSentence)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all3Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of3)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all3Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of3)
                    && Variable<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(all3Lambda.Parameters[2]), out Variable<TDomain, TElement> variableTerm3Of3))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        variableTerm1Of3,
                        new UniversalQuantification<TDomain, TElement>(
                            variableTerm2Of3,
                            new UniversalQuantification<TDomain, TElement>(variableTerm3Of3, all3SubSentence)));
                    return true;
                }
            }

            sentence = null;
            return false;
        }
    }
}
