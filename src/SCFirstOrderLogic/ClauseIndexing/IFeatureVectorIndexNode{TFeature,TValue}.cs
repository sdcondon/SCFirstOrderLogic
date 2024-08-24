// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// Interface for types capable of serving as nodes of a <see cref="FeatureVectorIndex{TKeyElement, TValue}"/>.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public interface IFeatureVectorIndexNode<TFeature, TValue>
{
    /// <summary>
    /// Gets the child nodes of this node, keyed by the vector component represented by the child.
    /// </summary>
    IReadOnlyDictionary<KeyValuePair<TFeature, int>, IFeatureVectorIndexNode<TFeature, TValue>> Children { get; }

    /// <summary>
    /// 
    /// </summary>
    bool HasValues { get; }

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be retrieved or added.</param>
    /// <returns>The retrieved or added node.</returns>
    IFeatureVectorIndexNode<TFeature, TValue> GetOrAddChild(KeyValuePair<TFeature, int> vectorComponent);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be removed.</param>
    void DeleteChild(KeyValuePair<TFeature, int> vectorComponent);

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
    /// <returns></returns>
    public IEnumerable<TValue> GetSubsumedValues(CNFClause clause);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    public IEnumerable<TValue> GetSubsumingValues(CNFClause clause);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(CNFClause clause, out TValue value);
}
