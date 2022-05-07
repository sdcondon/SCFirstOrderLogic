namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Class for Skolem function symbols.
    /// </summary>
    public class SkolemFunctionSymbol
    {
        /// <remarks>
        /// Intended only for construction by the normalisation process.
        /// </remarks>
        internal SkolemFunctionSymbol(object underlyingSymbol) => UnderlyingSymbol = underlyingSymbol;

        /// <summary>
        /// Gets the underlying (existentially quantified) variable symbol that this Skolem function represents the existential instantiation of.
        /// </summary>
        public object UnderlyingSymbol { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
        /// so this ToString override is "just in case".
        /// </remarks>
        public override string ToString() => $"SK:{UnderlyingSymbol}";
    }
}
