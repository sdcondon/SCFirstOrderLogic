// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
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
        /// <para>
        /// Returns all possible resolutions of a given clause with some clause in the store.
        /// </para>
        /// <para>
        /// NB: store implementations might limit what resolutions are "possible", depending
        /// on the resolution strategy they implement.
        /// </para>
        /// </summary>
        /// <param name="clause">The clause to find resolutions for.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>An enumerable of resolutions of the given clause with a clause in the store.</returns>
        IAsyncEnumerable<ClauseResolution> FindResolutions(CNFClause clause, CancellationToken cancellationToken = default);
    }
}
