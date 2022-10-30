using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// Interface for types that facilitate the storage of CNF definite clauses, for later lookup of all clauses
    /// that could theoretically be used to prove a given predicate (because they have a consequent that unifies with it).
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

        /// <summary>
        /// Returns all possible applications of a clause in the knowledge base
        /// </summary>
        /// <param name="goal">The goal - which the consequent term of the returned clauses should match (after transformation with the returned substitution).</param>
        /// <param name="constraints">
        /// The variable substitutions already made.
        /// <para/>
        /// TODO-PRE-V3: Not strictly necessary - but omitting it as-is would make for potentially confusing proof tree details.
        /// A higher level concern, really - but would mean needing to apply existing sub before calling this method.
        /// eliminate me from here if application of existing sub before this method is called proves not to be too much of a performance burden.
        /// </param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns></returns>
        IAsyncEnumerable<(CNFDefiniteClause Clause, VariableSubstitution Substitution)> GetClauseApplications(Predicate goal, VariableSubstitution constraints, CancellationToken cancellationToken = default);
    }
}
