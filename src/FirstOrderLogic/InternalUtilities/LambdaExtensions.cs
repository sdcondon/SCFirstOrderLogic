using System;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.InternalUtilities
{
    internal static class LambdaExtensions
    {
        public static Expression<Predicate<T>> MakeSubPredicateExpr<T>(this Expression<Predicate<T>> lambda, Expression body)
        {
            // TODO-ROBUSTNESS: (Debug) check that lambda contains body?
            // TODO-ROBUSTNESS: Hmm. The below will throw if body isn't Boolean-valued - but should we check and throw an ArgEx here for usability?
            return Expression.Lambda<Predicate<T>>(body, lambda.Parameters);
        }

        public static Expression<Predicate<T>> MakeSubPredicateExpr2<T>(this Expression<Predicate<T>> lambda, Expression body)
        {
            // TODO-ROBUSTNESS: Debug check that lambda contains body?
            // 
            if (!(body is LambdaExpression bodyLambda))
            {
                throw new Exception("BOOM");
            }

            // TODO-SHOWSTOPPER: For the lambda to make sense, needs to add the bodyLambda parameters to the parameters here.
            // But then the result signature is no longer correct. Sorting this out has deep consequences for the design of the lib.
            return Expression.Lambda<Predicate<T>>(bodyLambda.Body, lambda.Parameters);
        }

        public static LambdaExpression MakeSubLambda(this LambdaExpression lambda, Expression body)
        {
            // TODO-ROBUSTNESS: Debug check that lambda contains body?
            // 
            return Expression.Lambda(body, lambda.Parameters);
        }
    }
}
