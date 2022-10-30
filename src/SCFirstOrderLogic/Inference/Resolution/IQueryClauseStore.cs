using System;
using System.Collections.Generic;
using System.Threading;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for types intended for storing the intermediate clauses of a particular query.
    /// Notably, is <see cref="IDisposable"/> so that any unmanaged resources can be promptly tidied away once the query is complete.
    /// </summary>
    public interface IQueryClauseStore : IClauseStore, IDisposable
    {
        /// <summary>
        /// Returns all possible resolutions of a given clause with some clause in the store.
        /// </summary>
        /// <param name="clause">The clause to find resolutions for.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns></returns>
        IAsyncEnumerable<ClauseResolution> FindResolutions(CNFClause clause, CancellationToken cancellationToken = default);
    }
}
