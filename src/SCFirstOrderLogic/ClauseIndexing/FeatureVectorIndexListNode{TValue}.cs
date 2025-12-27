// Copyright © 2023-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation.Substitution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TValue}"/> that just stores things in memory. 
/// Uses a <see cref="SortedList{TKey, TValue}"/> for the children of a node, and a <see cref="Dictionary{TKey, TValue}"/>
/// for leaf values.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class FeatureVectorIndexListNode<TValue> : IFeatureVectorIndexNode<TValue>
{
    private readonly SortedList<FeatureVectorComponent, IFeatureVectorIndexNode<TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new(new VariableIdAgnosticEqualityComparer());

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexListNode{TValue}"/> class that
    /// uses the default comparer of the feature type to determine the ordering of nodes. Note that this comparer will
    /// throw if the runtime type of a feature object does not implement <see cref="IComparable{T}"/>.
    /// </summary>
    public FeatureVectorIndexListNode()
        : this(Comparer.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="FeatureVectorIndexListNode{TValue}"/> class.
    /// </summary>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the components
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public FeatureVectorIndexListNode(IComparer featureComparer)
        : this(featureComparer, new FeatureVectorComponentComparer(featureComparer))
    {
    }

    private FeatureVectorIndexListNode(IComparer featureComparer, IComparer<FeatureVectorComponent> vectorComponentComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(vectorComponentComparer);
    }

    /// <inheritdoc/>
    public IComparer FeatureComparer { get; }

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IEnumerable<KeyValuePair<FeatureVectorComponent, IFeatureVectorIndexNode<TValue>>> ChildrenAscending => childrenByVectorComponent;

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<FeatureVectorComponent, IFeatureVectorIndexNode<TValue>>> ChildrenDescending
    {
        get
        {
            for (int i = childrenByVectorComponent.Count - 1; i >= 0; i--)
            {
                yield return new KeyValuePair<FeatureVectorComponent, IFeatureVectorIndexNode<TValue>>(childrenByVectorComponent.Keys[i], childrenByVectorComponent.Values[i]);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs => valuesByKey;

    /// <inheritdoc/>
    public bool TryGetChild(FeatureVectorComponent vectorComponent, [MaybeNullWhen(false)] out IFeatureVectorIndexNode<TValue> child)
    {
        return childrenByVectorComponent.TryGetValue(vectorComponent, out child);
    }

    /// <inheritdoc/>
    public IFeatureVectorIndexNode<TValue> GetOrAddChild(FeatureVectorComponent vectorComponent)
    {
        if (!childrenByVectorComponent.TryGetValue(vectorComponent, out var node))
        {
            node = new FeatureVectorIndexListNode<TValue>(FeatureComparer, childrenByVectorComponent.Comparer);
            childrenByVectorComponent.Add(vectorComponent, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void DeleteChild(FeatureVectorComponent vectorComponent)
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
