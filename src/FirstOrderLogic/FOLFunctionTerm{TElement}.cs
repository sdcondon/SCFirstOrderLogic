using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLFunctionTerm<TElement> : FOLTerm<TElement>
    {
        internal static new bool TryCreate(LambdaExpression lambda, out FOLTerm<TElement> term)
        {
            // TODO!
            term = null;
            return false;
        }
    }
}
