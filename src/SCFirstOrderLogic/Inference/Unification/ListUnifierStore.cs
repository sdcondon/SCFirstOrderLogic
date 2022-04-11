#if FALSE
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Basic implementation of <see cref="IUnifierStore"/> that just maintains all known sentences in a <see cref="List{T}"./>
    /// </summary>
    public class ListUnifierStore : IUnifierStore
    {
        private readonly List<CNFClause> clauses = new List<CNFClause>();

        /// <inheritdoc />
        public Task StoreAsync(CNFClause clause)
        {
            clauses.Add(clause);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IReadOnlyDictionary<VariableReference, Term>> Fetch(CNFClause clause)
        {
            foreach (var otherClause in clauses)
            {
                foreach (var (unifier, unified) in ClauseUnifier.Unify(clause, otherClause))
                {
                    yield return unifier.Substitutions;
                }
            }
        }
    }
}
#endif
