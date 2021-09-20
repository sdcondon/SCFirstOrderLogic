using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        // TODO.. if we end up wanting it, visitor pattern for converting to equivalent lambda better than explicit lambda prop as shown below
        // and we'll want visitor anyway to work with sentences..
        //// public abstract T Accept<T>(ISentenceVisitor<T>, T);

        // TODO-FEATURE: Ultimately might be useful for verification - but can add this later..
        // NB: Not Expression<Predicate<TDomain>> since sub-sentences will probably also include
        // variables from quantifiers.
        // TODO-USABILITY: One way to introduce stronger type-safety here would be to add a (possibly
        // empty variable container - e.g. Expression<Predicate<TDomain, VariableContainer<TElement>>>.
        // Would need to decide if the extra complexity is worth it.
        ////public LambdaExpression Lambda { get; }
    }
}
