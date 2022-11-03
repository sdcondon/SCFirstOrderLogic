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
        private readonly ConcurrentBag<CNFClause> clauses = new();
        private readonly SemaphoreSlim addLock = new(1);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class.
        /// </summary>
        public SimpleClauseStore() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class that is pre-populated with some knowledge.
        /// <para/>
        /// NB: Most "real" implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
        /// "real" clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
        /// We only include it here because of the in-memory nature of this implementation and its usefulness for tests.
        /// </summary>
        /// <param name="sentences">The initial content of the store.</param>
        public SimpleClauseStore(IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                foreach (var clause in sentence.ToCNF().Clauses)
                {
                    clauses.Add(clause);
                }
            }
        }

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
        public IQueryClauseStore CreateQueryClauseStore() => new QueryClauseStore(clauses);

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="SimpleClauseStore"/>.
        /// </summary>
        private class QueryClauseStore : IQueryClauseStore
        {
            private readonly ConcurrentBag<CNFClause> clauses;
            private readonly SemaphoreSlim addLock = new(1);

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

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
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
