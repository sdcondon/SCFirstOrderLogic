// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TFeature, TValue}"/> that just stores things in memory. 
/// Uses a <see cref="Dictionary{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
// todo-breaking-v7: ordered lists for children make way more sense than dictionaries here, but this does of course
// raise questions about what should be responsible for the feature comparison logic
public class FeatureVectorIndexDictionaryNode<TFeature, TValue> : IFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly Dictionary<KeyValuePair<TFeature, int>, IFeatureVectorIndexNode<TFeature, TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new();

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexDictionaryNode{TFeature, TValue}"/> class.
    /// </summary>
    public FeatureVectorIndexDictionaryNode()
        : this(EqualityComparer<KeyValuePair<TFeature, int>>.Default)
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
    public FeatureVectorIndexDictionaryNode(IEqualityComparer<KeyValuePair<TFeature, int>> equalityComparer)
    {
        childrenByVectorComponent = new(equalityComparer);
    }

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyDictionary<KeyValuePair<TFeature, int>, IFeatureVectorIndexNode<TFeature, TValue>> Children => childrenByVectorComponent;

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs => valuesByKey;

    /// <inheritdoc/>
    public IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(KeyValuePair<TFeature, int> vectorComponent)
    {
        if (!childrenByVectorComponent.TryGetValue(vectorComponent, out var node))
        {
            node = new FeatureVectorIndexDictionaryNode<TFeature, TValue>();
            childrenByVectorComponent.Add(vectorComponent, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void DeleteChild(KeyValuePair<TFeature, int> vectorComponent)
    {
        childrenByVectorComponent.Remove(vectorComponent);
    }

    /// <inheritdoc/>
    public void AddValue(CNFClause clause, TValue value)
    {
        // todo: unify (vars only) - might not match exactly
        if (!valuesByKey.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }
    }

    /// <inheritdoc/>
    public bool RemoveValue(CNFClause clause)
    {
        // todo: unify (vars only) - might not match exactly
        return valuesByKey.Remove(clause);
    }

    /// <inheritdoc/>
    public bool TryGetValue(CNFClause clause, [MaybeNullWhen(false)] out TValue value)
    {
        // todo: unify (vars only) - might not match exactly
        return valuesByKey.TryGetValue(clause, out value);
    }
}
