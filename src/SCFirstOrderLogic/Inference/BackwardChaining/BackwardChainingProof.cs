// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// Container for a proof of a <see cref="BackwardChainingQuery"/>.
    /// </summary>
    public class BackwardChainingProof
    {
        private readonly Dictionary<Predicate, CNFDefiniteClause> steps;

        internal BackwardChainingProof()
        {
            steps = new();
            Unifier = new VariableSubstitution();
        }

        internal BackwardChainingProof(IEnumerable<KeyValuePair<Predicate, CNFDefiniteClause>> steps, VariableSubstitution unifier)
        {
            this.steps = new(steps);
            Unifier = unifier;
        }

        /// <summary>
        /// Gets the unifier used by this proof.
        /// </summary>
        // NB: I can't decide whether I prefer this approach (a single unifier for the whole proof - 
        // which of course requires us to restandardise variables for each step), or the unifier per
        // proof step used in forward chaining and resolution. Might play with some more alternative
        // implementations at some point..
        public VariableSubstitution Unifier { get; }

        /// <summary>
        /// Gets the steps of the proof. Each predicate used in the proof (including the goal) is present as a key.
        /// The associated value is the clause that was used to prove it. Each conjunct of that clause will also be present
        /// as a key - all the way back to and inclusive of the relevant unit clauses that are present in the KB (which
        /// of course have no conjuncts).
        /// </summary>
        public IReadOnlyDictionary<Predicate, CNFDefiniteClause> Steps => steps;

        /// <summary>
        /// Gets a human-readable (English) explanation of the proof.
        /// </summary>
        /// <param name="formatter">The sentence formatter to use.</param>
        public string GetExplanation(SentenceFormatter formatter)
        {
            var explanationBuilder = new StringBuilder();

            var stepList = steps.Keys.ToList();
            var stepIndex = 0;
            foreach(var (predicate, rule) in steps)
            {
                // Consequent:
                explanationBuilder.AppendLine($"Step #{stepIndex:D2}: {formatter.Format(predicate)}");

                // Rule applied:
                explanationBuilder.AppendLine($"  By Rule: {formatter.Format(rule)}");

                // Conjuncts used:
                foreach (var conjunct in rule.Conjuncts.Select(c => ApplyUnifierTo(c)))
                {
                    explanationBuilder.AppendLine($"  And Step #{stepList.IndexOf(conjunct):D2}: {formatter.Format(conjunct)}");
                }

                explanationBuilder.AppendLine();
                stepIndex++;
            }

            // Output the unifier:
            explanationBuilder.Append("Using: {");
            explanationBuilder.Append(string.Join(", ", Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
            explanationBuilder.AppendLine("}");

            // Explain all the normalisation terms (standardised variables and skolem functions)
            var normalisationTermsToExplain = new HashSet<Term>();

            foreach (var term in CNFInspector.FindNormalisationTerms(steps.Keys))
            {
                normalisationTermsToExplain.Add(term);
            }

            foreach (var term in Unifier.Bindings.SelectMany(kvp => new[] { kvp.Key, kvp.Value }).Where(t => t is VariableReference vr && vr.Identifier is StandardisedVariableIdentifier))
            {
                normalisationTermsToExplain.Add(term);
            }

            if (normalisationTermsToExplain.Any())
            {
                var cnfExplainer = new CNFExplainer(formatter);
                explanationBuilder.AppendLine();
                explanationBuilder.AppendLine("Where:");
                foreach (var term in normalisationTermsToExplain)
                {
                    explanationBuilder.AppendLine($"  {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                }
            }

            // NB: we don't bother caching the resulting string.
            // This won't be anywhere near a critical path - not worth the complexity hit.
            return explanationBuilder.ToString();
        }

        internal Predicate ApplyUnifierTo(Predicate predicate) => Unifier.ApplyTo(predicate).Predicate;

        internal void AddStep(Predicate predicate, CNFDefiniteClause rule) => steps[predicate] = rule;
    }
}
