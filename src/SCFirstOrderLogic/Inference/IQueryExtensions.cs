using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Helpful extension methods for <see cref="IQuery"/> instances.
    /// </summary>
    public static class IQueryExtensions
    {
        /// <summary>
        /// Continuously executes the next step of a query until it completes.
        /// </summary>
        /// <param name="query">The query to execute to completion.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>The result of the query.</returns>
        public static async Task<bool> CompleteAsync(this IQuery query, CancellationToken cancellationToken = default)
        {
            while (!query.IsComplete)
            {
                await query.NextStepAsync(cancellationToken);
            }

            return query.Result;
        }
    }
}
