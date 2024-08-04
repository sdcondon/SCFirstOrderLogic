// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncFeatureVectorIndexNode{TKeyElement, TValue}"/> that just stores its content in memory.
/// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> for child nodes.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider just using <see cref="FeatureVectorIndex{TKeyElement, TValue}"/> to avoid the overhead of asynchronicity.
/// <see cref="AsyncFeatureVectorIndex{TKeyElement, TValue}"/> is intended to facilitate tries that use secondary storage - this type is primarily
/// intended as an example implementation to base real (secondary storage utilising) implementations on.
/// </para>
/// </summary>
/// <typeparam name="TKeyElement">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class AsyncFeatureVectorIndexDictionaryNode<TKeyElement, TValue> : IAsyncFeatureVectorIndexNode<TKeyElement, TValue>
    where TKeyElement : notnull
{
    private readonly ConcurrentDictionary<TKeyElement, IAsyncFeatureVectorIndexNode<TKeyElement, TValue>> children;
    private TValue? value;

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    public AsyncFeatureVectorIndexDictionaryNode()
        : this(EqualityComparer<TKeyElement>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    /// <param name="equalityComparer">
    /// The equality comparer that should be used by the child dictionary.
    /// For correct behaviour, trie instances accessing this node should be using an <see cref="IComparer{T}"/> that is consistent with it. 
    /// That is, one that only returns zero for elements considered equal by equality comparer used by this instance.
    /// </param>
    public AsyncFeatureVectorIndexDictionaryNode(IEqualityComparer<TKeyElement> equalityComparer)
    {
        children = new(equalityComparer);
    }

    /// <inheritdoc/>
    public bool HasValue { get; private set; }

    /// <inheritdoc/>
    public TValue Value => HasValue ? value! : throw new InvalidOperationException("Node has no attached value");

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<TKeyElement, IAsyncFeatureVectorIndexNode<TKeyElement, TValue>>> GetChildren()
    {
        foreach (var kvp in children)
        {
            yield return kvp;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TKeyElement, TValue>?> TryGetChildAsync(TKeyElement keyElement)
    {
        children.TryGetValue(keyElement, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TKeyElement, TValue>> GetOrAddChildAsync(TKeyElement keyElement)
    {
        IAsyncFeatureVectorIndexNode<TKeyElement, TValue> node = new AsyncFeatureVectorIndexDictionaryNode<TKeyElement, TValue>();
        if (!children.TryAdd(keyElement, node))
        {
            node = children[keyElement];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(TKeyElement keyElement)
    {
        children.Remove(keyElement, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask AddValueAsync(TValue value)
    {
        if (HasValue)
        {
            throw new InvalidOperationException("A value is already stored against this node");
        }

        this.value = value;
        HasValue = true;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask RemoveValueAsync()
    {
        value = default;
        HasValue = false;

        return ValueTask.CompletedTask;
    }
}
