// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface for path tree node types whose descendents represent all of the values of a particular function parameter
/// (or, for the root node, all values of the root element of the term) that occur within the terms stored in a path tree.
/// </summary>
public interface IPathTreeParameterNode<TValue>
{
    /// <summary>
    /// Gets the children of this node, keyed by the appropriate <see cref="IPathTreeArgumentNodeKey"/> for the term element represented by each.
    /// </summary>
    IReadOnlyDictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> Children { get; }

    /// <summary>
    /// Gets or adds a child of this node - dependent on whether or not a child node with a given key is already present.
    /// </summary>
    /// <param name="key">The key for the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    IPathTreeArgumentNode<TValue> GetOrAddChild(IPathTreeArgumentNodeKey key);
}
