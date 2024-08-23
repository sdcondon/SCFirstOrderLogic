// Copyright © 2023-2024 Simon Condon.
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
    /// Get the child nodes of this node, keyed by the element represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<KeyValuePair<TFeature, int>, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by the element it represents.
    /// </summary>
    /// <param name="vectorElement">The element represented by the child node to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(KeyValuePair<TFeature, int> vectorElement);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="vectorElement">The element represented by the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(KeyValuePair<TFeature, int> vectorElement);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="vectorElement">The element represented by the node to be removed.</param>
    ValueTask DeleteChildAsync(KeyValuePair<TFeature, int> vectorElement);

    /// <summary>
    /// Gets the values stored against the node.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> GetValues();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause);

    /// <summary>
    /// Adds a value to this node, in so doing specifying that this node represents the "last" element of a stored set.
    /// </summary>
    /// <param name="value">The value to store.</param>
    ValueTask AddValueAsync(CNFClause clause, TValue value);

    /// <summary>
    /// Removes the value from this node, in so doing specifying that this node no longer represents the "last" element of a stored set.
    /// </summary>
    ValueTask<bool> RemoveValueAsync(CNFClause clause);
}
