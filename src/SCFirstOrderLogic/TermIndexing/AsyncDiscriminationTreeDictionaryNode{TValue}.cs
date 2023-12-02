// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing
{
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
        private readonly Dictionary<IDiscriminationTreeElementInfo, IAsyncDiscriminationTreeNode<TValue>> children = new();

        /// <inheritdoc/>
        public TValue Value => throw new NotSupportedException("Internal node - has no value");

        /// <inheritdoc/>
        public async IAsyncEnumerable<KeyValuePair<IDiscriminationTreeElementInfo, IAsyncDiscriminationTreeNode<TValue>>> GetChildren()
        {
            foreach (var child in children)
            {
                yield return child;
            }
        }

        /// <inheritdoc/>
        public Task<IAsyncDiscriminationTreeNode<TValue>?> TryGetChildAsync(IDiscriminationTreeElementInfo elementInfo)
        {
            children.TryGetValue(elementInfo, out var child);
            return Task.FromResult(child);
        }

        /// <inheritdoc/>
        public Task<IAsyncDiscriminationTreeNode<TValue>> GetOrAddInternalChildAsync(IDiscriminationTreeElementInfo elementInfo)
        {
            if (!children.TryGetValue(elementInfo, out var node))
            {
                node = new AsyncDiscriminationTreeDictionaryNode<TValue>();
                children.Add(elementInfo, node);
            }

            return Task.FromResult(node);
        }

        /// <inheritdoc/>
        public Task AddLeafChildAsync(IDiscriminationTreeElementInfo elementInfo, TValue value)
        {
            if (!children.TryAdd(elementInfo, new LeafNode(value)))
            {
                throw new ArgumentException("Key already present", nameof(elementInfo));
            }

            return Task.CompletedTask;
        }

        private class LeafNode : IAsyncDiscriminationTreeNode<TValue>
        {
            internal LeafNode(TValue value) => Value = value;

            public TValue Value { get; }

            public async IAsyncEnumerable<KeyValuePair<IDiscriminationTreeElementInfo, IAsyncDiscriminationTreeNode<TValue>>> GetChildren()
            {
                yield break;
            }

            public Task<IAsyncDiscriminationTreeNode<TValue>?> TryGetChildAsync(IDiscriminationTreeElementInfo elementInfo)
            {
                return Task.FromResult<IAsyncDiscriminationTreeNode<TValue>?>(null);
            }

            public Task<IAsyncDiscriminationTreeNode<TValue>> GetOrAddInternalChildAsync(IDiscriminationTreeElementInfo elementInfo)
            {
                return Task.FromException<IAsyncDiscriminationTreeNode<TValue>>(new NotSupportedException("Leaf node - cannot have children"));
            }

            public Task AddLeafChildAsync(IDiscriminationTreeElementInfo elementInfo, TValue value)
            {
                return Task.FromException(new NotSupportedException("Leaf node - cannot have children"));
            }
        }
    }
    #pragma warning restore CS1998
}
