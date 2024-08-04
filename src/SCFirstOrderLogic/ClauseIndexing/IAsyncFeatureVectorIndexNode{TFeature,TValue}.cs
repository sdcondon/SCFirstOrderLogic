// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
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
    /// Gets a value indicating whether a value is stored against this node.
    /// That is, whether this node represents the "last" element of a stored set.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Gets the value stored against the node. Should throw an <see cref="InvalidOperationException"/> 
    /// if no value has been stored against the node.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Get the child nodes of this node, keyed by the element represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<TFeature, IAsyncFeatureVectorIndexNode<TFeature, TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by the element it represents.
    /// </summary>
    /// <param name="keyElement">The element represented by the child node to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>?> TryGetChildAsync(TFeature keyElement);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncFeatureVectorIndexNode<TFeature, TValue>> GetOrAddChildAsync(TFeature keyElement);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the node to be removed.</param>
    ValueTask DeleteChildAsync(TFeature keyElement);

    /// <summary>
    /// Adds a value to this node, in so doing specifying that this node represents the "last" element of a stored set.
    /// </summary>
    /// <param name="value">The value to store.</param>
    ValueTask AddValueAsync(TValue value);

    /// <summary>
    /// Removes the value from this node, in so doing specifying that this node no longer represents the "last" element of a stored set.
    /// </summary>
    ValueTask RemoveValueAsync();
}
