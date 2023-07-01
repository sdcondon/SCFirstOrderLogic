// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.InternalUtilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// <para>
    /// A basic resolution strategy that just filters and prioritises clause resolutions using given delegates.
    /// </para>
    /// <para>
    /// Notes:
    /// </para>
    /// <list type="bullet">
    /// <item/>Has no in-built handling of equality - so, if equality appears in the knowledge base,
    /// its properties need to be axiomised - see §9.5.5 of 'Artifical Intelligence: A Modern Approach'.
    /// </list>
    /// </summary>
    public class DelegateResolutionStrategy : IResolutionStrategy
    {
        private readonly IKnowledgeBaseClauseStore clauseStore;
        private readonly Func<ClauseResolution, bool> filter;
        private readonly Comparison<ClauseResolution> priorityComparison;
        
        /// <summary>
        /// Initialises a new instance of the <see cref="DelegateResolutionStrategy"/> class.
        /// </summary>
        /// <param name="clauseStore">The clause store to use.</param>
        /// <param name="filter">
        /// A delegate to use to filter the pairs of clauses to be queued for a resolution attempt.
        /// A true value indicates that the pair should be enqueued. See the <see cref="Filters"/>
        /// inner class for some useful examples.
        /// </param>
        /// <param name="priorityComparison">
        /// A delegate to use to compare the pairs of clauses to be queued for a resolution attempt.
        /// See the <see cref="PriorityComparisons"/> inner class for some useful examples.
        /// </param>
        public DelegateResolutionStrategy(
            IKnowledgeBaseClauseStore clauseStore,
            Func<ClauseResolution, bool> filter,
            Comparison<ClauseResolution> priorityComparison)
        {
            this.clauseStore = clauseStore;
            this.filter = filter;
            this.priorityComparison = priorityComparison;
        }

        /// <inheritdoc/>
        public Task<bool> AddClauseAsync(CNFClause clause, CancellationToken cancellationToken)
        {
            return clauseStore.AddAsync(clause, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IResolutionQueryStrategy> MakeQueryStrategyAsync(ResolutionQuery query, CancellationToken cancellationToken)
        {
            return new QueryStrategy(query, await clauseStore.CreateQueryStoreAsync(cancellationToken), filter, priorityComparison);
        }

        /// <summary>
        /// <para>
        /// Useful resolution filter delegates for use with <see cref="DelegateResolutionStrategy"/>.
        /// </para>
        /// <para>
        /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
        /// </para>
        /// </summary>
        public static class Filters
        {
            /// <summary>
            /// "Filter" that doesn't actually filter out any pairings.
            /// </summary>
            public static Func<ClauseResolution, bool> None { get; } = pair => true;

            /// <summary>
            /// Filter that requires that one of the clauses be a unit clause.
            /// </summary>
            public static Func<ClauseResolution, bool> UnitResolution { get; } = pair => pair.Clause1.IsUnitClause || pair.Clause2.IsUnitClause;
        }

        /// <summary>
        /// <para>
        /// Useful priority comparisons for use with <see cref="DelegateResolutionStrategy"/>.
        /// </para>
        /// <para>
        /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
        /// </para>
        /// </summary>
        public static class PriorityComparisons
        {
            /// <summary>
            /// Naive comparison that orders clause pairs consistently but not according to any particular algorithm.
            /// </summary>
            // NB: While a comparison that just returns zero all the time might seem intuitive here,
            // such a comparison would cause our priority queue to behave more like a stack (see its code - noting
            // that it sensibly doesn't bubble in either direction when the comparison is 0). It'd of course be faster,
            // but I suspect the resulting "depth-first" behaviour of the resolution would be hands-down unhelpful.
            // TODO-ZZ-PERFORMANCE: Better to allow for the strategy to use a regular queue, not a heap-based one - e.g if a ctor overload
            // that simply doesn't have a comparison parameter is invoked. Or of course a different strategy class entirely.
            public static Comparison<ClauseResolution> None { get; } = (x, y) =>
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            };

            /// <summary>
            /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
            /// </summary>
            public static Comparison<ClauseResolution> UnitPreference { get; } = (x, y) =>
            {
                var xHasUnitClause = x.Clause1.IsUnitClause || x.Clause2.IsUnitClause;
                var yHasUnitClause = y.Clause1.IsUnitClause || y.Clause2.IsUnitClause;

                if (xHasUnitClause && !yHasUnitClause)
                {
                    return 1;
                }
                else if (!xHasUnitClause && yHasUnitClause)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            };

            /// <summary>
            /// <para>
            /// Comparison that gives priority to pairs with a lower total literal count.
            /// </para>
            /// <para>
            /// Not mentioned in source material, but I figured it was a logical variant of unit preference.
            /// </para>
            /// </summary>
            public static Comparison<ClauseResolution> TotalLiteralCountMinimisation { get; } = (x, y) =>
            {
                var xTotalClauseCount = x.Clause1.Literals.Count + x.Clause2.Literals.Count;
                var yTotalClauseCount = y.Clause1.Literals.Count + y.Clause2.Literals.Count;

                if (xTotalClauseCount < yTotalClauseCount)
                {
                    return 1;
                }
                else if (xTotalClauseCount > yTotalClauseCount)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            };
        }

        /// <summary>
        /// The strategy implementation for individual queries.
        /// </summary>
        private class QueryStrategy : IResolutionQueryStrategy
        {
            private readonly ResolutionQuery query;
            private readonly IQueryClauseStore clauseStore;
            private readonly Func<ClauseResolution, bool> filter;
            private readonly MaxPriorityQueue<ClauseResolution> priorityQueue;

            public QueryStrategy(
                ResolutionQuery query,
                IQueryClauseStore clauseStore,
                Func<ClauseResolution, bool> filter,
                Comparison<ClauseResolution> priorityComparison)
            {
                this.query = query;
                this.clauseStore = clauseStore;
                this.filter = filter;
                this.priorityQueue = new MaxPriorityQueue<ClauseResolution>(priorityComparison);
            }

            public bool IsQueueEmpty => priorityQueue.Count == 0;

            public ClauseResolution DequeueResolution() => priorityQueue.Dequeue();

            public void Dispose() => clauseStore.Dispose();

            public async Task EnqueueInitialResolutionsAsync(CancellationToken cancellationToken)
            {
                // Initialise the query clause store with the clauses from the negation of the query:
                foreach (var clause in query.NegatedQuerySentence.Clauses)
                {
                    await clauseStore.AddAsync(clause, cancellationToken);
                }

                // Queue up initial clause pairings:
                // TODO-PERFORMANCE: potentially repeating a lot of work here. Some of this (i.e. clause pairings that are just
                // from the KB - not from the negated query sentence) will be repeated every query, and some will produce the same
                // resolvent as each other - note that we don't filter out such dupes. Some caching here would be useful - is this
                // in scope for this *simple* implementation?
                await foreach (var clause in clauseStore)
                {
                    await foreach (var resolution in clauseStore.FindResolutions(clause, cancellationToken))
                    {
                        // NB: Throwing away clauses returned by (an arbitrary) clause store obviously has a performance impact.
                        // Better to use a store that knows to not look for certain clause pairings in the first place.
                        // However, the purpose of this strategy implementation is demonstration, not performance, so this is fine.
                        if (filter(resolution))
                        {
                            priorityQueue.Enqueue(resolution);
                        }
                    }
                }
            }

            public async Task EnqueueResolutionsAsync(CNFClause clause, CancellationToken cancellationToken)
            {
                // Check if we've found a new clause (i.e. something that we didn't know already).
                // NB: Upside of using Add to implicitly check for existence: it means we don't need a separate "Contains" method
                // on the store (which would raise potential misunderstandings about what the store means by "contains" - c.f. subsumption..)
                // Downside of using Add: clause store will encounter itself when looking for unifiers - not a big deal,
                // but a performance/maintainability tradeoff nonetheless.
                if (await clauseStore.AddAsync(clause, cancellationToken))
                {
                    // This is a new clause, so find and queue up its resolutions.
                    await foreach (var newResolution in clauseStore.FindResolutions(clause, cancellationToken))
                    {
                        // NB: Throwing away clauses returned by (an arbitrary) clause store obviously has a performance impact.
                        // Better to use a store that knows to not look for certain clause pairings in the first place.
                        // However, the purpose of this strategy implementation is to be a basic example, not a high performance one, so this is fine.
                        if (filter(newResolution))
                        {
                            priorityQueue.Enqueue(newResolution);
                        }
                    }
                }
            }
        }
    }
}
