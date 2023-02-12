#if false
using SCFirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Resolution strategy that applies linear resolution.
    /// </summary>
    public class LinearResolutionStrategy : ISimpleResolutionStrategy
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LinearResolutionStrategy"/> class.
        /// </summary>
        /// <param name="clauseStore">The clause store to use.</param>
        public LinearResolutionStrategy(IKnowledgeBaseClauseStore clauseStore)
        {
            ClauseStore = new LinearClauseStore(clauseStore);
        }

        /// <inheritdoc/>
        public IKnowledgeBaseClauseStore ClauseStore { get; }

        /// <inheritdoc/>
        public ISimpleResolutionQueue MakeResolutionQueue() => new LinearResolutionQueue();

        // linear clause store wraps the iknowledgebaseclausestore
        // we DONT add intermediate clauses to the provided one, we just use
        // it for the lookup of resolutions with KB knowledge. we use a separate
        // (always in-mem) tree structure for resolutions with proof tree ancestors.
        private class LinearClauseStore : IKnowledgeBaseClauseStore
        {
            private readonly IKnowledgeBaseClauseStore innerClauseStore;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinearClauseStore"/> class.
            /// </summary>
            public LinearClauseStore(IKnowledgeBaseClauseStore innerClauseStore) => this.innerClauseStore = innerClauseStore;

            /// <inheritdoc />
            public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
            {
                // Shouldn't ever be called, but this is consistent behaviour,
                // so no point in throwing if we don't have to.
                return innerClauseStore.AddAsync(clause, cancellationToken);
            }

            /// <inheritdoc />
            public IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return innerClauseStore.GetAsyncEnumerator(cancellationToken);
            }

            /// <inheritdoc />
            public async Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken = default)
            {
                return await QueryStore.CreateAsync(innerClauseStore, cancellationToken);
            }

            /// <summary>
            /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="LinearClauseStore"/>.
            /// </summary>
            private class QueryStore : IQueryClauseStore
            {
                private readonly IQueryClauseStore kbClauseStore;
                private Dictionary<CNFClause, (CNFClause Clause1, CNFClause Clause2)> proofTree = new();

                public QueryStore(IQueryClauseStore kbClauseStore) => this.kbClauseStore = kbClauseStore;

                public static async Task<QueryStore> CreateAsync(IKnowledgeBaseClauseStore innerClauseStore, CancellationToken cancellationToken = default)
                {
                    return new QueryStore(await innerClauseStore.CreateQueryStoreAsync(cancellationToken));
                }

                /// <inheritdoc />
                public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
                {
                    if (kbClauseStore.)
                    {
                        return false;
                    }

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

        private class LinearResolutionQueue : ISimpleResolutionQueue
        {
            private readonly Queue<ClauseResolution> queue = new();

            public bool IsEmpty => queue.Count == 0;

            public ClauseResolution Dequeue() => queue.Dequeue();

            public void Enqueue(ClauseResolution resolution) => queue.Enqueue(resolution);
        }
    }
}
#endif