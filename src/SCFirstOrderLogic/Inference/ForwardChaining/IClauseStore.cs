using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// <para>
    /// Interface for types that facilitate the storage and retrieval of CNF definite clauses
    /// for the purposes of forward chaining.
    /// </para>
    /// <para>
    /// NB: Clause storage and retrieval is necessary during the chaining process. We have an abstraction for it because
    /// knowledge bases of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// </para>
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
