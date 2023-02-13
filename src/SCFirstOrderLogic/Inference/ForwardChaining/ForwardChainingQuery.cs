using SCFirstOrderLogic;
using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IQuery"/> that uses a breadth-first, incremental forward chaining algorithm.
    /// Used by <see cref="ForwardChainingKnowledgeBase"/>.
    /// </summary>
    public sealed class ForwardChainingQuery : IQuery
    {
        private readonly Predicate queryGoal;
        private readonly IQueryClauseStore clauseStore;
        private readonly Dictionary<Predicate, ForwardChainingProofStep> proof = new();
        private readonly Lazy<ReadOnlyCollection<Predicate>> usefulPredicates;

        private int executeCount = 0;
        private bool? result;

        internal ForwardChainingQuery(Predicate queryGoal, IQueryClauseStore clauseStore)
        {
            this.queryGoal = queryGoal;
            this.clauseStore = clauseStore;
            this.usefulPredicates = new Lazy<ReadOnlyCollection<Predicate>>(MakeUsefulPredicates);
        }

        /// <inheritdoc />
        public bool IsComplete => result.HasValue;

        /// <inheritdoc />
        public bool Result => result ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a (very raw, English) explanation of the steps that led to the result of the query.
        /// </summary>
        public string ResultExplanation => GetResultExplanation(new SentenceFormatter());

        /// <summary>
        /// Gets the proof tree generated during execution of the query. Each discovered fact (not including those from the knowledge
        /// base) is present as a key. The value associated with each is information about the step used to discover it.
        /// </summary>
        public IReadOnlyDictionary<Predicate, ForwardChainingProofStep> Proof => proof;

        /// <summary>
        /// Gets a list of the useful (in that they led to the result) predicates discovered by a query that has returned a positive result.
        /// Starts with predicates that were discovered using only predicates from the knowledge base, and ends with the goal predicate.
        /// </summary>
        /// <returns>A list of the discovered predicates of the given query.</returns>
        /// <exception cref="InvalidOperationException">The query is not complete, or returned a negative result.</exception>
        public ReadOnlyCollection<Predicate> UsefulPredicates => usefulPredicates.Value;

        /// <inheritdoc />
        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // ..while it might be nice to allow for other threads to just get the existing task back
            // if its already been started, the possibility of the cancellation token being different
            // makes it awkward. The complexity added by dealing with that simply isn't worth it.
            // So, we just throw if the query is already in progress. Messing about with a query from
            // multiple threads is fairly unlikely anyway (as opposed wanting an individual query to
            // parallelise itself - which is definitely something I want to look at).
            if (Interlocked.Exchange(ref executeCount, 1) == 1)
            {
                throw new InvalidOperationException("Query execution has already begun via a prior ExecuteAsync invocation");
            }

            // First, quickly check if we've been asked something that is in the KB directly.
            // Otherwise, we only check against the goal when we discover something we didn't already know.
            // This means that if we didn't do a quick check here, asking the KB something that 
            // is in the KB as-is wouldn't work..
            if (await clauseStore.MatchWithKnownFacts(queryGoal, new VariableSubstitution(), cancellationToken).AnyAsync(cancellationToken))
            {
                result = true;
                return true;
            }

            // This query does incremental chaining - at each step, only the rules that
            // are applicable to newly discovered facts are considered. This means that prior to
            // the first step, we need to populate the "new" list with all of the facts from the KB.
            List<CNFDefiniteClause> newFacts = (await clauseStore.ToListAsync(cancellationToken)).Where(c => c.IsUnitClause).ToList();

            do
            {
                var applicableRules = await clauseStore.GetApplicableRules(newFacts.Select(f => f.Consequent), cancellationToken).ToListAsync(cancellationToken);
                newFacts.Clear();

                foreach (var rule in applicableRules)
                {
                    // NB: we don't need the variable standardisation line from the book because that's already
                    // happened as part of the conversion to CNF that is carried out when the KB is told things.

                    await foreach (var proofStep in MatchWithKnownFacts(rule))
                    {
                        // Need the inferred predicate as a clause - worth looking into a bit more type fluidity at some point
                        // so that this line is unecessary
                        var inferredClause = new CNFDefiniteClause(proofStep.InferredPredicate);

                        // If we have a new conclusion..    
                        if (!await clauseStore.MatchWithKnownFacts(inferredClause.Consequent, new VariableSubstitution(), cancellationToken).AnyAsync(cancellationToken) && !inferredClause.UnifiesWithAnyOf(newFacts))
                        {
                            // ..add it to the proof tree
                            proof[proofStep.InferredPredicate] = proofStep;

                            // .. check if it is the goal
                            if (LiteralUnifier.TryCreate(inferredClause.Consequent, queryGoal, out var _))
                            {
                                result = true;
                                return true;
                            }

                            // ..and to the list of new conclusions of this iteration..
                            newFacts.Add(inferredClause);
                        }
                    }
                }

                foreach (var newFact in newFacts)
                {
                    await clauseStore.AddAsync(newFact, cancellationToken);
                }
            }
            while (newFacts.Count > 0);

            result = false;
            return false;
        }

        /// <summary>
        /// Gets a (very raw, English) explanation of the steps that led to the result of the query, using a specified <see cref="SentenceFormatter"/>.
        /// </summary>
        /// <param name="formatter">The sentence formatter to use.</param>
        public string GetResultExplanation(SentenceFormatter formatter)
        {
            if (!IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            var cnfExplainer = new CNFExplainer(formatter);
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
                explanation.AppendLine($"     By Rule : {formatter.Format(proofStep.Rule)}");

                foreach (var knownUnitClause in proofStep.KnownPredicates)
                {
                    explanation.AppendLine($"     From {GetSource(knownUnitClause)}: {formatter.Format(knownUnitClause)}");
                }

                explanation.Append("     Using   : {");
                explanation.Append(string.Join(", ", proofStep.Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                explanation.AppendLine("}");

                foreach (var term in CNFInspector.FindNormalisationTerms(proofStep.KnownPredicates.Append(UsefulPredicates[i])))
                {
                    explanation.AppendLine($"     ..where {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                }

                explanation.AppendLine();
            }

            return explanation.ToString();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            clauseStore.Dispose();
        }

        private IAsyncEnumerable<ForwardChainingProofStep> MatchWithKnownFacts(CNFDefiniteClause rule)
        {
            // NB: there's no specific conjunct ordering here - just look at them in the order they happen to fall.
            // To improve performance we could at least TRY to order the conjuncts in a way that minimises
            // the amount of work we have to do. And this is where we'd do it.
            // TODO-FEATURE: would be relatively easy to add an (optional) ctor parameter for controlling 
            // conjunct ordering.
            return MatchWithKnownFacts(rule.Conjuncts, new ForwardChainingProofStep(rule));
        }

        private async IAsyncEnumerable<ForwardChainingProofStep> MatchWithKnownFacts(
            IEnumerable<Predicate> conjuncts,
            ForwardChainingProofStep proofStep)
        {
            if (!conjuncts.Any())
            {
                yield return proofStep;
            }
            else
            {
                await foreach (var (knownFact, unifier) in clauseStore.MatchWithKnownFacts(conjuncts.First(), proofStep.Unifier))
                {
                    await foreach (var restOfConjunctsProof in MatchWithKnownFacts(conjuncts.Skip(1), new ForwardChainingProofStep(proofStep, knownFact, unifier)))
                    {
                        yield return restOfConjunctsProof;
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
            var queue = new Queue<Predicate>(new[] { queryGoal });
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
    }
}
