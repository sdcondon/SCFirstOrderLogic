// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using System;

namespace SCFirstOrderLogic.SentenceManipulation.Normalisation;

/// <summary>
/// Utility logic for human-readable explanations of CNF clauses - notably, the normalisation terms (standardised variables and Skolem functions) within them.
/// </summary>
public class CNFExplainer
{
    private readonly SentenceFormatter sentenceFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CNFExplainer"/> class.
    /// </summary>
    /// <param name="sentenceFormatter">The formatter to use when formatting sentence elements as part of output.</param>
    public CNFExplainer(SentenceFormatter sentenceFormatter)
    {
        this.sentenceFormatter = sentenceFormatter;
    }

    /// <summary>
    /// <para>
    /// Outputs a human-readable string for a given normalisation term (standardised variable or Skolem function).
    /// Throws an exception if the passed term is not a normalisation term.
    /// </para>
    /// </summary>
    /// <param name="term">The term to examine.</param>
    /// <returns>A human-readable string that completes the sentence "{term} is .."</returns>
    // TODO-ZZ-LOCALISATION: (if I get bored or this ever takes off for whatever reason): Allow for localisation and
    // allow specification of culture in ctor (optional - default to current culture).
    public string ExplainNormalisationTerm(Term term)
    {
        if (term is Function function && function.Identifier is SkolemFunctionIdentifier skolemFunctionIdentifier)
        {
            return $"some {sentenceFormatter.Format(skolemFunctionIdentifier.VariableIdentifier)} from {sentenceFormatter.Format(skolemFunctionIdentifier.OriginalSentence)}";
        }
        else if (term is VariableReference variable && variable.Identifier is StandardisedVariableIdentifier standardisedVariableIdentifier)
        {
            // ..doesn't necessarily help if the original identifier occurs more than once (in different scopes, of course - else normalisation
            // should have thrown). "Xth occurence (left-to-right) of.."? Not a big deal - an edge case to deal with at a later date.
            return $"a standardisation of {standardisedVariableIdentifier.OriginalIdentifier} from {sentenceFormatter.Format(standardisedVariableIdentifier.OriginalSentence)}";
        }
        else
        {
            throw new ArgumentException("The provided term is not a standardised variable reference or Skolem function", nameof(term));
        }
    }
}
