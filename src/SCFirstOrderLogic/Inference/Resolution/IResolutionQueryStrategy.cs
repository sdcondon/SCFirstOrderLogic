﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Interface for types that implement a resolution strategy for a particular query.
    /// </summary>
    public interface IResolutionQueryStrategy : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the resolution queue is empty. That is, whether the query is exhausted.
        /// </summary>
        bool IsQueueEmpty { get; }

        /// <summary>
        /// Dequeues the next resolution for further exploration.
        /// </summary>
        /// <returns>The next resolution for further exploration.</returns>
        ClauseResolution DequeueResolution();

        /// <summary>
        /// Initialises the strategy by enqueuing the initial resolutions for a query. That is, enqueues all of the relevant pairings of the clauses in the knowledge base and negation of the query sentence.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task representing completion of the operation.</returns>
        Task EnqueueInitialResolutionsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue all of the relevant resolutions for a new intermediate clause.
        /// </summary>
        /// <param name="clause">The new clause to find and enqueue all of the resolutions for.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task representing completion of the operation.</returns>
        Task EnqueueResolutionsAsync(CNFClause clause, CancellationToken cancellationToken);
    }
}
