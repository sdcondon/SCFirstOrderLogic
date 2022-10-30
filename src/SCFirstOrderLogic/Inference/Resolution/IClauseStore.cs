using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Interface for types that facilitate the storage of CNF clauses, for later lookup of all of stored clauses
    /// that a given clause resolves with. See §9.2.3 of 'Artificial Intelligence: A Modern Approach' ("Storage and Retrieval")
    /// for a little context.
    /// <para/>
    /// NB: Clause storage and retrieval is functionality that underpins resolution. We have an abstraction for it because
    /// knowledge bases of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
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
