using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// See §9.5 ("Resolution") of 'Artificial Intelligence: A Modern Approach' for a detailed explanation of resolution.
    /// Notes:
    /// <list type="bullet">
    /// <item/>Includes functionality for fine-grained execution and examination of individual steps of queries. Use the <see cref="CreateQuery"/> method.
    /// <item/>Has no in-built handling of equality - so, if equality appears in the knowledge base, its properties need to be axiomised - see §9.5.5 of 'Artifical Intelligence: A Modern Approach'.
    /// <item/>Not thread-safe (i.e. not re-entrant) - despite the fact that resolution is ripe for parallelisation.
    /// </list>
    /// </summary>
    public sealed partial class SimpleResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBaseClauseStore clauseStore;
        private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
        private readonly Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clausePairFilter">
        /// A delegate to use to filter the pairs of clauses to be queued for a unification attempt.
        /// A true value indicates that the pair should be enqueued.
        /// </param>
        /// <param name="clausePairPriorityComparison">An delegate to use to compare the pairs of clauses to be queued for a unification attempt.</param>
        public SimpleResolutionKnowledgeBase(
            IKnowledgeBaseClauseStore clauseStore,
            Func<(CNFClause, CNFClause), bool> clausePairFilter,
            Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison)
        {
            this.clauseStore = clauseStore;
            this.clausePairFilter = clausePairFilter; 
            this.clausePairPriorityComparison = clausePairPriorityComparison;
        }

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            foreach(var clause in new CNFSentence(sentence).Clauses)
            {
                await clauseStore.AddAsync(clause, cancellationToken);
            }
        }

        /// <summary>
        /// Creates an <see cref="IResolutionQuery"/> instance for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>An <see cref="IResolutionQuery"/> instance that can be used to execute and examine the query.</returns>
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(sentence, cancellationToken);
        }

        /// <summary>
        /// Creates an <see cref="IResolutionQuery"/> instance for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="IResolutionQuery"/> instance that can be used to execute and examine the query.</returns>
        public async Task<SimpleResolutionQuery> CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            return await SimpleResolutionQuery.CreateAsync(clauseStore, clausePairFilter, clausePairPriorityComparison, sentence, cancellationToken);
        }
    }
}
