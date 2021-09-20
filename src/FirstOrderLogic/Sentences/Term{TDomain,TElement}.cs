using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Term<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        // TODO..
        //// public abstract T Accept<T>(ISentenceVisitor<T>, T);
    }
}
