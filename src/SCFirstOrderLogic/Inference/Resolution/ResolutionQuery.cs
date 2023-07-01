// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
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
        private readonly Task strategyCreation;
        private readonly Dictionary<CNFClause, ClauseResolution> steps;
        private readonly Lazy<ReadOnlyCollection<CNFClause>> discoveredClauses;

        private IResolutionQueryStrategy? strategy;
        private bool? result;

        private ResolutionQuery(
            Sentence querySentence,
            IResolutionStrategy strategy,
            CancellationToken cancellationToken)
        {
            NegatedQuerySentence = new Negation(querySentence).ToCNF();
            strategyCreation = strategy.MakeQueryStrategyAsync(this, cancellationToken).ContinueWith(t => this.strategy = t.Result);
            steps = new();
            discoveredClauses = new(MakeDiscoveredClauses);
        }

        /// <summary>
        /// Gets the (CNF representation of the) negation of the query sentence.
        /// </summary>
        public CNFSentence NegatedQuerySentence { get; }
        
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
        public string ResultExplanation => GetResultExplanation(new SentenceFormatter());

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

            // NB: By awaiting here (as opposed to synchronising in the ctor itself),
            // we don't tie up a thread for any longer than we need to.
            await query.strategyCreation;

            await query.strategy!.EnqueueInitialResolutionsAsync(cancellationToken);

            if (query.strategy.IsQueueEmpty)
            {
                query.result = false;
            }

            return query;
        }

        /// <summary>
        /// Gets a human-readable explanation of the query result, using a specified <see cref="SentenceFormatter"/> instance.
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
                    else if (NegatedQuerySentence.Clauses.Contains(clause))
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

        /// <inheritdoc/>
        public override async Task<ClauseResolution> NextStepAsync(CancellationToken cancellationToken = default)
        {
            if (IsComplete)
            {
                throw new InvalidOperationException("Query is already complete");
            }

            var resolution = strategy!.DequeueResolution();

            // While we generally expect the strategies to be well-behaved enough to not
            // to repeat resolvents, checking if we've seen this (exact - leveraging subsumption
            // is the strategy's job) resolvent before is cheap and eliminates any performance
            // hit or proof tree confusion if the strategy behaves badly.
            if (steps.TryAdd(resolution.Resolvent, resolution))
            {
                if (resolution.Resolvent.Equals(CNFClause.Empty))
                {
                    result = true;
                    return resolution;
                }

                await strategy.EnqueueResolutionsAsync(resolution.Resolvent, cancellationToken);
            }

            if (strategy.IsQueueEmpty)
            {
                result = false;
            }

            return resolution;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            strategy!.Dispose();
            GC.SuppressFinalize(this);
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
