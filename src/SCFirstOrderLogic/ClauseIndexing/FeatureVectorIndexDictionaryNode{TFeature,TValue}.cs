// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TFeature, TValue}"/> that just stores things in memory. 
/// Uses a <see cref="Dictionary{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class FeatureVectorIndexDictionaryNode<TFeature, TValue> : IFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly Dictionary<TFeature, IFeatureVectorIndexNode<TFeature, TValue>> children;
    private TValue? value;

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    public FeatureVectorIndexDictionaryNode()
        : this(EqualityComparer<TFeature>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    /// <param name="equalityComparer">
    /// The equality comparer that should be used by the child dictionary.
    /// For correct behaviour, index instances accessing this node should be using an <see cref="IComparer{T}"/> that is consistent with it. 
    /// That is, one that only returns zero for features considered equal by the equality comparer used by this instance.
    /// </param>
    public FeatureVectorIndexDictionaryNode(IEqualityComparer<TFeature> equalityComparer)
    {
        children = new(equalityComparer);
    }

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyDictionary<TFeature, IFeatureVectorIndexNode<TFeature, TValue>> Children => children;

    /// <inheritdoc/>
    public bool HasValue { get; private set; }

    /// <inheritdoc/>
    public TValue Value => HasValue ? value! : throw new InvalidOperationException("Node has no attached value");

    /// <inheritdoc/>
    public IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(TFeature keyElement)
    {
        if (!children.TryGetValue(keyElement, out var node))
        {
            node = new FeatureVectorIndexDictionaryNode<TFeature, TValue>();
            children.Add(keyElement, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void DeleteChild(TFeature keyElement)
    {
        children.Remove(keyElement);
    }

    /// <inheritdoc/>
    public void AddValue(TValue value)
    {
        if (HasValue)
        {
            throw new InvalidOperationException("A value is already stored against this node");
        }

        this.value = value;
        HasValue = true;
    }

    /// <inheritdoc/>
    public void RemoveValue()
    {
        value = default;
        HasValue = false;
    }
}
