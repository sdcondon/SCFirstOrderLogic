using System.Collections.Generic;

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
    /// TODO-TESTABILITY: Difficult to test, and has shortcomings when serialization is involved. 
    /// Would ultimately rather implement value semantics for equality. Same variable in same sentence is the same.
    /// Inside the var, could store sentence tree re-arranged so that var is the root.
    /// Equality would need to avoid infinite loop though, and couldn't work on output of CNFConversion since this
    /// class doesn't completely normalise. Perhaps made easier by the fact that after normalisation, all surviving variables
    /// are universally quantified.
    /// </summary>
    public class StandardisedVariableSymbol
    {
        /// <remarks>
        /// Intended only for construction by the normalisation process.
        /// </remarks>
        internal StandardisedVariableSymbol(object originalSymbol, Stack<Sentence> originalContext)
        {
            OriginalSymbol = originalSymbol;
            OriginalContext = originalContext.ToArray();
        }

        /// <summary>
        /// Gets the original variable symbol that this symbol is the standardisation of.
        /// </summary>
        public object OriginalSymbol { get; }

        /// <summary>
        /// Gets the original context of the variable. Contains all ancestors of the original variable declaration
        /// in the sentence tree - starting with the quantification that declares the variable, and ending with the
        /// root element of the sentence.
        /// </summary>
        public IReadOnlyList<Sentence> OriginalContext { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
        /// so this ToString override is "just in case".
        /// </remarks>
        public override string ToString() => $"ST:{OriginalSymbol}";
    }
}
