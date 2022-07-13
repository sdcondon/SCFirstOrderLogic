using SCFirstOrderLogic.SentenceFormatting;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Class for symbols of variables that have been standardised as part of the normalisation process.
    /// <para/>
    /// NB: Doesn't override equality or hash code, so uses reference equality;
    /// and the normalisation process creates exactly one instance per variable scope - thus achieving standardisation
    /// without having to muck about with anything like trying to ensure names that are unique strings
    /// (which only sentence formatting logic, not the symbol itself, should care about).
    /// <para/>
    /// This is however slightly awkward to to test, and has shortcomings when serialization is involved.
    /// Also can cause inconsistent behaviour across runs when anything relies on hash code (like ClausePairPriorityComparers.UnitPreference).
    /// As such, it might be nice to implement value semantics for equality. Same variable in same (normalised) sentence is the same.
    /// Equality would need to avoid infinite loop though, and couldn't work on output of CNFConversion as-is since this
    /// class doesn't completely normalise. Definitely doable though.
    /// </summary>
    public class StandardisedVariableSymbol
    {
        /// <remarks>
        /// Internal because it doesn't check that the quantification occurs in the sentence - relies on the normalisation process to ensure that.
        /// <para/>
        /// A key consideration here is the possibility that the "same" variable (either just by equality, or full reference equality) could occur
        /// more than once in a sentence. This is completely valid, as long as their scopes don't overlap.
        /// <para/>
        /// Originally, we stored the full context in this symbol - a collection starting from the quantification and ending with the top-level sentence.
        /// This would protect against pretty much everything a user could throw at us. It'd not be able to differentiate between cases where the exact
        /// same quantification (reference) occurs twice as a child of a single sentence object, but there's no sentence type where distinguishing the two
        /// copies is important (and honestly when would that ever happen).
        /// <para/>
        /// I have however decided in the end to go for an approach that is slightly lighter from a memory usage standpoint (no extra collection on the heap),
        /// and very nearly as robust. Storing just the quantification and top-level sentence will only be potentially unclear when the exact same quantification
        /// (reference!) occurs more than once in a sentence. Not quite as strong as storing the full stack, but pretty strong (again, when would this ever happen).
        /// If needed though, we could always add some validation of the sentence here (a quick search through the sentence to ensure the quantification reference
        /// is unique), rather than necessarily have to go back to the full context approach.
        /// </remarks>
        internal StandardisedVariableSymbol(Quantification originalVariableScope, Sentence originalSentence)
        {
            OriginalVariableScope = originalVariableScope;
            OriginalSentence = originalSentence;
        }

        /// <summary>
        /// Gets the quantification in which the original variable was declared.
        /// </summary>
        public Quantification OriginalVariableScope { get; }

        /// <summary>
        /// Gets the original top-level sentence in which the variable was declared.
        /// Contains all ancestors of the original variable declaration
        /// in the sentence tree - starting with the quantification that declares the variable, and ending with the
        /// root element of the sentence. Intended for use within explanations of query results.
        /// </summary>
        public Sentence OriginalSentence { get; }

        /// <summary>
        /// Gets the original variable symbol that this symbol is the standardisation of.
        /// Intended for use within explanations of query results.
        /// </summary>

        public object OriginalSymbol => OriginalVariableScope.Variable.Symbol;

        /////// <summary>
        /////// Gets the context of the original variable symbol that this symbol is the standardisation of.
        /////// An enumeration starting from the quantification that declares the variable, moving back up through
        /////// the sentence tree to the top-level sentence.
        /////// </summary>
        ////public IEnumerable<Sentence> OriginalContext
        ////{
        ////    get
        ////    {
        ////        // DFS for originalvariablescope (by reference, just in case), keeping track of path to it as we do so.
        ////    }
        ////}

        /// <inheritdoc/>
        public override string ToString() => new SentenceFormatter().Print(this);
    }
}
