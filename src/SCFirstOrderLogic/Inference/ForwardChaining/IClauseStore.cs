using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// TODO: Interface for types that facilitate the storage of CNF definite clauses, for later lookup of all clauses
    /// that could ..
    /// <para/>
    /// NB: Clause storage and retrieval is necessary during the chaining process. We have an abstraction for it because
    /// knowledge bases of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// </summary>
    public interface IClauseStore : IAsyncEnumerable<CNFDefiniteClause>
    {
        /// <summary>
        /// Stores a clause - if it is not already present.
        /// </summary>
        /// <param name="clause">The clause to store.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>True if the clause was added, false if it was already present.</returns>
        Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default);
    }
}
