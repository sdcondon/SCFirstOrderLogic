using SCFirstOrderLogic;
using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// The implementation of <see cref="IQuery"/> used by <see cref="SimpleBackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate queryGoal;
        private readonly IClauseStore clauseStore;

        private int executeCount = 0;
        private List<Proof>? proofs;

        internal SimpleBackwardChainingQuery(Predicate queryGoal, IClauseStore clauseStore)
        {
            this.queryGoal = queryGoal;
            this.clauseStore = clauseStore;
        }

        /// <inheritdoc />
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
                        resultExplanation.AppendLine($"  By Rule: {formatter.Format(proofStep)}");

                        // Conjuncts used:
                        foreach (var childPredicate in proofStep.Conjuncts)
                        {
                            resultExplanation.AppendLine($"  And Step #{orderedPredicates.IndexOf(childPredicate):D2}: {formatter.Format(childPredicate)}");
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

                    foreach (var term in CNFInspector.FindNormalisationTerms(proof.Steps.Keys))
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
        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // ..while it might be nice to allow for other threads to just get the existing task back
            // if its already been started, the possibility of the cancellation token being different
            // makes it awkward. The complexity added by attempting to deal with that simply isn't worth it.
            // So, we just throw if the query is already in progress. Messing about with a query from
            // multiple threads is fairly unlikely anyway (as opposed wanting an individual query to
            // parallelise itself - which is definitely something I want to look at).
            if (Interlocked.Exchange(ref executeCount, 1) == 1)
            {
                throw new InvalidOperationException("Query execution has already begun via a prior ExecuteAsync invocation");
            }

            proofs = await ProvePredicate(queryGoal, new Proof()).ToListAsync(cancellationToken);
            return Result;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            //// Nothing to do
        }

        private async IAsyncEnumerable<Proof> ProvePredicate(Predicate goal, Proof parentProof)
        {
            // NB: This implementation is coded to be a depth-first and-or search, but the clause store can at least
            // control which branches get explored first by ordering the returned clause applications appropriately.
            await foreach (var (clause, substitution) in clauseStore.GetClauseApplications(goal, parentProof.Unifier))
            {
                await foreach (var clauseProof in ProvePredicates(clause.Conjuncts, new Proof(parentProof.Steps, substitution)))
                {
                    clauseProof.AddStep(clauseProof.ApplyUnifierTo(goal), clause);
                    yield return clauseProof;
                }
            }
        }

        private async IAsyncEnumerable<Proof> ProvePredicates(IEnumerable<Predicate> goals, Proof proof)
        {
            if (!goals.Any())
            {
                yield return proof;
            }
            else
            {
                await foreach (var firstGoalProof in ProvePredicate(proof.ApplyUnifierTo(goals.First()), proof))
                {
                    await foreach (var restOfGoalsProof in ProvePredicates(goals.Skip(1), firstGoalProof))
                    {
                        yield return restOfGoalsProof;
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

            internal Proof(IEnumerable<KeyValuePair<Predicate, CNFDefiniteClause>> steps, VariableSubstitution unifier)
            {
                Unifier = unifier;
                this.steps = steps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            /// <summary>
            /// Gets the unifier used by this proof.
            /// </summary>
            public VariableSubstitution Unifier { get; }

            /// <summary>
            /// Gets the steps of the proof. Each predicate used in the proof (including the goal) is present as a key.
            /// The associated value is the clause that was used to prove it. Each conjunct of that clause will also be present
            /// as a key - all the way back to and inclusive of the relevant unit clauses that are present in the KB (which
            /// of course have no conjuncts).
            /// </summary>
            public IReadOnlyDictionary<Predicate, CNFDefiniteClause> Steps => steps;

            internal Predicate ApplyUnifierTo(Predicate predicate) => Unifier.ApplyTo(predicate).Predicate;

            internal void AddStep(Predicate predicate, CNFDefiniteClause rule) => steps[predicate] = rule;
        }
    }
}
