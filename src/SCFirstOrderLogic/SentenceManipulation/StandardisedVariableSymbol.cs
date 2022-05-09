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
    /// This is however slightly awkward to to test, and has shortcomings when serialization is involved.
    /// Also can cause inconsistent behaviour across runs when anything relies on hash code (like ClausePairPriorityComparers.UnitPreference)
    /// Might be nice to implement value semantics for equality. Same variable in same (normalised) sentence is the same.
    /// Equality would need to avoid infinite loop though, and couldn't work on output of CNFConversion as-is since this
    /// class doesn't completely normalise. Definitely doable though.
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
