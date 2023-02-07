namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// Interface for types that represent a labelling scope - a scope within which labels
    /// should be uniquely assigned to symbols.
    /// </summary>
    /// <typeparam name="T">The type of the symbols to be labelled.</typeparam>
    public interface ILabellingScope<T>
    {
        /// <summary>
        /// Get the label for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to make or retrieve the label for.</param>
        /// <returns>A label for the symbol that is unique within this scope.</returns>
        string GetLabel(T symbol);
    }
}
