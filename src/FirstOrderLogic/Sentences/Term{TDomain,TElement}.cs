using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Term<TDomain, TElement>
    {
        internal static bool TryCreate(LambdaExpression lambda, out Term<TDomain, TElement> sentence)
        {
            return Function<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Constant<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Variable<TDomain, TElement>.TryCreate(lambda, out sentence);
        }
    }
}
