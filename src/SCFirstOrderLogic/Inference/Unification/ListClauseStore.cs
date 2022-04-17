using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Basic implementation of <see cref="IKnowledgeBaseClauseStore"/> that just maintains all known clauses in a <see cref="List{T}"/>.
    /// </summary>
    public class ListClauseStore : IKnowledgeBaseClauseStore
    {
        private readonly List<CNFClause> clauses = new List<CNFClause>();

        /// <inheritdoc />
        public Task AddAsync(CNFClause clause)
        {
            clauses.Add(clause);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clause in clauses)
            {
                yield return clause;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindUnifiers(CNFClause clause)
        {
            foreach (var otherClause in clauses)
            {
                foreach (var (unifier, unified) in ClauseUnifier.Unify(clause, otherClause))
                {
                    yield return (otherClause, unifier, unified);
                }
            }
        }

        /// <inheritdoc />
        public IQueryClauseStore CreateQueryClauseStore() => new QueryClauseStore(clauses);

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="ListClauseStore"/>.
        /// </summary>
        private class QueryClauseStore : IQueryClauseStore
        {
            private readonly List<CNFClause> clauses;

            public QueryClauseStore(IEnumerable<CNFClause> clauses) => this.clauses = new List<CNFClause>(clauses);

            /// <inheritdoc />
            public Task AddAsync(CNFClause clause)
            {
                clauses.Add(clause);
                return Task.CompletedTask;
            }

            /// <inheritdoc />
            public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                foreach (var clause in clauses)
                {
                    yield return clause;
                }
            }

            /// <inheritdoc />
            public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindUnifiers(CNFClause clause)
            {
                foreach (var otherClause in clauses)
                {
                    foreach (var (unifier, unified) in ClauseUnifier.Unify(clause, otherClause))
                    {
                        yield return (otherClause, unifier, unified);
                    }
                }
            }

            /// <inheritdoc />
            public void Dispose()
            {
                //// Nothing to do..
            }
        }
    }
}
