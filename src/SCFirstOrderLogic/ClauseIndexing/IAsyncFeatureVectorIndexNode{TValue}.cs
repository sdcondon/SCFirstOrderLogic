// Copyright (c) 2023-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// Interface for types capable of serving as nodes of an <see cref="AsyncFeatureVectorIndex{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public interface IAsyncFeatureVectorIndexNode<TValue>
{
    /// <summary>
    /// Gets the comparer that should be used to compare features when adding to or retrieving from this node.
    /// </summary>
    IComparer FeatureComparer { get; }

    /// <summary>
    /// Gets the child nodes of this node, keyed by the vector component represented by the child, and in ascending order.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> ChildrenAscending { get; }

    /// <summary>
    /// Gets the child nodes of this node, keyed by the vector component represented by the child, and in descending order.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> ChildrenDescending { get; }

    /// <summary>
    /// Gets the key-value pairs attached to this node.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs { get; }

    /// <summary>
    /// Attempts to retrieve a child node by the vector component it represents.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the child node to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TValue>?> TryGetChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the retrieved or added node.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TValue>> GetOrAddChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="vectorComponent">The vector component represented by the node to be removed.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    ValueTask DeleteChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a value to this node.
    /// </summary>
    /// <param name="clause">The clause to add the value for.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    ValueTask AddValueAsync(CNFClause clause, TValue value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the value from this node.
    /// </summary>
    /// <param name="clause">The clause to add the value for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    ValueTask<bool> RemoveValueAsync(CNFClause clause, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns></returns>
    ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause, CancellationToken cancellationToken = default);
}
