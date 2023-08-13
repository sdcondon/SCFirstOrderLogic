﻿// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// <para>
    /// Class for Skolem function identifiers, which are generated by the sentence normalisation process and feature in sentences in conjunctive normal form (CNF).
    /// </para>
    /// <para>
    /// The important thing about this class is that it doesn't override equality or hash code, so uses reference equality.
    /// The normalisation process creates exactly one instance per variable scope - thus achieving standardisation
    /// without having to muck about with anything like trying to ensure names that are unique strings (which only sentence
    /// formatting logic, not the identifier itself, should care about).
    /// </para>
    /// </summary>
    // As with standardised variables, optional value semantics for equality might be useful.
    // An equality comparer for this may appear in a future version.
    public class SkolemFunctionIdentifier
    {
        /// <remarks>
        /// Intended only for construction by the normalisation process.
        /// (Internal means we don't have to validate that the variable is standardised).
        /// </remarks>
        internal SkolemFunctionIdentifier(StandardisedVariableIdentifier variableIdentifier, Sentence originalSentence)
        {
            VariableIdentifier = variableIdentifier;
            OriginalSentence = originalSentence;
        }

        /// <summary>
        /// Gets the original top-level sentence in which the variable was declared.
        /// Intended for use within explanations of query results.
        /// </summary>
        public Sentence OriginalSentence { get; }

        /// <summary>
        /// Gets the identifier of the underlying (existentially quantified) variable that this Skolem function represents the existential instantiation of.
        /// </summary>
        public StandardisedVariableIdentifier VariableIdentifier { get; }

        /////// <summary>
        /////// Gets the quantification in which the original variable was declared.
        /////// </summary>
        ////public ExistentialQuantification OriginalVariableScope => .. // Find the variable's declaration in OriginalSentence. It's standardised, so we know it's unique.

        //// TODO-ZZ-FEATURE: We could (should?) include some information about what the function parameters (if any) represent.
        //// (i.e. what universally declared vars are in scope).

        /// <inheritdoc/>
        public override string ToString() => new SentenceFormatter().Format(this);
    }
}
