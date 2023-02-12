using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// <para>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// </para>
    /// <para>
    /// See §9.5 ("Resolution") of 'Artificial Intelligence: A Modern Approach' for a detailed explanation of resolution.
    /// Notes:
    /// </para>
    /// <list type="bullet">
    /// <item/>Has no in-built handling of equality - so, if equality appears in the knowledge base, its properties need to be axiomised - see §9.5.5 of 'Artifical Intelligence: A Modern Approach'.
    /// </list>
    /// </summary>
    public sealed partial class ResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly IResolutionStrategy strategy;

        /// <summary>
        /// Initialises a new instance of the <see cref="ResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="strategy">The resolution strategy to use.</param>
        public ResolutionKnowledgeBase(IResolutionStrategy strategy) => this.strategy = strategy;

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            foreach(var clause in sentence.ToCNF().Clauses)
            {
                await strategy.ClauseStore.AddAsync(clause, cancellationToken);
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
        /// <returns>A task that returns an <see cref="ResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public async Task<ResolutionQuery> CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            return await ResolutionQuery.CreateAsync(sentence, strategy, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="ResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public ResolutionQuery CreateQuery(Sentence sentence)
        {
            return CreateQueryAsync(sentence).GetAwaiter().GetResult();
        }
    }
}
