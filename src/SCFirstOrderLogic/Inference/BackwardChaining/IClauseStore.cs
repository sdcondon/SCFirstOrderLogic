// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// <para>
    /// Interface for types that facilitate the storage of CNF definite clauses for the purposes of backward chaining.
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

        /// <summary>
        /// Returns all applications of a stored clause that result in a given consequent.
        /// </summary>
        /// <param name="goal">The goal - which the consequent term of the returned clauses should match (after transformation with the returned substitution).</param>
        /// <param name="constraints">The variable substitutions already made that must be respected.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>An enumerable of clause-substitution pairs, one for each possible clause application.</returns>
        IAsyncEnumerable<(CNFDefiniteClause Clause, VariableSubstitution Substitution)> GetClauseApplications(Predicate goal, VariableSubstitution constraints, CancellationToken cancellationToken = default);
    }
}
