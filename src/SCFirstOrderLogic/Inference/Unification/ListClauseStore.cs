using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            // NB: a limitation of this implementation - we only check if the clause is already present exactly - we don't check for clauses that subsume it.
            await foreach (var existingClause in this.WithCancellation(cancellationToken))
            {
                if (existingClause.Equals(clause))
                {
                    return false;
                }
            }

            clauses.Add(clause);
            return true;
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
        public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindUnifiers(
            CNFClause clause,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var otherClause in this.WithCancellation(cancellationToken))
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
            public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
            {
                // NB: a limitation of this implementation - we only check if the clause is already present exactly - we don't check for clauses that subsume it.
                await foreach (var existingClause in this.WithCancellation(cancellationToken))
                {
                    if (existingClause.Equals(clause))
                    {
                        return false;
                    }
                }

                clauses.Add(clause);
                return true;
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
            public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindUnifiers(
                CNFClause clause,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await foreach (var otherClause in this.WithCancellation(cancellationToken))
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
