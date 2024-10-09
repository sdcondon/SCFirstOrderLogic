// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TFeature, TValue}"/> that just stores things in memory. 
/// Uses a <see cref="SortedList{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class FeatureVectorIndexDictionaryNode<TFeature, TValue> : IFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly SortedList<FeatureVectorComponent<TFeature>, IFeatureVectorIndexNode<TFeature, TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new(new VariableIdIgnorantEqualityComparer());

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class that
    /// uses the default comparer of the feature type to determine the ordering of nodes.
    /// </summary>
    public FeatureVectorIndexDictionaryNode()
        : this(Comparer<TFeature>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the components
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public FeatureVectorIndexDictionaryNode(IComparer<TFeature> featureComparer)
        : this(featureComparer, new FeatureVectorComponentComparer<TFeature>(featureComparer))
    {
    }

    private FeatureVectorIndexDictionaryNode(IComparer<TFeature> featureComparer, IComparer<FeatureVectorComponent<TFeature>> vectorComponentComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(vectorComponentComparer);
    }

    /// <inheritdoc/>
    public IComparer<TFeature> FeatureComparer { get; }

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyCollection<KeyValuePair<FeatureVectorComponent<TFeature>, IFeatureVectorIndexNode<TFeature, TValue>>> Children => childrenByVectorComponent;

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs => valuesByKey;

    /// <inheritdoc/>
    public bool TryGetChild(FeatureVectorComponent<TFeature> vectorComponent, [MaybeNullWhen(false)] out IFeatureVectorIndexNode<TFeature, TValue> child)
    {
        return childrenByVectorComponent.TryGetValue(vectorComponent, out child);
    }

    /// <inheritdoc/>
    public IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(FeatureVectorComponent<TFeature> vectorComponent)
    {
        if (!childrenByVectorComponent.TryGetValue(vectorComponent, out var node))
        {
            node = new FeatureVectorIndexDictionaryNode<TFeature, TValue>(FeatureComparer, childrenByVectorComponent.Comparer);
            childrenByVectorComponent.Add(vectorComponent, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void DeleteChild(FeatureVectorComponent<TFeature> vectorComponent)
    {
        childrenByVectorComponent.Remove(vectorComponent);
    }

    /// <inheritdoc/>
    public void AddValue(CNFClause clause, TValue value)
    {
        if (!valuesByKey.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }
    }

    /// <inheritdoc/>
    public bool RemoveValue(CNFClause clause)
    {
        return valuesByKey.Remove(clause);
    }

    /// <inheritdoc/>
    public bool TryGetValue(CNFClause clause, [MaybeNullWhen(false)] out TValue value)
    {
        return valuesByKey.TryGetValue(clause, out value);
    }
}
