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
/// An implementation of <see cref="IAsyncFeatureVectorIndexNode{TFeature, TValue}"/> that just stores its content in memory.
/// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> for child nodes.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider just using <see cref="FeatureVectorIndex{TFeature, TValue}"/> to avoid the overhead of asynchronicity.
/// <see cref="AsyncFeatureVectorIndex{TFeature, TValue}"/> is intended to facilitate tries that use secondary storage - this type is primarily
/// intended as an example implementation to base real (secondary storage utilising) implementations on.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
// todo: ordered lists make way more sense than dictionaries here..
public class AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue> : IAsyncFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly ConcurrentDictionary<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>> children;
    private readonly Dictionary<CNFClause, TValue> values = new();

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
        children = new(equalityComparer);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetChildren()
    {
        foreach (var kvp in children)
        {
            yield return kvp;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(KeyValuePair<TFeature, int> vectorElement)
    {
        children.TryGetValue(vectorElement, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(KeyValuePair<TFeature, int> vectorElement)
    {
        IAsyncFeatureVectorIndexNode<TFeature, TValue> node = new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>();
        if (!children.TryAdd(vectorElement, node))
        {
            node = children[vectorElement];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(KeyValuePair<TFeature, int> vectorElement)
    {
        children.Remove(vectorElement, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> GetValues()
    {
        foreach (var value in values)
        {
            yield return value;
        }
    }

    /// <inheritdoc/>
    public ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause)
    {
        var isSucceeded = values.TryGetValue(clause, out var value);
        return ValueTask.FromResult((isSucceeded, value));
    }

    /// <inheritdoc/>
    public ValueTask AddValueAsync(CNFClause clause, TValue value)
    {
        // todo: unify, or expect ordinalisation? tricky bit in ordinalisation will be ensuring consistent ordering of literals in clause..
        if (!values.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<bool> RemoveValueAsync(CNFClause clause)
    {
        // todo: unify, or expect ordinalisation? tricky bit in ordinalisation will be ensuring consistent ordering of literals in clause..
        return ValueTask.FromResult(values.Remove(clause));
    }
}
