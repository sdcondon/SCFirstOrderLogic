using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Interface for types that implement a resolution strategy that can
    /// be utilised by <see cref="ResolutionKnowledgeBase"/> instances.
    /// </summary>
    public interface IResolutionStrategy
    {
        /// <summary>
        /// Informs the strategy of a new clause added to the knowledge base.
        /// </summary>
        /// <param name="clause">The clause that has been added to the knowledge base.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns></returns>
        Task<bool> AddClauseAsync(CNFClause clause, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a strategy for use by a particular query.
        /// </summary>
        /// <param name="query">The query to create a strategy object for.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A (task representing the creation of a) resolution strategy for use by the given query.</returns>
        Task<IResolutionQueryStrategy> MakeQueryStrategyAsync(ResolutionQuery query, CancellationToken cancellationToken);
    }
}
