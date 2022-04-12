#if FALSE
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Basic implementation of <see cref="IClauseStore"/> that just maintains all known clauses in a <see cref="List{T}"/>.
    /// </summary>
    public class ListClauseStore : IClauseStore
    {
        private readonly List<CNFClause> clauses = new List<CNFClause>();

        /// <inheritdoc />
        public Task StoreAsync(CNFClause clause)
        {
            clauses.Add(clause);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<(CNFClause, VariableSubstitution)> FindUnifiers(CNFClause clause)
        {
            foreach (var otherClause in clauses)
            {
                foreach (var (unifier, _) in ClauseUnifier.Unify(clause, otherClause))
                {
                    yield return (otherClause, unifier);
                }
            }
        }
    }
}
#endif
