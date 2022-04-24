namespace SCFirstOrderLogic
{
    /// <summary>
    /// Singleton representation of the symbol of a predicate of first order logic that represents equality. In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// <para/>
    /// NB: Equality is just a binary predicate with particular (admittedly fairly special combination of - given that they mean that the predicate
    /// describes a partition of the terms of the domain) properties - namely commutativity, transitivity &amp; reflexivity.
    /// Not having a separate <see cref="Sentence"/> (or indeed <see cref="Predicate"/>) sub-type for it thus makes sense - and in fact makes
    /// a bunch of stuff to do with normalisation easier because it means there's a singular <see cref="Sentence"/> subtype for atomic sentences.
    /// <para/>
    /// This does of course mean that the way we render these in FoL syntax ("Equals(x, y)") doesn't match the usual FoL syntax for equality
    /// ("x = y"). Easy enough to resolve, either specifically (with a ToString overload) or in general (an IsRenderedInfix property?). Haven't done
    /// that yet because its not a big deal, is not worth the complexity, and I'm not yet sure how I want to deal with rendering in FoL syntax going forwards.
    /// </summary>
    public sealed class EqualitySymbol
    {
        private EqualitySymbol()
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="EqualitySymbol"/>.
        /// </summary>
        public static EqualitySymbol Instance { get; } = new EqualitySymbol();

        /// <inheritdoc />
        public override string ToString() => "Equals";
    }
}
