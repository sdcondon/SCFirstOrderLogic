using SCFirstOrderLogic.InternalUtilities;
using System;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Basic resolution strategy that filters and prioritises resolutions using given delegates.
    /// </summary>
    public class DelegateResolutionStrategy : ISimpleResolutionStrategy
    {
        private readonly Func<ClauseResolution, bool> filter;
        private readonly Comparison<ClauseResolution> priorityComparison;

        /// <summary>
        /// Initialises a new instance of the <see cref="DelegateResolutionStrategy"/> class.
        /// </summary>
        /// <param name="clauseStore">The clause store to use.</param>
        /// <param name="filter">
        /// A delegate to use to filter the pairs of clauses to be queued for a unification attempt.
        /// A true value indicates that the pair should be enqueued. See the <see cref="Filters"/>
        /// inner class for some useful examples.
        /// </param>
        /// <param name="priorityComparison">
        /// A delegate to use to compare the pairs of clauses to be queued for a unification attempt.
        /// See the <see cref="PriorityComparisons"/> inner class for some useful examples.
        /// </param>
        public DelegateResolutionStrategy(
            IKnowledgeBaseClauseStore clauseStore,
            Func<ClauseResolution, bool> filter,
            Comparison<ClauseResolution> priorityComparison)
        {
            ClauseStore = clauseStore;
            this.filter = filter;
            this.priorityComparison = priorityComparison;
        }

        /// <inheritdoc/>
        public IKnowledgeBaseClauseStore ClauseStore { get; }

        /// <inheritdoc/>
        public ISimpleResolutionQueue MakeResolutionQueue() => new PrioritisedAndFilteredResolutionQueue(filter, priorityComparison);

        /// <summary>
        /// <para>
        /// Useful resolution filter delegates. Might prove useful with, e.g. <see cref="DelegateResolutionStrategy"/>.
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
            public static Func<ClauseResolution, bool> UnitResolution { get; } = pair => pair.Clause1.Literals.Count == 1 || pair.Clause2.Literals.Count == 1;
        }

        /// <summary>
        /// <para>
        /// Useful priority comparisons for use by <see cref="DelegateResolutionStrategy"/>.
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
            // TODO-MINOR: Better to allow for the strategy to use a regular queue, not a heap-based one - e.g if a ctor overload
            // that simply doesn't have a comparison parameter is invoked. Or of course a different strategy class entirely.
            public static Comparison<ClauseResolution> None { get; } = (x, y) =>
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            };

            /// <summary>
            /// <para>
            /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
            /// </para>
            /// <para>
            /// NB: falls back on hash code comparison when not ordering because of unit clause presence. Given that
            /// some sentence things use reference equality (notably, symbols of standardised variables and Skolem functions),
            /// means that things can be ordered differently from one execution to the next. Not ideal..
            /// </para>
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
                    return x.GetHashCode().CompareTo(y.GetHashCode());
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
                    return x.GetHashCode().CompareTo(y.GetHashCode());
                }
            };
        }

        private class PrioritisedAndFilteredResolutionQueue : ISimpleResolutionQueue
        {
            private readonly Func<ClauseResolution, bool> filter;
            private readonly MaxPriorityQueue<ClauseResolution> priorityQueue;

            public PrioritisedAndFilteredResolutionQueue(
                Func<ClauseResolution, bool> filter,
                Comparison<ClauseResolution> priorityComparison)
            {
                this.filter = filter;
                priorityQueue = new(priorityComparison);
            }

            public bool IsEmpty => priorityQueue.Count == 0;

            public ClauseResolution Dequeue() => priorityQueue.Dequeue();

            public void Enqueue(ClauseResolution resolution)
            {
                // NB: Throwing away clauses returned by (an arbitrary) clause store has a performance impact.
                // Better to use a store that knows to not look for certain clause pairings in the first place.
                // However, the purpose of this strategy implementation is flexibility, not performance, so this is fine.
                if (filter(resolution))
                {
                    priorityQueue.Enqueue(resolution);
                }
            }
        }
    }
}
