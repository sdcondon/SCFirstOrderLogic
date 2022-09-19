using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Query implementation used by <see cref="SimpleForwardChainingKnowledgeBase"/>.
    /// </summary>
    public sealed class SimpleForwardChainingQuery : IQuery
    {
        private readonly Predicate α;
        private readonly List<CNFDefiniteClause> kb;
        private readonly Dictionary<Predicate, ProofStep> proof = new();
        private readonly Lazy<ReadOnlyCollection<Predicate>> usefulPredicates;

        private bool? result;

        internal SimpleForwardChainingQuery(Predicate α, List<CNFDefiniteClause> clauses)
        {
            this.α = α;
            this.kb = new List<CNFDefiniteClause>(clauses);
            this.usefulPredicates = new Lazy<ReadOnlyCollection<Predicate>>(MakeUsefulPredicates);
        }

        /// <inheritdoc />
        public bool IsComplete => result.HasValue;

        /// <inheritdoc />
        public bool Result => result ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a (very raw) explanation of the steps that led to the result of the query.
        /// </summary>
        public string ResultExplanation
        {
            get
            {
                if (!IsComplete)
                {
                    throw new InvalidOperationException("Query is not yet complete");
                }
                else if (!Result)
                {
                    throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
                }

                var formatter = new SentenceFormatter();
                var cnfExplainer = new CNFExplainer(formatter);

                // Now build the explanation string.
                var explanation = new StringBuilder();
                for (var i = 0; i < UsefulPredicates.Count; i++)
                {
                    var proofStep = Proof[UsefulPredicates[i]];

                    string GetSource(Predicate predicate)
                    {
                        if (UsefulPredicates.Contains(predicate))
                        {
                            return $"#{UsefulPredicates.IndexOf(predicate):D2}";
                        }
                        else
                        {
                            return " KB";
                        }
                    }

                    explanation.AppendLine($"#{i:D2}: {formatter.Format(UsefulPredicates[i])}");
                    explanation.AppendLine($"     By Rule : {proofStep.Rule.Format(formatter)}");

                    foreach (var knownUnitClause in proofStep.KnownPredicates)
                    {
                        explanation.AppendLine($"     From {GetSource(knownUnitClause)}: {formatter.Format(knownUnitClause)}");
                    }

                    explanation.Append("     Using   : {");
                    explanation.Append(string.Join(", ", proofStep.Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                    explanation.AppendLine("}");

                    // TODO: Ugh, unreadable. More type fluidity. Or just more overloads of FindMormalisationTerms..
                    foreach (var term in CNFExplainer.FindNormalisationTerms(proofStep.KnownPredicates.Select(p => new CNFClause(new CNFLiteral[] { p })).Append(new CNFClause(new CNFLiteral[] { UsefulPredicates[i] })).ToArray()))
                    {
                        explanation.AppendLine($"     ..where {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                    }

                    explanation.AppendLine();
                }

                return explanation.ToString();
            }
        }

        /// <summary>
        /// Gets the proof tree generated during execution of the query. Each discovered fact (not including those from the knowledge
        /// base) is present as a key. The value associated with each is information about the step used to discover it.
        /// </summary>
        public IReadOnlyDictionary<Predicate, ProofStep> Proof => proof;

        /// <summary>
        /// Gets a list of the useful (in that they led to the result) predicates discovered by a query that has returned a positive result.
        /// Starts with predicates that were discovered using only predicates from the knowledge base, and ends with the goal predicate.
        /// </summary>
        /// <returns>A list of the discovered predicates of the given query.</returns>
        /// <exception cref="InvalidOperationException">The query is not complete, or returned a negative result.</exception>
        public ReadOnlyCollection<Predicate> UsefulPredicates => usefulPredicates.Value;

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var @new = new List<CNFDefiniteClause>();

            do
            {
                @new.Clear();

                foreach (var fact in kb)
                {
                    // First check if we've found our goal
                    if (fact.IsUnitClause && LiteralUnifier.TryCreate(fact.Consequent, α, out var φ))
                    {
                        result = true;
                        return Task.FromResult(true);
                    }

                    // NB: we don't need the variable standardisation line from the book because that's already
                    // happened as part of the conversion to CNF that is carried out when the KB is told things.
                    // So all we do is call the consequent 'q' to match the book listing:
                    var q = fact.Consequent;

                    foreach (var proofStep in MatchWithKnownFacts(fact))
                    {
                        // Need the inferred predicate as a clause - worth looking into a bit more type fluidity at some point
                        // so that this line is unecessary
                        var inferredClause = new CNFDefiniteClause(proofStep.InferredPredicate);

                        // If we have a new conclusion..
                        if (!inferredClause.UnifiesWithAnyOf(kb) && !inferredClause.UnifiesWithAnyOf(@new))
                        {
                            // ..add it to the proof tree
                            proof[proofStep.InferredPredicate] = proofStep;
                            
                            // ..and to the list of new conclusions of this iteration..
                            @new.Add(inferredClause);
                        }
                    }
                }
 
                kb.AddRange(@new);
            }
            while (@new.Count > 0);

            result = false;
            return Task.FromResult(false);
        }

        /// <summary>
        /// Finds the variable substitutions t such that t.ApplyTo(clause) = t.ApplyTo(p1 ∧ ... ∧ pₙ) for some p1, .., pₙ in KB
        /// </summary>
        private IEnumerable<ProofStep> MatchWithKnownFacts(CNFDefiniteClause rule)
        {
            // NB: no specific conjunct ordering here - just look at them in the order they happen to fall.
            // In a production scenario, we'd at least TRY to order the conjuncts in a way that minimises
            // the amount of work we have to do. And this is where we'd do it.
            return MatchWithKnownFacts(rule.Conjuncts, new ProofStep(rule));
        }

        // I'm not a huge fan of recursion when trying to write "learning" code (because its not particularly readable) - but I'll admit it is handy here.
        // May revisit this - perhaps when I try to make the whole thing executable step-by-step for greater observability - can step through without debugging).
        private IEnumerable<ProofStep> MatchWithKnownFacts(IEnumerable<Predicate> conjuncts, ProofStep proofStep)
        {
            if (!conjuncts.Any())
            {
                yield return proofStep;
            }
            else
            {
                // Here we just iterate through ALL known predicates trying to find something that unifies with the first conjunct.
                // We'd use an index here in anything approaching a production scenario:
                foreach (var knownUnitClause in kb.Where(k => k.IsUnitClause))
                {
                    var updatedUnifier = new VariableSubstitution(proofStep.Unifier);

                    if (LiteralUnifier.TryUpdateUnsafe(knownUnitClause.Consequent, conjuncts.First(), updatedUnifier))
                    {
                        foreach (var substitution in MatchWithKnownFacts(conjuncts.Skip(1), new ProofStep(proofStep, knownUnitClause.Consequent, updatedUnifier)))
                        {
                            yield return substitution;
                        }
                    }
                }
            }
        }

        private ReadOnlyCollection<Predicate> MakeUsefulPredicates()
        {
            if (!IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            // Walk back through the DAG of predicates, starting from the goal, breadth-first:
            var orderedSteps = new List<Predicate>();
            var queue = new Queue<Predicate>(new[] { α });
            while (queue.Count > 0)
            {
                var predicate = queue.Dequeue();
                if (orderedSteps.Contains(predicate))
                {
                    // We have found the same predicate "earlier" than another encounter of it.
                    // Remove the "later" one so that once we reverse the list, there are no
                    // references to predicates we've not seen yet.
                    orderedSteps.Remove(predicate);
                }

                // If the predicate is an intermediate one from the query (as opposed to one found in the knowledge base)..
                if (Proof.TryGetValue(predicate, out var proofStep))
                {
                    // ..queue up the predicates that were used to give us this one..
                    foreach (var knownPredicate in proofStep.KnownPredicates)
                    {
                        queue.Enqueue(knownPredicate);
                    }

                    // ..and record it in the steps list.
                    orderedSteps.Add(predicate);
                }
            }

            // Reverse the steps encountered in the backward pass, to obtain a list of steps
            // contributing to the result, in (roughly..) the order in which query found them.
            orderedSteps.Reverse();

            return orderedSteps.AsReadOnly();
        }

        /// <summary>
        /// Container for information about an attempt to apply a specific rule from the knowledge base, given what we've already discerned.
        /// </summary>
        public class ProofStep
        {
            internal ProofStep(CNFDefiniteClause rule)
            {
                Rule = rule;
                KnownPredicates = Enumerable.Empty<Predicate>();
                Unifier = new VariableSubstitution();
            }

            /// <summary>
            /// Extends a proof step with an additional predicate and updated unifier.
            /// </summary>
            /// <param name="parent">The existing proof step.</param>
            /// <param name="additionalPredicate">The predicate to add.</param>
            /// <param name="updatedUnifier">The updated unifier.</param>
            internal ProofStep(ProofStep parent, Predicate additionalPredicate, VariableSubstitution updatedUnifier)
            {
                Rule = parent.Rule;
                KnownPredicates = parent.KnownPredicates.Append(additionalPredicate); // Hmm. Nesting.. Though we can probably realise it lazily, given the usage.
                Unifier = updatedUnifier;
            }

            /// <summary>
            /// The rule that was applied by this step.
            /// </summary>
            public CNFDefiniteClause Rule { get; }

            /// <summary>
            /// The known predicates that were used to make this step.
            /// </summary>
            public IEnumerable<Predicate> KnownPredicates { get; }

            /// <summary>
            /// The substitution that is applied to the rule's conjuncts to make them match the known predicates.
            /// <para/>
            /// TODO-BREAKING: Make VariableSubstitution readonly, and add MutableVariableSubstitution.
            /// </summary>
            public VariableSubstitution Unifier { get; }

            /// <summary>
            /// Gets the predicate that was inferred by this step by the application of the rule to the known unit clauses.
            /// </summary>
            public Predicate InferredPredicate => Unifier.ApplyTo(Rule.Consequent).Predicate;
        }
    }
}
