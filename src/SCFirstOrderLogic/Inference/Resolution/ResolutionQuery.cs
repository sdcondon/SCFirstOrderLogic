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
    /// Implementation of <see cref="IQuery"/> used by <see cref="ResolutionKnowledgeBase"/>.
    /// Encapsulates a query, allowing for step-by-step execution, as well as examination of those steps, as or after they are carried out.
    /// </summary>
    public class ResolutionQuery : SteppableQuery<ClauseResolution>
    {
        private readonly Task<IResolutionQueryStrategy> strategyCreation;
        private readonly Dictionary<CNFClause, ClauseResolution> steps;
        private readonly Lazy<ReadOnlyCollection<CNFClause>> discoveredClauses;

        private IResolutionQueryStrategy? strategy;
        private bool? result;

        private ResolutionQuery(
            Sentence querySentence,
            IResolutionStrategy strategy,
            CancellationToken cancellationToken)
        {
            NegatedQuery = new Negation(querySentence).ToCNF();
            strategyCreation = strategy.MakeQueryStrategyAsync(this, cancellationToken);
            steps = new();
            discoveredClauses = new(MakeDiscoveredClauses);
        }

        /// <summary>
        /// Gets the (CNF representation of) the negation of the query.
        /// </summary>
        public CNFSentence NegatedQuery { get; }
        
        /// <inheritdoc/>
        public override bool IsComplete => result.HasValue;

        /// <inheritdoc/>
        public override bool Result
        {
            get
            {
                if (!result.HasValue)
                {
                    throw new InvalidOperationException("Query is not yet complete");
                }

                return result.Value;
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

                    foreach (var term in CNFInspector.FindNormalisationTerms(DiscoveredClauses[i], resolution.Clause1, resolution.Clause2))
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
        /// Creates and initialises a new instance of the <see cref="ResolutionQuery"/> class. Initialisation can potentially
        /// be a long-running operation (and long-running constructors are a bad idea) - so the constructor is private and this method exists.
        /// </summary>
        /// <param name="querySentence">The query itself.</param>
        /// <param name="strategy">The resolution strategy to use.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A new query instance.</returns>
        public static async Task<ResolutionQuery> CreateAsync(
            Sentence querySentence,
            IResolutionStrategy strategy,
            CancellationToken cancellationToken = default)
        {
            var query = new ResolutionQuery(querySentence, strategy, cancellationToken);
            query.strategy = await query.strategyCreation;

            // Ask the strategy to enqueue the initial clause pairings for the query:
            await query.strategy.EnqueueInitialResolutionsAsync(cancellationToken);

            // Double-check that we actually managed to enqueue some initial clause pairings.
            // Mark the query as failed if not:
            if (query.strategy.IsQueueEmpty)
            {
                query.result = false;
            }

            return query;
        }

        /// <inheritdoc/>
        public override async Task<ClauseResolution> NextStepAsync(CancellationToken cancellationToken = default)
        {
            if (IsComplete)
            {
                throw new InvalidOperationException("Query is already complete");
            }

            // Ask the strategy for the next resolution from the queue.
            var resolution = strategy!.DequeueResolution();

            // If the resolvent is an empty clause, we've found a contradiction and can thus return a positive result:
            if (resolution.Resolvent.Equals(CNFClause.Empty))
            {
                result = true;
                return resolution;
            }

            // Ask the strategy to take a look at the resolvent and enqueue any new resolutions for it:
            await strategy.EnqueueResolutionsAsync(resolution.Resolvent, cancellationToken);

            // Ask the strategy if we've run out of resolutions. Mark the query as failed if so.
            if (strategy.IsQueueEmpty)
            {
                result = false;
            }

            // Finally, make a note of the latest resolution in the 'steps' field (for the proof tree), and return it.
            return steps[resolution.Resolvent] = resolution;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            strategy!.Dispose();
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
