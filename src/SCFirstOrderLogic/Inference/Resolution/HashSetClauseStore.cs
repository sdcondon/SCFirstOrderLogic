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
    public class HashSetClauseStore : IKnowledgeBaseClauseStore
    {
        // Yes, not actually a hash set, despite the name of the class. I want strong concurrency support,
        // but System.Collections.Concurrent doesn't include a hash set implementation. Using a ConcurrentDictionary
        // means some overhead (the values), but its not worth doing anything else for this basic implementation.
        // Might consider using a third-party package for an actual concurrent hash set at some point, but.. probably won't.
        // Consumers to whom this matters can always create their own implementation.
        private readonly ConcurrentDictionary<CNFClause, byte> clauses = new();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetClauseStore"/> class.
        /// </summary>
        public HashSetClauseStore() { }

        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="HashSetClauseStore"/> class that is pre-populated with some knowledge.
        /// </para>
        /// <para>
        /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
        /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
        /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
        /// </para>
        /// </summary>
        /// <param name="sentences">The initial content of the store.</param>
        public HashSetClauseStore(IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                foreach (var clause in sentence.ToCNF().Clauses)
                {
                    clauses.TryAdd(clause, 0);
                }
            }
        }

        /// <inheritdoc />
        public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(clauses.TryAdd(clause, 0));
        }

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clause in clauses.Keys)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return clause;
            }
        }
#pragma warning restore CS1998

        /// <inheritdoc />
        public Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IQueryClauseStore>(new QueryStore(clauses));
        }

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="HashSetClauseStore"/>.
        /// </summary>
        private class QueryStore : IQueryClauseStore
        {
            private readonly ConcurrentDictionary<CNFClause, byte> clauses = new();

            public QueryStore(IEnumerable<KeyValuePair<CNFClause, byte>> clauses) => this.clauses = new(clauses);

            /// <inheritdoc />
            public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(clauses.TryAdd(clause, 0));
            }

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
            /// <inheritdoc />
            public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                foreach (var clause in clauses.Keys)
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
