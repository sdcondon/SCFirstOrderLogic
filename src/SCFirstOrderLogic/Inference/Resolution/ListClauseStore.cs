using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Basic implementation of <see cref="IKnowledgeBaseClauseStore"/> that just maintains all known clauses in
    /// an in-memory collection and iterates through them all to find resolvents.
    /// </summary>
    public class ListClauseStore : IKnowledgeBaseClauseStore
    {
        // TODO-ROBUSTNESS: concurrent(bag?), or keep it as a list and do locking - intended as simplest implementation after all.
        // Concurrency problems abound with this class at the mo.
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

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clause in clauses) // nb: will currently explode if changed..
            {
                yield return clause;
            }
        }
#pragma warning restore CS1998

        /// <inheritdoc />
        public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindResolutions(
            CNFClause clause,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var otherClause in this.WithCancellation(cancellationToken))
            {
                foreach (var (unifier, resolvent) in ClauseResolver.Resolve(clause, otherClause))
                {
                    yield return (otherClause, unifier, resolvent);
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
            private readonly List<CNFClause> clauses; // todo: concurrent or locking.

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

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
            /// <inheritdoc />
            public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                foreach (var clause in clauses)
                {
                    yield return clause;
                }
            }
#pragma warning restore CS1998

            /// <inheritdoc />
            public async IAsyncEnumerable<(CNFClause, VariableSubstitution, CNFClause)> FindResolutions(
                CNFClause clause,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await foreach (var otherClause in this.WithCancellation(cancellationToken))
                {
                    foreach (var (unifier, unified) in ClauseResolver.Resolve(clause, otherClause))
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
