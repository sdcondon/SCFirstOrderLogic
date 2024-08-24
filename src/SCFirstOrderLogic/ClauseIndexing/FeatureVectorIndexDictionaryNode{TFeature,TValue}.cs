// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TFeature, TValue}"/> that just stores things in memory. 
/// Uses a <see cref="Dictionary{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
// todo: ordered lists make way more sense than dictionaries here..
public class FeatureVectorIndexDictionaryNode<TFeature, TValue> : IFeatureVectorIndexNode<TFeature, TValue>
    where TFeature : notnull
{
    private readonly Dictionary<KeyValuePair<TFeature, int>, IFeatureVectorIndexNode<TFeature, TValue>> children;
    private readonly Dictionary<CNFClause, TValue> values = new();

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
        children = new(equalityComparer);
    }

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyDictionary<KeyValuePair<TFeature, int>, IFeatureVectorIndexNode<TFeature, TValue>> Children => children;

    /// <inheritdoc/>
    public bool HasValues => values.Count > 0;

    /// <inheritdoc/>
    public IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(KeyValuePair<TFeature, int> vectorComponent)
    {
        if (!children.TryGetValue(vectorComponent, out var node))
        {
            node = new FeatureVectorIndexDictionaryNode<TFeature, TValue>();
            children.Add(vectorComponent, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void DeleteChild(KeyValuePair<TFeature, int> vectorComponent)
    {
        children.Remove(vectorComponent);
    }

    /// <inheritdoc/>
    public void AddValue(CNFClause clause, TValue value)
    {
        // todo: unify (vars only) - might not match exactly
        if (!values.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }
    }

    /// <inheritdoc/>
    public bool RemoveValue(CNFClause clause)
    {
        // todo: unify (vars only) - might not match exactly
        return values.Remove(clause);
    }

    /// <inheritdoc/>
    public IEnumerable<TValue> GetSubsumedValues(CNFClause clause)
    {
        return values.Where(kvp => clause.Subsumes(kvp.Key)).Select(kvp => kvp.Value);
    }

    /// <inheritdoc/>
    public IEnumerable<TValue> GetSubsumingValues(CNFClause clause)
    {
        return values.Where(kvp => kvp.Key.Subsumes(clause)).Select(kvp => kvp.Value);
    }

    /// <inheritdoc/>
    public bool TryGetValue(CNFClause clause, out TValue value)
    {
        // todo: unify (vars only) - might not match exactly
        return values.TryGetValue(clause, out value);
    }
}
