// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncFeatureVectorIndexNode{TFeature, TValue}"/> that just stores its content in memory.
/// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> for child nodes.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider using <see cref="FeatureVectorIndex{TFeature, TValue}"/> instead, to avoid the overhead of asynchronicity.
/// <see cref="AsyncFeatureVectorIndex{TFeature, TValue}"/> is intended for use with node implementations that interact with external (i.e. I/O-requiring) storage.
/// This type is intended only as an example implementation to base real implementations on.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
// todo-breaking-v7: ordered lists for children make way more sense than dictionaries here, but this does of course
// raise questions about what should be responsible for the feature comparison logic
public class AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue> : IAsyncFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly ConcurrentDictionary<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new(new VariableIdIgnorantEqualityComparer());

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    public AsyncFeatureVectorIndexDictionaryNode()
        : this(EqualityComparer<KeyValuePair<TFeature, int>>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    /// <param name="equalityComparer">
    /// The equality comparer that should be used by the child dictionary.
    /// For correct behaviour, index instances accessing this node should be using an <see cref="IComparer{T}"/> that is consistent with it. 
    /// That is, one that only returns zero for features considered equal by equality comparer used by this instance.
    /// </param>
    public AsyncFeatureVectorIndexDictionaryNode(IEqualityComparer<KeyValuePair<TFeature, int>> equalityComparer)
    {
        childrenByVectorComponent = new(equalityComparer);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetChildren()
    {
        foreach (var kvp in childrenByVectorComponent)
        {
            yield return kvp;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(KeyValuePair<TFeature, int> vectorComponent)
    {
        childrenByVectorComponent.TryGetValue(vectorComponent, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(KeyValuePair<TFeature, int> vectorComponent)
    {
        IAsyncFeatureVectorIndexNode<TFeature, TValue> node = new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>();
        if (!childrenByVectorComponent.TryAdd(vectorComponent, node))
        {
            node = childrenByVectorComponent[vectorComponent];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(KeyValuePair<TFeature, int> vectorComponent)
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
    public async IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> GetKeyValuePairs()
    {
        foreach (var kvp in valuesByKey)
        {
            yield return kvp;
        }
    }

    /// <inheritdoc/>
    public ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause)
    {
        var isSucceeded = valuesByKey.TryGetValue(clause, out var value);
        return ValueTask.FromResult((isSucceeded, value));
    }
}
