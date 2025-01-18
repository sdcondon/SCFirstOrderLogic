// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncDiscriminationTreeNode{TValue}"/> that just stores its content using an in-memory dictionary.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider using <see cref="DiscriminationTree{TValue}"/> to avoid the overhead of asynchronicity.
/// <see cref="AsyncDiscriminationTree{TValue}"/> is intended to facilitate indices that use secondary storage - this type is primarily
/// intended as an example implementation to base real (secondary storage utilising) implementations on.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
public class AsyncDiscriminationTreeDictionaryNode<TValue> : IAsyncDiscriminationTreeNode<TValue>
{
    private readonly ConcurrentDictionary<IDiscriminationTreeNodeKey, IAsyncDiscriminationTreeNode<TValue>> children = new();

    /// <inheritdoc/>
    public TValue Value => throw new NotSupportedException("Internal node - has no value");

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<IDiscriminationTreeNodeKey, IAsyncDiscriminationTreeNode<TValue>>> GetChildren()
    {
        foreach (var child in children)
        {
            yield return child;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncDiscriminationTreeNode<TValue>?> TryGetChildAsync(IDiscriminationTreeNodeKey elementInfo)
    {
        children.TryGetValue(elementInfo, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncDiscriminationTreeNode<TValue>> GetOrAddInternalChildAsync(IDiscriminationTreeNodeKey elementInfo)
    {
        IAsyncDiscriminationTreeNode<TValue> node = new AsyncDiscriminationTreeDictionaryNode<TValue>();
        if (!children.TryAdd(elementInfo, node))
        {
            node = children[elementInfo];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask AddLeafChildAsync(IDiscriminationTreeNodeKey elementInfo, TValue value)
    {
        if (!children.TryAdd(elementInfo, new LeafNode(value)))
        {
            throw new ArgumentException("Key already present", nameof(elementInfo));
        }

        return ValueTask.CompletedTask;
    }

    private class LeafNode : IAsyncDiscriminationTreeNode<TValue>
    {
        internal LeafNode(TValue value) => Value = value;

        public TValue Value { get; }

        public async IAsyncEnumerable<KeyValuePair<IDiscriminationTreeNodeKey, IAsyncDiscriminationTreeNode<TValue>>> GetChildren()
        {
            yield break;
        }

        public ValueTask<IAsyncDiscriminationTreeNode<TValue>?> TryGetChildAsync(IDiscriminationTreeNodeKey elementInfo)
        {
            return ValueTask.FromResult<IAsyncDiscriminationTreeNode<TValue>?>(null);
        }

        public ValueTask<IAsyncDiscriminationTreeNode<TValue>> GetOrAddInternalChildAsync(IDiscriminationTreeNodeKey elementInfo)
        {
            return ValueTask.FromException<IAsyncDiscriminationTreeNode<TValue>>(new NotSupportedException("Leaf node - cannot have children"));
        }

        public ValueTask AddLeafChildAsync(IDiscriminationTreeNodeKey elementInfo, TValue value)
        {
            return ValueTask.FromException(new NotSupportedException("Leaf node - cannot have children"));
        }
    }
}
#pragma warning restore CS1998

