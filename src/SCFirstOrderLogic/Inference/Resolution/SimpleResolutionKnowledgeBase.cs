using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// <para/>
    /// See §9.5 ("Resolution") of 'Artificial Intelligence: A Modern Approach' for a detailed explanation of resolution.
    /// Notes:
    /// <list type="bullet">
    /// <item/>Has no in-built handling of equality - so, if equality appears in the knowledge base, its properties need to be axiomised - see §9.5.5 of 'Artifical Intelligence: A Modern Approach'.
    /// </list>
    /// </summary>
    public sealed partial class SimpleResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBaseClauseStore clauseStore;
        private readonly Func<ClauseResolution, bool> filter;
        private readonly Comparison<ClauseResolution> priorityComparison;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clauseStore">The object to use to store clauses.</param>
        /// <param name="filter">
        /// A delegate to use to filter the pairs of clauses to be queued for a unification attempt.
        /// A true value indicates that the pair should be enqueued. See the <see cref="SimpleResolutionKnowledgeBase.Filters"/>
        /// inner class for some useful examples.
        /// </param>
        /// <param name="priorityComparison">
        /// A delegate to use to compare the pairs of clauses to be queued for a unification attempt.
        /// See the <see cref="SimpleResolutionKnowledgeBase.PriorityComparisons"/> inner class for some useful examples.
        /// </param>
        public SimpleResolutionKnowledgeBase(
            IKnowledgeBaseClauseStore clauseStore,
            Func<ClauseResolution, bool> filter,
            Comparison<ClauseResolution> priorityComparison)
        {
            this.clauseStore = clauseStore;
            this.filter = filter; 
            this.priorityComparison = priorityComparison;
        }

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            foreach(var clause in sentence.ToCNF().Clauses)
            {
                await clauseStore.AddAsync(clause, cancellationToken);
            }
        }

        /// <inheritdoc />
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(sentence, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task that returns an <see cref="SimpleResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public async Task<SimpleResolutionQuery> CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            return await SimpleResolutionQuery.CreateAsync(clauseStore, filter, priorityComparison, sentence, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="SimpleResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public SimpleResolutionQuery CreateQuery(Sentence sentence)
        {
            return CreateQueryAsync(sentence).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Useful filters for use by <see cref="SimpleResolutionKnowledgeBase"/>.
        /// <para/>
        /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
        /// </summary>
        public class Filters
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
        /// Useful priority comparisons for use by <see cref="SimpleResolutionKnowledgeBase"/>.
        /// <para/>
        /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
        /// </summary>
        public class PriorityComparisons
        {
            /// <summary>
            /// Naive comparison that orders clause pairs consistently but not according to any particular algorithm.
            /// </summary>
            public static Comparison<ClauseResolution> None { get; } = (x, y) =>
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            };

            /// <summary>
            /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
            /// <para/>
            /// NB: falls back on hash code comparison when not ordering because of unit clause presence. Given that
            /// some sentence things use reference equality (notably, symbols of standardised variables and Skolem functions),
            /// means that things can be ordered differently from one execution to the next. Not ideal..
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
            /// Comparison that gives priority to pairs with a lower total literal count.
            /// <para/>
            /// Not mentioned in source material, but I figured it was a logical variant of unit preference.
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
    }
}
