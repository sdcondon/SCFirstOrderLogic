// Copyright � 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncFeatureVectorIndexNode{TFeature, TValue}"/> that just stores its content in memory.
/// Uses a <see cref="SortedList{TKey, TValue}"/> for child nodes.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider using <see cref="FeatureVectorIndex{TFeature, TValue}"/> instead, to avoid the overhead of asynchronicity.
/// <see cref="AsyncFeatureVectorIndex{TFeature, TValue}"/> is intended for use with node implementations that interact with external (i.e. I/O-requiring) storage.
/// This type is intended only as an example implementation to base real implementations on.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue> : IAsyncFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly SortedList<FeatureVectorComponent<TFeature>, IAsyncFeatureVectorIndexNode<TFeature, TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new(new VariableIdIgnorantEqualityComparer());

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    public AsyncFeatureVectorIndexDictionaryNode()
        : this(Comparer<TFeature>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the components
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public AsyncFeatureVectorIndexDictionaryNode(IComparer<TFeature> featureComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(new FeatureVectorComponentComparer<TFeature>(featureComparer));
    }

    private AsyncFeatureVectorIndexDictionaryNode(IComparer<TFeature> featureComparer, IComparer<FeatureVectorComponent<TFeature>> vectorComponentComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(vectorComponentComparer);
    }

    /// <inheritdoc/>
    public IComparer<TFeature> FeatureComparer { get; }

    /// <inheritdoc/>
    public IAsyncEnumerable<KeyValuePair<FeatureVectorComponent<TFeature>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> Children
    {
        get
        {
            async IAsyncEnumerable<KeyValuePair<FeatureVectorComponent<TFeature>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetReturnValue()
            {
                foreach (var kvp in childrenByVectorComponent)
                {
                    yield return kvp;
                }
            }

            return GetReturnValue();
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs
    {
        get
        {
            async IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> GetReturnValue()
            {
                foreach (var kvp in valuesByKey)
                {
                    yield return kvp;
                }
            }

            return GetReturnValue();
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(FeatureVectorComponent<TFeature> vectorComponent)
    {
        childrenByVectorComponent.TryGetValue(vectorComponent, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(FeatureVectorComponent<TFeature> vectorComponent)
    {
        IAsyncFeatureVectorIndexNode<TFeature, TValue> node = new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>(FeatureComparer, childrenByVectorComponent.Comparer);
        if (!childrenByVectorComponent.TryAdd(vectorComponent, node))
        {
            node = childrenByVectorComponent[vectorComponent];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(FeatureVectorComponent<TFeature> vectorComponent)
    {
        childrenByVectorComponent.Remove(vectorComponent, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask AddValueAsync(CNFClause clause, TValue value)
    {
        if (!valuesByKey.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<bool> RemoveValueAsync(CNFClause clause)
    {
        return ValueTask.FromResult(valuesByKey.Remove(clause));
    }

    /// <inheritdoc/>
    public ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause)
    {
        var isSucceeded = valuesByKey.TryGetValue(clause, out var value);
        return ValueTask.FromResult((isSucceeded, value));
    }
}
