namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Class for Skolem function symbols.
    /// </summary>
    /// <remarks>
    /// NB: Doesn't override equality or hash code, so uses reference equality;
    /// and the normalisation process creates exactly one instance per variable scope - thus achieving standardisation
    /// without having to muck about with anything like trying to ensure names that are unique strings
    /// (which should only be a rendering concern anyway).
    /// <para/>
    /// As with standardised variables, value semantics for equality might be useful.
    /// </remarks>
    public class SkolemFunctionSymbol
    {
        /// <remarks>
        /// Intended only for construction by the normalisation process.
        /// </remarks>
        internal SkolemFunctionSymbol(StandardisedVariableSymbol existentialVariableSymbol) => ExistentialVariableSymbol = existentialVariableSymbol;

        /// <summary>
        /// Gets the underlying (existentially quantified) variable symbol that this Skolem function represents the existential instantiation of.
        /// </summary>
        public StandardisedVariableSymbol ExistentialVariableSymbol { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
        /// so this ToString override is "just in case".
        /// </remarks>
        public override string ToString() => $"SK:{ExistentialVariableSymbol}";
    }
}
