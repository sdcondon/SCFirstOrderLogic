// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface shared by all nodes of a <see cref="DiscriminationTree{TValue}"/>.
/// </summary>
public interface IDiscriminationTreeNode<TValue>
{
    /// <summary>
    /// Gets the child nodes of this node, keyed by objects that describe the element represented by the child.
    /// </summary>
    IReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> Children { get; }

    /// <summary>
    /// Gets the value attached to a leaf node. Will not be called for internal nodes - throwing an exception is acceptable behaviour for them.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Gets or adds an internal child of this node.
    /// </summary>
    /// <param name="elementInfo">The element info for the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    IDiscriminationTreeNode<TValue> GetOrAddInternalChild(IDiscriminationTreeNodeKey elementInfo);

    /// <summary>
    /// Adds a child node of this node that is a leaf.
    /// </summary>
    /// <param name="elementInfo">The element info for the added node.</param>
    /// <param name="value">The value to be attached to the new node.</param>
    void AddLeafChild(IDiscriminationTreeNodeKey elementInfo, TValue value);
}
