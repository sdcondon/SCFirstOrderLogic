// Copyright � 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// Interface for types capable of serving as nodes of an <see cref="AsyncFeatureVectorIndex{TKeyElement, TValue}"/>.
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public interface IAsyncFeatureVectorIndexNode<TFeature, TValue>
{
    /// <summary>
    /// Get the child nodes of this node, keyed by the vector component represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by the vector component it represents.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the child node to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(KeyValuePair<TFeature, int> vectorComponent);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(KeyValuePair<TFeature, int> vectorComponent);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be removed.</param>
    ValueTask DeleteChildAsync(KeyValuePair<TFeature, int> vectorComponent);

    /// <summary>
    /// Adds a value to this node, in so doing specifying that this node represents the "last" element of a stored set.
    /// </summary>
    /// <param name="clause"></param>
    /// <param name="value">The value to store.</param>
    ValueTask AddValueAsync(CNFClause clause, TValue value);

    /// <summary>
    /// Removes the value from this node, in so doing specifying that this node no longer represents the "last" element of a stored set.
    /// </summary>
    ValueTask<bool> RemoveValueAsync(CNFClause clause);

    /// <summary>
    /// Gets a value indicating whether any values are stored against the node.
    /// </summary>
    ValueTask<bool> GetHasValues();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    IAsyncEnumerable<TValue> GetSubsumedValues(CNFClause clause);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    IAsyncEnumerable<TValue> GetSubsumingValues(CNFClause clause);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause);
}