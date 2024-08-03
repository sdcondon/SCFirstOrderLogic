// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation.Normalisation;

/// <summary>
/// <para>
/// Class for identifiers of variables that have been standardised as part of the normalisation process.
/// </para>
/// <para>
/// The important thing about this class is that it doesn't override equality or hash code, so uses reference equality.
/// The normalisation process creates exactly one instance per variable scope - thus achieving standardisation
/// without having to muck about with anything like trying to ensure names that are unique strings (which only sentence
/// formatting logic, not the identifier itself, should care about).
/// </para>
/// </summary>
// The equality behaviour here is however slightly awkward to to test, and has shortcomings when serialization is involved.
// Also can cause inconsistent behaviour across runs when anything relies on hash code (like ClausePairPriorityComparers.UnitPreference).
// As such, it *might* be nice to *also* implement value semantics for equality. Same variable in same (normalised) sentence is the same.
// Equality would need to avoid infinite loop though, and couldn't work on output of CNFConversion as-is since this
// class doesn't completely normalise. Also note potential problems when we need to restandardise variables (happens in backward chaining).
public class StandardisedVariableIdentifier
{
    /// <remarks>
    /// <para>
    /// Internal because it doesn't check that the quantification occurs in the sentence - relies on the normalisation process to ensure that.
    /// </para>
    /// <para>
    /// A key consideration here is the possibility that the "same" variable (either just by equality, or full reference equality) could occur
    /// more than once in a sentence. This is completely valid, as long as their scopes don't overlap.
    /// </para>
    /// <para>
    /// Originally, we stored the full context in this identifier - a collection starting from the quantification and ending with the top-level sentence.
    /// This would protect against pretty much everything a user could throw at us. It'd not be able to differentiate between cases where the exact
    /// same quantification (reference) occurs twice as a child of a single sentence object, but there's no sentence type where distinguishing the two
    /// copies is important (and honestly when would that ever happen).
    /// </para>
    /// <para>
    /// I have however decided in the end to go for an approach that is slightly lighter from a memory usage standpoint (no extra collection on the heap),
    /// and very nearly as robust. Storing just the quantification and top-level sentence will only be potentially unclear when the exact same quantification
    /// (reference!) occurs more than once in a sentence. Not quite as strong as storing the full stack, but pretty strong (again, when would this ever happen).
    /// If needed though, we could always add some validation of the sentence here (a quick search through the sentence to ensure the quantification reference
    /// is unique), rather than necessarily have to go back to the full context approach.
    /// </para>
    /// </remarks>
    internal StandardisedVariableIdentifier(Quantification originalVariableScope, Sentence originalSentence)
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
    /// Intended for use within explanations of query results.
    /// </summary>
    public Sentence OriginalSentence { get; }

    /// <summary>
    /// Gets the original variable identifier that this identifier is the standardisation of.
    /// Intended for use within explanations of query results.
    /// </summary>
    public object OriginalIdentifier => OriginalVariableScope.Variable.Identifier;

    /////// <summary>
    /////// Gets the context of the original variable identifier that this identifier is the standardisation of.
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
    public override string? ToString() => OriginalIdentifier.ToString();
}
