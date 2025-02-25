﻿// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// An implementation of <see cref="IDiscriminationTreeNode{TValue}"/> that stores its child nodes using a <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
public class DiscriminationTreeDictionaryNode<TValue> : IDiscriminationTreeNode<TValue>
{
    private readonly Dictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> children = new();

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> Children => children;

    /// <inheritdoc/>
    public TValue Value => throw new NotSupportedException("Internal node - has no value");

    /// <inheritdoc/>
    public IDiscriminationTreeNode<TValue> GetOrAddInternalChild(IDiscriminationTreeNodeKey elementInfo)
    {
        if (!children.TryGetValue(elementInfo, out var node))
        {
            node = new DiscriminationTreeDictionaryNode<TValue>();
            children.Add(elementInfo, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void AddLeafChild(IDiscriminationTreeNodeKey elementInfo, TValue value)
    {
        if (!children.TryAdd(elementInfo, new LeafNode(value)))
        {
            throw new ArgumentException("Key already present", nameof(elementInfo));
        }
    }

    /// <summary>
    /// Representation of a leaf node (that is, a node with an attached value) of a discrimination tree.
    /// </summary>
    private sealed class LeafNode : IDiscriminationTreeNode<TValue>
    {
        private static readonly ReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> emptyChildren = new(new Dictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>>());

        internal LeafNode(TValue value) => Value = value;

        /// <inheritdoc/>
        public IReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> Children => emptyChildren;

        /// <inheritdoc/>
        public TValue Value { get; }

        /// <inheritdoc/>
        public IDiscriminationTreeNode<TValue> GetOrAddInternalChild(IDiscriminationTreeNodeKey elementInfo)
        {
            throw new NotSupportedException("Leaf node - cannot have children");
        }

        /// <inheritdoc/>
        public void AddLeafChild(IDiscriminationTreeNodeKey elementInfo, TValue value)
        {
            throw new NotSupportedException("Leaf node - cannot have children");
        }
    }
}
