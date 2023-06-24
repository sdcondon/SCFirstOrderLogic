// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for implementations intended for storing the known clauses
    /// for a knowledge base as a whole. In particular, defines a method for creating a <see cref="IQueryClauseStore"/>
    /// for storing the intermediate clauses of an individual query.
    /// </summary>
    public interface IKnowledgeBaseClauseStore : IClauseStore
    {
        /// <summary>
        /// Creates a (disposable) copy of the current store, for storing the intermediate clauses of a particular query.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task, the result of which is a new <see cref="IQueryClauseStore"/> instance.</returns>
        Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken);
    }
}
