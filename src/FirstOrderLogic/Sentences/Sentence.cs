namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract partial class Sentence
    {
        // TODO.. if we end up wanting it, visitor pattern for converting (back) to equivalent lambda better than explicit lambda prop as shown below
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        // TODO-FEATURE: Ultimately might be useful for verification - but can add this later..
        // NB: Not Expression<Predicate<TDomain>> since sub-sentences will probably also include
        // variables from quantifiers.
        // TODO-USABILITY: One way to introduce stronger type-safety here would be to add a (possibly
        // empty variable container - e.g. Expression<Predicate<TDomain, VariableContainer<TElement>>>.
        // Would need to decide if the extra complexity is worth it.
        ////public LambdaExpression Lambda { get; }
    }
}
