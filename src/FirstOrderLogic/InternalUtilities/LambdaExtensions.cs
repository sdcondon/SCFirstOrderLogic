using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.InternalUtilities
{
    internal static class LambdaExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambda"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <remarks>
        /// TODO-USABILITY: One way to introduce stronger type-safety here would be to add a (possibly
        /// empty) variable container - e.g. Expression<Predicate<TDomain, VariableContainer<TElement>>>.
        /// Or perhaps Expresison<Predicate<FOLScope<TDomain, TElement>>>. Something for v2...
        /// Would need to decide if the extra complexity (& perf hit with variable access?) is worth it.
        /// </remarks>
        public static LambdaExpression MakeSubLambda(this LambdaExpression lambda, Expression body)
        {
            // TODO-ROBUSTNESS: Debug check that lambda contains body?
            // 

            // Flatten body that is a lamba (happens in quantifiers)
            if (body is LambdaExpression bodyLambda)
            {
                return Expression.Lambda(bodyLambda.Body, lambda.Parameters.Concat(bodyLambda.Parameters));
            }

            return Expression.Lambda(body, lambda.Parameters);
        }
    }
}
