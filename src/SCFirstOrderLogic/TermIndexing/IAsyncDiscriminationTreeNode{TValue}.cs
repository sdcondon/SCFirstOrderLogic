// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface shared by all nodes of a <see cref="AsyncDiscriminationTree{TValue}"/>.
/// </summary>
public interface IAsyncDiscriminationTreeNode<TValue>
{
    /// <summary>
    /// Gets the value attached to a leaf node. Will not be called for internal nodes - throwing an exception is acceptable behaviour for them.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Get the child nodes of this node, keyed by objects that describe the element represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<IDiscriminationTreeNodeKey, IAsyncDiscriminationTreeNode<TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by its <see cref="IDiscriminationTreeNodeKey"/> key.
    /// </summary>
    /// <param name="elementInfo">The element info of the child to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncDiscriminationTreeNode<TValue>?> TryGetChildAsync(IDiscriminationTreeNodeKey elementInfo);

    /// <summary>
    /// Gets or adds an internal child of this node.
    /// </summary>
    /// <param name="elementInfo">The element info for the retrieved or added node.</param>
    /// <returns>A task, the result of which is the retrieved or added node.</returns>
    ValueTask<IAsyncDiscriminationTreeNode<TValue>> GetOrAddInternalChildAsync(IDiscriminationTreeNodeKey elementInfo);

    /// <summary>
    /// Adds a child node of this node that is a leaf.
    /// </summary>
    /// <param name="elementInfo">The element info for the added node.</param>
    /// <param name="value">The value to be attached to the new node.</param>
    /// <returns>A task representing the completion of this operation.</returns>
    ValueTask AddLeafChildAsync(IDiscriminationTreeNodeKey elementInfo, TValue value);
}
