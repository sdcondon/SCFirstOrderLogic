using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Interface for types that facilitate the storage of CNF clauses, for later lookup of all of stored clauses
    /// that a given clause unifies with. See §9.2.3 of 'Artificial Intelligence: A Modern Approach' ("Storage and Retrieval")
    /// for a little context.
    /// <para/>
    /// NB: Clause storage and retrieval is functionality that underpins first-order logic inference algorithms. We have an
    /// abstraction for it because knowledge bases of different sizes and natures will have different requirements for how
    /// the known clauses are stored (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably
    /// performant.
    /// </summary>
    public interface IClauseStore : IAsyncEnumerable<CNFClause>
    {
        /// <summary>
        /// Stores a clause - if it is not already present.
        /// </summary>
        /// <param name="clause">The clause to store.</param>
        /// <returns>True if the clause was added, false if it was already present.</returns>
        Task<bool> AddAsync(CNFClause clause);

        /// <summary>
        /// Returns all unifiers such that the given clause unifies with some clause in the store.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<(CNFClause otherClause, VariableSubstitution unifier, CNFClause unified)> FindUnifiers(CNFClause clause);
    }
}
