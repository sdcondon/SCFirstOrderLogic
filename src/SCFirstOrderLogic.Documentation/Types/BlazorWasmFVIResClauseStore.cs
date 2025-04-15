// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.ClauseIndexing;
using System.Runtime.CompilerServices;

namespace SCFirstOrderLogic.Inference.Basic.Resolution;

/// <summary>
/// An implementation of <see cref="IKnowledgeBaseClauseStore"/> that maintains all known clauses in
/// a <see cref="AsyncFeatureVectorIndex{TFeature}"/>.
/// </summary>
// TODO-MAINTAINABILITY/PERFORMANCE: Remove as soon as possible, keep in sync with real implementation until then
public class BlazorWasmFVIResClauseStore<TFeature> : IKnowledgeBaseClauseStore
    where TFeature : notnull
{
    private readonly Func<CNFClause, IEnumerable<FeatureVectorComponent<TFeature>>> featureVectorMaker;
    private readonly IClauseStoreFVINode<TFeature> featureVectorIndexRoot;
    private readonly AsyncFeatureVectorIndex<TFeature> featureVectorIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndexClauseStore{TFeature}"/> class.
    /// </summary>
    /// <param name="featureVectorMaker">Delegate that makes the feature vector for a given clause.</param>
    /// <param name="featureVectorIndexRoot">The root node to use for the feature vector index.</param>
    public BlazorWasmFVIResClauseStore(
        Func<CNFClause, IEnumerable<FeatureVectorComponent<TFeature>>> featureVectorMaker,
        IClauseStoreFVINode<TFeature> featureVectorIndexRoot)
    {
        this.featureVectorMaker = featureVectorMaker;
        this.featureVectorIndexRoot = featureVectorIndexRoot;

        featureVectorIndex = new AsyncFeatureVectorIndex<TFeature>(
            featureVectorMaker,
            featureVectorIndexRoot);
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
    {
        return await featureVectorIndex.TryReplaceSubsumedAsync(clause);
    }

    /// <inheritdoc />
    public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);

        await foreach(var clause in featureVectorIndex.WithCancellation(cancellationToken))
        {
            yield return clause;
        }
    }

    /// <inheritdoc />
    public async Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken = default)
    {
        return new QueryStore(
            featureVectorMaker,
            await featureVectorIndexRoot.CopyAsync());
    }

    /// <summary>
    /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="HashSetClauseStore"/>.
    /// </summary>
    private class QueryStore : IQueryClauseStore
    {
        private readonly IClauseStoreFVINode<TFeature> featureVectorIndexRoot;
        private readonly AsyncFeatureVectorIndex<TFeature> featureVectorIndex;

        public QueryStore(
            Func<CNFClause, IEnumerable<FeatureVectorComponent<TFeature>>> featureVectorSelector,
            IClauseStoreFVINode<TFeature> featureVectorIndexRoot)
        {
            this.featureVectorIndexRoot = featureVectorIndexRoot;

            featureVectorIndex = new AsyncFeatureVectorIndex<TFeature>(
                featureVectorSelector,
                featureVectorIndexRoot);
        }

        /// <inheritdoc />
        public async Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            return await featureVectorIndex.TryReplaceSubsumedAsync(clause);
        }

        /// <inheritdoc />
        public async Task<bool> AddAsync(CNFClause clause, Func<CNFClause, Task> removedClauseCallback, CancellationToken cancellationToken = default)
        {
            return await featureVectorIndex.TryReplaceSubsumedAsync(clause, removedClauseCallback);
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            return await featureVectorIndex.ContainsAsync(clause);
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);

            await foreach (var clause in featureVectorIndex.WithCancellation(cancellationToken))
            {
                yield return clause;
            }
        }

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
            featureVectorIndexRoot.Dispose();
        }
    }
}
