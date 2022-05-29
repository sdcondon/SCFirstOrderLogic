using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A basic implementation of <see cref="IKnowledgeBaseClauseStore"/> that just maintains all known clauses in
    /// an (un-indexed) in-memory collection and iterates through them all to find resolvents.
    /// </summary>
    public class SimpleClauseStore : IKnowledgeBaseClauseStore
    {
        private readonly ConcurrentBag<CNFClause> clauses = new ConcurrentBag<CNFClause>();
        private readonly SemaphoreSlim addLock = new SemaphoreSlim(1);

        /// <inheritdoc />
        public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            // We need to lock here to avoid an issue when a clause is added by another
            // thread between us checking and adding.
            await addLock.WaitAsync(cancellationToken);

            try
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
            finally
            {
                addLock.Release();
            }
        }

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clause in clauses) // nb: will currently explode if changed..
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return clause;
            }
        }
#pragma warning restore CS1998

        /// <inheritdoc />
        public async IAsyncEnumerable<ClauseResolution> FindResolutions(
            CNFClause clause,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var otherClause in this.WithCancellation(cancellationToken))
            {
                foreach (var resolution in ClauseResolution.Resolve(clause, otherClause))
                {
                    yield return resolution;
                }
            }
        }

        /// <inheritdoc />
        public IQueryClauseStore CreateQueryClauseStore() => new QueryClauseStore(clauses);

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="SimpleClauseStore"/>.
        /// </summary>
        private class QueryClauseStore : IQueryClauseStore
        {
            private readonly ConcurrentBag<CNFClause> clauses;
            private readonly SemaphoreSlim addLock = new SemaphoreSlim(1);

            public QueryClauseStore(IEnumerable<CNFClause> clauses) => this.clauses = new ConcurrentBag<CNFClause>(clauses);

            /// <inheritdoc />
            public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
            {
                // We need to lock here to avoid an issue when a clause is added by another
                // thread between us checking and adding.
                await addLock.WaitAsync(cancellationToken);

                try
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
                finally
                {
                    addLock.Release();
                }
            }

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
            /// <inheritdoc />
            public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                foreach (var clause in clauses)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return clause;
                }
            }
#pragma warning restore CS1998

            /// <inheritdoc />
            public async IAsyncEnumerable<ClauseResolution> FindResolutions(
                CNFClause clause,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await foreach (var otherClause in this.WithCancellation(cancellationToken))
                {
                    foreach (var resolution in ClauseResolution.Resolve(clause, otherClause))
                    {
                        yield return resolution;
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
