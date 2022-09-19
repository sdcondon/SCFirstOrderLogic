using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Query implementation used by <see cref="SimpleBackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate query;
        private readonly IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol;

        private IEnumerable<Proof>? proofs;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol)
        {
            this.query = query;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        // BUG: will be immediately true. Need to think about how best to think about "IsComplete" when query execution
        // is an iterator method for proofs? Should IsComplete actually not be part of the interface? At the very least, clarify
        // meaning in interface docs ("IsComplete indicates that you can retrieve result without a delay.." vs "IsComplete means
        // that all work is done").
        public bool IsComplete => proofs != null; 

        /// <inheritdoc />
        public bool Result => proofs?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a human-readable explanation of the query result.
        /// </summary>
        public string ResultExplanation
        {
            get
            {
                // Don't bother lazy.. Won't be critical path - not worth the complexity hit. Might revisit.
                var formatter = new SentenceFormatter();
                var cnfExplainer = new CNFExplainer(formatter);
                var resultExplanation = new StringBuilder();

                var proofNum = 1;
                foreach (var proof in Proofs)
                {
                    resultExplanation.AppendLine($"--- PROOF #{proofNum++}");
                    resultExplanation.AppendLine();

                    // Now build the explanation string.
                    var proofStepsByPredicate = proof.Steps;
                    var orderedPredicates = proofStepsByPredicate.Keys.ToList();

                    for (var i = 0; i < orderedPredicates.Count; i++)
                    {
                        var predicate = orderedPredicates[i];
                        var proofStep = proofStepsByPredicate[predicate];

                        // Consequent:
                        resultExplanation.AppendLine($"Step #{i:D2}: {formatter.Format(predicate)}");

                        // Rule applied:
                        resultExplanation.AppendLine($"  By Rule: {proofStep.Format(formatter)}");

                        // Conjuncts used:
                        foreach (var childPredicate in proofStep.Conjuncts)
                        {
                            var unifiedChildPredicate = proof.GetUnified(childPredicate);
                            resultExplanation.AppendLine($"  And Step #{orderedPredicates.IndexOf(unifiedChildPredicate):D2}: {formatter.Format(unifiedChildPredicate)}");
                        }

                        resultExplanation.AppendLine();
                    }

                    // Output the unifier:
                    resultExplanation.Append("Using: {");
                    resultExplanation.Append(string.Join(", ", proof.Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                    resultExplanation.AppendLine("}");
                    resultExplanation.AppendLine();

                    // Explain all the normalisation terms (standardised variables and skolem functions)
                    resultExplanation.AppendLine("Where:");
                    var normalisationTermsToExplain = new HashSet<Term>();

                    foreach (var term in CNFExplainer.FindNormalisationTerms(proof.Steps.Keys.Select(p => new CNFClause(new CNFLiteral[] { p })).ToArray()))
                    {
                        normalisationTermsToExplain.Add(term);
                    }

                    foreach (var term in proof.Unifier.Bindings.SelectMany(kvp => new[] { kvp.Key, kvp.Value }).Where(t => t is VariableReference vr && vr.Symbol is StandardisedVariableSymbol))
                    {
                        normalisationTermsToExplain.Add(term);
                    }

                    foreach (var term in normalisationTermsToExplain)
                    {
                        resultExplanation.AppendLine($"  {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                    }

                    resultExplanation.AppendLine();
                }

                return resultExplanation.ToString();
            }
        }

        /// <summary>
        /// Gets the set of proofs of the query..
        /// Result will be empty if and only if the query returned a negative result.
        /// </summary>
        public IEnumerable<Proof> Proofs => proofs ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            proofs = ProvePredicate(query, new Proof());
            return Task.FromResult(Result);
        }

        private IEnumerable<Proof> ProvePredicate(Predicate goal, Proof parentProof)
        {
            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal)
                {
                    var restandardisedClause = clause.Restandardize();
                    var clauseProofPrototype = new Proof(parentProof);

                    if (LiteralUnifier.TryUpdate(restandardisedClause.Consequent, goal, clauseProofPrototype.Unifier))
                    {
                        foreach (var clauseProof in ProvePredicates(restandardisedClause.Conjuncts, clauseProofPrototype))
                        {
                            clauseProof.AddStep(clauseProof.ApplyUnifierTo(goal), restandardisedClause);
                            yield return clauseProof;
                        }
                    }
                }
            }
        }

        private IEnumerable<Proof> ProvePredicates(IEnumerable<Predicate> goals, Proof proof)
        {
            if (!goals.Any())
            {
                yield return proof;
            }
            else
            {
                foreach (var firstConjunctProof in ProvePredicate(proof.ApplyUnifierTo(goals.First()), proof))
                {
                    foreach (var restOfConjunctsProof in ProvePredicates(goals.Skip(1), firstConjunctProof))
                    {
                        yield return restOfConjunctsProof;
                    }
                }
            }
        }

        /// <summary>
        /// Container for a proof of a query.
        /// </summary>
        public class Proof
        {
            private readonly Dictionary<Predicate, CNFDefiniteClause> steps;

            internal Proof()
            {
                Unifier = new VariableSubstitution();
                steps = new();
            }

            internal Proof(Proof parent)
            {
                Unifier = new VariableSubstitution(parent.Unifier);
                steps = parent.Steps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            /// <summary>
            /// Gets the unifier used by this proof.
            /// </summary>
            public VariableSubstitution Unifier { get; }

            /// <summary>
            /// Gets the steps of the proof. Each predicate used in the proof (including the goal) is present as a key.
            /// The associated value is the clause that was used to prove it. Each conjunct of that clause will be present
            /// as a key - all the way back to the relevant unit clauses.
            /// </summary>
            public IReadOnlyDictionary<Predicate, CNFDefiniteClause> Steps => steps;

            /// <summary>
            /// Applies the proof's unifier to a given predicate.
            /// <para/>
            /// NB: the algorithm builds up the unifier step-by-step, so that it can (will probably) result in many
            /// terms being bound indirectly. E.g. variable A is bound to variable B, which is in turn bound to constant C.
            /// There's no sense in making the algorithm itself more complex (and thus slower) than it needs to be to streamline
            /// this, but for the readability of explanations, this method follows the chain to the end by applying the
            /// unifier until it doesn't make any more changes. The "full story" remains accessible via the unifier and proof steps.
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns>The input predicate transformed by the proof's unifier.</returns>
            public Predicate GetUnified(Predicate predicate)
            {
                var newPredicate = ApplyUnifierTo(predicate);
                while (!newPredicate.Equals(predicate))
                {
                    predicate = newPredicate;
                    newPredicate = ApplyUnifierTo(predicate);
                }

                return predicate;
            }

            internal Predicate ApplyUnifierTo(Predicate predicate) => Unifier.ApplyTo(predicate).Predicate;

            internal void AddStep(Predicate predicate, CNFDefiniteClause rule) => steps[predicate] = rule;
        }
    }
}
