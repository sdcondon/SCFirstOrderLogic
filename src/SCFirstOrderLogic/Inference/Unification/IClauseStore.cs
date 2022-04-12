using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Interface for types that facilitate the storage of CNF clauses, for later lookup of all of stored clauses
    /// that a given clause unifies with. See §9.2.3 of 'Artificial Intelligence: A Modern Approach' for a little
    /// more information on storage and retrieval.
    /// <para/>
    /// NB: This is functionality that underpins first-order logic inference algorithms. We have an abstraction for it because
    /// knowledge bases of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// </summary>
    public interface IClauseStore
    {
        /// <summary>
        /// Stores a clause.
        /// </summary>
        /// <param name="clause">The clause to store.</param>
        Task StoreAsync(CNFClause clause);

        /// <summary>
        /// Returns all unifiers such that the given clause unifies with some clause in the store.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindUnifiers(CNFClause clause);
    }
}
