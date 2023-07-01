// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic
{
    /// <summary>
    /// <para>
    /// Singleton representation of the identifier of the equality predicate. In typical FOL syntax, this is written as:
    /// <code>{term} = {term}</code>
    /// </para>
    /// <para>
    /// NB: Equality is just a binary predicate with particular (admittedly fairly special combination of - given that they mean that the predicate
    /// describes a partition of the terms of the domain) properties - namely commutativity, transitivity &amp; reflexivity.
    /// Not having a separate <see cref="Sentence"/> (or indeed <see cref="Predicate"/>) sub-type for it thus makes sense - and in fact makes
    /// a bunch of stuff to do with normalisation easier because it means there's a singular <see cref="Sentence"/> subtype for atomic sentences.
    /// </para>
    /// <para>
    /// This does of course mean that the way we render these in FoL syntax ("Equals(x, y)") doesn't match the usual FoL syntax for equality
    /// ("x = y"). Easy enough to resolve, either specifically (by accounting specifically for this identifier in formatting logic) or in general (an IPredicateIdentifier 
    /// interface with an IsRenderedInfix property?). Haven't done that yet because its not a big deal, is not worth the complexity, and I'm not yet sure
    /// how I want to deal with rendering in FoL syntax going forwards.
    /// </para>
    /// </summary>
    public sealed class EqualityIdentifier
    {
        private EqualityIdentifier() {}

        /// <summary>
        /// Gets the singleton instance of <see cref="EqualityIdentifier"/>.
        /// </summary>
        public static EqualityIdentifier Instance { get; } = new EqualityIdentifier();

        /// <inheritdoc />
        public override string ToString() => "Equals";
    }
}
