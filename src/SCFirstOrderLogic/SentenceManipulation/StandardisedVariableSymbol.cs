namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Class for symbols of variables that have been standardised as part of the normalisation process.
    /// <para/>
    /// NB: Doesn't override equality or hash code, so uses reference equality;
    /// and the normalisation process creates exactly one instance per variable scope - thus achieving standardisation
    /// without having to muck about with anything like trying to ensure names that are unique strings
    /// (which should only be a rendering concern).
    /// <para/>
    /// TODO-TESTABILITY: Difficult to test. Even with the above, would perhaps still rather implement value semantics for equality.
    /// Same variable in same sentence is the same (inside the var, store sentence tree re-arranged so that var is the root.
    /// Equality would need to avoid infinite loop though, and couldn't work on output of this CNFConversion since this
    /// class doesn't completely normalise. Perhaps made easier by the fact that after normalisation, all surviving variables
    /// are universally quantified).
    /// </summary>
    public class StandardisedVariableSymbol
    {
        /// <remarks>
        /// Intended only for construction by the normalisation process.
        /// </remarks>
        internal StandardisedVariableSymbol(object underlyingSymbol) => UnderlyingSymbol = underlyingSymbol;

        /// <summary>
        /// Gets the underlying variable symbol that this symbol is the standardisation of.
        /// </summary>
        public object UnderlyingSymbol { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
        /// so this ToString override is "just in case".
        /// </remarks>
        public override string ToString() => $"ST:{UnderlyingSymbol}";
    }
}
