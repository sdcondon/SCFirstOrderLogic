using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceFormatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Implementation of <see cref="IQuery"/> used by <see cref="SimpleResolutionKnowledgeBase"/>.
    /// Encapsulates a query, allowing for step-by-step execution, as well as examination of those steps, as or after they are carried out.
    /// Notes:
    /// <list type="bullet">
    /// <item/>TODO: Not thread-safe (i.e. not re-entrant) - despite the fact that resolution is ripe for parallelisation.
    /// </list>  
    /// </summary>
    public class SimpleResolutionQuery : SteppableQuery<ClauseResolution>
    {
        private readonly IQueryClauseStore clauseStore;
        private readonly Func<ClauseResolution, bool> filter;
        private readonly MaxPriorityQueue<ClauseResolution> queue;
        private readonly Dictionary<CNFClause, ClauseResolution> steps;
        private readonly Lazy<ReadOnlyCollection<CNFClause>> discoveredClauses;

        private bool isComplete;
        private bool result;

        private SimpleResolutionQuery(
            IQueryClauseStore clauseStore,
            Func<ClauseResolution, bool> filter,
            Comparison<ClauseResolution> priorityComparison,
            Sentence querySentence)
        {
            this.clauseStore = clauseStore;
            this.filter = filter;
            queue = new MaxPriorityQueue<ClauseResolution>(priorityComparison);
            steps = new Dictionary<CNFClause, ClauseResolution>();
            discoveredClauses = new(MakeDiscoveredClauses);

            NegatedQuery = new Negation(querySentence).ToCNF();
        }

        /// <summary>
        /// Gets the (CNF representation of) the negation of the query.
        /// </summary>
        public CNFSentence NegatedQuery { get; }

        /// <inheritdoc/>
        public override bool IsComplete => isComplete;

        /// <inheritdoc/>
        public override bool Result
        {
            get
            {
                if (!IsComplete)
                {
                    throw new InvalidOperationException("Query is not yet complete");
                }

                return result;
            }
        }

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
                for (var i = 0; i < DiscoveredClauses.Count; i++)
                {
                    var resolution = Steps[DiscoveredClauses[i]];

                    string GetSource(CNFClause clause)
                    {
                        if (DiscoveredClauses.Contains(clause))
                        {
                            return $"#{DiscoveredClauses.IndexOf(clause):D2}";
                        }
                        else if (NegatedQuery.Clauses.Contains(clause))
                        {
                            return " ¬Q";
                        }
                        else
                        {
                            return " KB";
                        }
                    }

                    explanation.AppendLine($"#{i:D2}: {formatter.Format(DiscoveredClauses[i])}");
                    explanation.AppendLine($"     From {GetSource(resolution.Clause1)}: {formatter.Format(resolution.Clause1)}");
                    explanation.AppendLine($"     And  {GetSource(resolution.Clause2)}: {formatter.Format(resolution.Clause2)} ");
                    explanation.Append("     Using   : {");
                    explanation.Append(string.Join(", ", resolution.Substitution.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                    explanation.AppendLine("}");

                    foreach (var term in CNFExaminer.FindNormalisationTerms(DiscoveredClauses[i], resolution.Clause1, resolution.Clause2))
                    {
                        explanation.AppendLine($"     ..where {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                    }

                    explanation.AppendLine();
                }

                return explanation.ToString();
            }
        }

        /// <summary>
        /// Gets a list of the (useful) clauses discovered by a query that has returned a positive result.
        /// Starts with clauses that were discovered using only clauses from the knowledge base or negated
        /// query, and ends with the empty clause.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the query is not complete, or returned a negative result.</exception>
        public ReadOnlyCollection<CNFClause> DiscoveredClauses => discoveredClauses.Value;

        /// <summary>
        /// Gets a mapping from a clause to the resolution from which it was inferred.
        /// </summary>
        public IReadOnlyDictionary<CNFClause, ClauseResolution> Steps => steps;

        /// <summary>
        /// Creates and initialises a new instance of the <see cref="SimpleResolutionQuery"/> class. Initialisation can potentially
        /// be a long-running operation (and long-running constructors are a bad idea) - so the constructor is private and this method exists.
        /// </summary>
        /// <param name="clauseStore">A knowledge base clause store to use to create the clause store for this query.</param>
        /// <param name="filter">A filter to apply to clause pairings during this query.</param>
        /// <param name="priorityComparison">A comparison delegate to use to prioritise clause pairings during this query.</param>
        /// <param name="querySentence">The query itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A new query instance.</returns>
        internal static async Task<SimpleResolutionQuery> CreateAsync(
            IKnowledgeBaseClauseStore clauseStore,
            Func<ClauseResolution, bool> filter,
            Comparison<ClauseResolution> priorityComparison,
            Sentence querySentence,
            CancellationToken cancellationToken = default)
        {
            var queryClauseStore = await clauseStore.CreateQueryStoreAsync(cancellationToken);

            var query = new SimpleResolutionQuery(queryClauseStore, filter, priorityComparison, querySentence);

            // Initialise the query clause store with the clauses from the negation of the query:
            foreach (var clause in query.NegatedQuery.Clauses)
            {
                await queryClauseStore.AddAsync(clause, cancellationToken);
            }

            // Queue up all initial clause pairings - adhering to our clause pair filter and priority comparer.
            await foreach (var clause in queryClauseStore)
            {
                await query.EnqueueUnfilteredResolventsAsync(clause, cancellationToken);
            }

            if (query.queue.Count == 0)
            {
                query.result = false;
                query.isComplete = true;
            }

            return query;
        }

        /// <inheritdoc/>
        public override async Task<ClauseResolution> NextStepAsync(CancellationToken cancellationToken = default)
        {
            if (IsComplete)
            {
                throw new InvalidOperationException("Query is complete");
            }

            // Grab the next resolution (including the resolvent, the clauses it originates from, and the variable substitution) from the queue..
            var resolution = queue.Dequeue();
            steps[resolution.Resolvent] = resolution;

            // If the resolvent is an empty clause, we've found a contradiction and can thus return a positive result:
            if (resolution.Resolvent.Equals(CNFClause.Empty))
            {
                result = true;
                isComplete = true;
                return resolution;
            }

            // Otherwise, check if we've found a new clause (i.e. something that we didn't know already)..
            // Upside of using Add to implicitly check for existence: it means we don't need a separate "Contains" method
            // on the store (which would raise potential misunderstandings about what the store means by "contains" - c.f. subsumption..)
            // Downside of using Add: clause store will encounter itself when looking for unifiers - not a big deal,
            // but a performance/maintainability tradeoff nonetheless
            if (await clauseStore.AddAsync(resolution.Resolvent, cancellationToken))
            {
                // This is a new clause, so we queue up some more clause pairings -
                // (combinations of the resolvent and existing known clauses)
                // adhering to any filtering and ordering we have in place.

                // ..and iterate through its resolvents (if any) - also make a note of the unifier so that we can include it in the record of steps that we maintain:
                await EnqueueUnfilteredResolventsAsync(resolution.Resolvent, cancellationToken);
            }
            
            if (queue.Count == 0)
            {
                // We've run out of clauses to smash together - return a negative result.
                result = false;
                isComplete = true;
            }

            return resolution;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            clauseStore.Dispose();
        }

        private async Task EnqueueUnfilteredResolventsAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            await foreach (var resolution in clauseStore.FindResolutions(clause, cancellationToken))
            {
                // NB: Throwing away clauses returned by the unifier store has a performance impact.
                // Could instead/also use a store that knows to not look for certain clause pairings in the first place..
                // However, REQUIRING the store to do this felt a little ugly from a code perspective, since the store is
                // then a mix of implementation (how unifiers are stored/indexed) and strategy, plus there's a bit more
                // strategy in the form of the priority comparer. This feels a good compromise - there are of course
                // alternatives (e.g. some kind of strategy object that encapsulates both) - but they felt like overkill
                // for this *simple* implementation.
                if (filter(resolution))
                {
                    queue.Enqueue(resolution);
                }
            }
        }

        private ReadOnlyCollection<CNFClause> MakeDiscoveredClauses()
        {
            if (!IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            // Walk back through the DAG of clauses, starting from CNFClause.Empty, breadth-first:
            var orderedSteps = new List<CNFClause>();
            var queue = new Queue<CNFClause>(new[] { CNFClause.Empty });
            while (queue.Count > 0)
            {
                var clause = queue.Dequeue();
                if (orderedSteps.Contains(clause))
                {
                    // We have found the same clause "earlier" than another encounter of it.
                    // Remove the "later" one so that once we reverse the list, there are no
                    // references to clauses we've not seen yet.
                    orderedSteps.Remove(clause);
                }

                // If the clause is an intermediate one from the query (as opposed to one found
                // in the knowledge base or negated query)..
                if (Steps.TryGetValue(clause, out var resolution))
                {
                    // ..queue up the two clauses that resolved to give us this one..
                    queue.Enqueue(resolution.Clause1);
                    queue.Enqueue(resolution.Clause2);

                    // ..and record it in the steps list.
                    orderedSteps.Add(clause);
                }
            }

            // Reverse the steps encountered in the backward pass, to obtain a list of steps
            // contributing to the result, in (roughly..) the order in which query found them.
            orderedSteps.Reverse();

            return orderedSteps.AsReadOnly();
        }
    }
}
