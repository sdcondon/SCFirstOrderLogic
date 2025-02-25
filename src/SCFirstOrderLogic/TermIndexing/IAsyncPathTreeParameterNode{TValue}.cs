﻿// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface for path tree node types whose descendents represent all of the values of a particular function parameter
/// (or, for the root node, all values of the root element of the term) that occur within the terms stored in a path tree.
/// </summary>
public interface IAsyncPathTreeParameterNode<TValue>
{
    /// <summary>
    /// Get the child nodes of this node, keyed by objects that describe the element represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<IPathTreeArgumentNodeKey, IAsyncPathTreeArgumentNode<TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by its <see cref="IPathTreeArgumentNodeKey"/>.
    /// </summary>
    /// <param name="key">The key of the child to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncPathTreeArgumentNode<TValue>?> TryGetChildAsync(IPathTreeArgumentNodeKey key);

    /// <summary>
    /// Gets or adds a child of this node - dependent on whether or not a child node with a given key is already present.
    /// </summary>
    /// <param name="key">The key for the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncPathTreeArgumentNode<TValue>> GetOrAddChildAsync(IPathTreeArgumentNodeKey key);
}
