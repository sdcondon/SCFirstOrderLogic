// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// <para>
    /// Interface for types that facilitate the storage of CNF clauses for the purposes of resolution.
    /// </para>
    /// <para>
    /// NB: Clause storage and retrieval is functionality that underpins resolution. We have an abstraction for it because
    /// knowledge bases of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// See §9.2.3 of 'Artificial Intelligence: A Modern Approach' ("Storage and Retrieval") for a little more on clause storage.
    /// </para>
    /// </summary>
    public interface IClauseStore : IAsyncEnumerable<CNFClause>
    {
        /// <summary>
        /// Stores a clause - if it is not already present.
        /// </summary>
        /// <param name="clause">The clause to store.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>True if the clause was added, false if it was already present.</returns>
        Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default);
    }
}
