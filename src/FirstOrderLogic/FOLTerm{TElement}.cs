using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class FOLTerm<TElement> // TODO: Will almost certainly need (possibly a derived class with - though be careful of boxing, i guess) a second generic type param - the type of the term..
    {
        internal static bool TryCreate(LambdaExpression lambda, out FOLTerm<TElement> sentence)
        {
            return FOLFunctionTerm<TElement>.TryCreate(lambda, out sentence)
                || FOLConstantTerm<TElement>.TryCreate(lambda, out sentence)
                || FOLVariableTerm<TElement>.TryCreate(lambda, out sentence);
        }
    }
}
