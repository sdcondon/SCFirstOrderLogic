// Copyright © 2023-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// Interface for types capable of serving as nodes of a <see cref="FeatureVectorIndex{TKeyElement, TValue}"/>.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public interface IFeatureVectorIndexNode<TFeature, TValue>
{
    /// <summary>
    /// Gets the comparer that should be used to compare features when adding to or retrieving from this node.
    /// </summary>
    IComparer<TFeature> FeatureComparer { get; }

    /// <summary>
    /// Gets the child nodes of this node, keyed by the vector component represented by the child, and in ascending order.
    /// </summary>
    IEnumerable<KeyValuePair<FeatureVectorComponent<TFeature>, IFeatureVectorIndexNode<TFeature, TValue>>> ChildrenAscending { get; }

    /// <summary>
    /// Gets the child nodes of this node, keyed by the vector component represented by the child, and in descending order.
    /// </summary>
    IEnumerable<KeyValuePair<FeatureVectorComponent<TFeature>, IFeatureVectorIndexNode<TFeature, TValue>>> ChildrenDescending { get; }

    /// <summary>
    /// Gets the key-value pairs attached to this node.
    /// </summary>
    IEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs { get; }

    /// <summary>
    /// Attempts to retrieve a child node by the vector component it represents.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the child node to retrieve.</param>
    /// <param name="child"></param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    bool TryGetChild(FeatureVectorComponent<TFeature> vectorComponent, [MaybeNullWhen(false)] out IFeatureVectorIndexNode<TFeature, TValue> child);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be retrieved or added.</param>
    /// <returns>The retrieved or added node.</returns>
    IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(FeatureVectorComponent<TFeature> vectorComponent);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be removed.</param>
    void DeleteChild(FeatureVectorComponent<TFeature> vectorComponent);

    /// <summary>
    /// Adds a value to this node.
    /// </summary>
    /// <param name="clause">The clause with which this value is associated.</param>
    /// <param name="value">The value to store.</param>
    void AddValue(CNFClause clause, TValue value);

    /// <summary>
    /// Removes a value from this node.
    /// </summary>
    /// <param name="clause"></param>
    bool RemoveValue(CNFClause clause);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(CNFClause clause, [MaybeNullWhen(false)] out TValue value);
}
