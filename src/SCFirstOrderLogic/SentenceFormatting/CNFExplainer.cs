using System;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// Utility logic for human-readable explanations of CNF clauses - notably, the normalisation terms (standardised variables and Skolem functions) within them.
    /// </summary>
    public class CNFExplainer
    {
        private readonly SentenceFormatter sentenceFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CNFExaminer"/> class.
        /// </summary>
        /// <param name="sentenceFormatter">The formatter to use when formatting sentence elements as part of output.</param>
        public CNFExplainer(SentenceFormatter sentenceFormatter)
        {
            this.sentenceFormatter = sentenceFormatter;
        }

        /// <summary>
        /// <para>
        /// Outputs a human-readable string for a given normalisation term (standardised variable or Skolem function). Throws an exception if the passed term is not a normalisation term.
        /// </para>
        /// <para>
        /// TODO-LOCALISATION: (if I get bored or this ever takes off for whatever reason): Allow for localisation and allow specification of culture in ctor (optional - default to current culture).
        /// </para>
        /// </summary>
        /// <param name="term">The term to examine.</param>
        /// <returns>A human-readable string that completes the sentence "{term} is .."</returns>
        public string ExplainNormalisationTerm(Term term)
        {
            if (term is Function function && function.Symbol is SkolemFunctionSymbol skolemFunctionSymbol)
            {
                return $"some {sentenceFormatter.Format(skolemFunctionSymbol.StandardisedVariableSymbol)} from {sentenceFormatter.Format(skolemFunctionSymbol.OriginalSentence)}";
            }
            else if (term is VariableReference variable && variable.Symbol is StandardisedVariableSymbol standardisedVariableSymbol)
            {
                return $"a standardisation of {standardisedVariableSymbol.OriginalSymbol} from {sentenceFormatter.Format(standardisedVariableSymbol.OriginalSentence)}";
            }
            else
            {
                throw new ArgumentException("The provided term is not a standardised variable reference or Skolem function", nameof(term));
            }
        }
    }
}
