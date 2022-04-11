#if FALSE
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Interface for types that facilitate lookup of all of the unifiers (i.e. sets of variable assignments) such
    /// that a given query unifies (i.e. matches, once the variable assignents are made) with some known clause.
    /// <para/>
    /// This is a process that underpins first-order logic inference algorithms. We have an abstraction for it because
    /// stores of different sizes and natures will have different requirements for how the known clauses are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// </summary>
    public interface IUnifierStore
    {
        /// <summary>
        /// Registers a clause with the store.
        /// </summary>
        /// <param name="clause">The clause to store.</param>
        Task StoreAsync(CNFClause clause);

        /// <summary>
        /// Returns all unifiers such that the given clause unifies with some clause in the store.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<IReadOnlyDictionary<VariableReference, Term>> Fetch(CNFClause clause);
    }
}
#endif
