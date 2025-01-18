// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncPathTreeParameterNode{TValue}"/> that just stores its content using an in-memory dictionary.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider using <see cref="PathTree{TValue}"/> to avoid the overhead of asynchronicity.
/// <see cref="AsyncPathTree{TValue}"/> is intended to facilitate indices that use secondary storage - this type is primarily
/// intended as an example implementation to base real (secondary storage utilising) implementations on.
/// </para>
/// </summary>
public sealed class AsyncPathTreeDictionaryNode<TValue> : IAsyncPathTreeParameterNode<TValue>
{
    private readonly ConcurrentDictionary<IPathTreeArgumentNodeKey, IAsyncPathTreeArgumentNode<TValue>> children = new();

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<IPathTreeArgumentNodeKey, IAsyncPathTreeArgumentNode<TValue>>> GetChildren()
    {
        foreach (var child in children)
        {
            yield return child;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncPathTreeArgumentNode<TValue>?> TryGetChildAsync(IPathTreeArgumentNodeKey key)
    {
        children.TryGetValue(key, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncPathTreeArgumentNode<TValue>> GetOrAddChildAsync(IPathTreeArgumentNodeKey key)
    {
        IAsyncPathTreeArgumentNode<TValue> node = key.ChildElementCount > 0 ? new InternalNode() : new LeafNode();
        if (!children.TryAdd(key, node))
        {
            node = children[key];
        }

        return ValueTask.FromResult(node);
    }

    /// <summary>
    /// An <see cref="IAsyncPathTreeArgumentNode{TValue}"/> that represents a term element that has children -
    /// i.e. a function with a non-zero number of parameters.
    /// </summary>
    private sealed class InternalNode : IAsyncPathTreeArgumentNode<TValue>
    {
        private readonly List<IAsyncPathTreeParameterNode<TValue>> children = new();

        public async IAsyncEnumerable<IAsyncPathTreeParameterNode<TValue>> GetChildren()
        {
            foreach (var child in children)
            {
                yield return child;
            }
        }

        public ValueTask<IAsyncPathTreeParameterNode<TValue>> GetChildAsync(int index)
        {
            return ValueTask.FromResult(children[index]);
        }

        public IAsyncEnumerable<KeyValuePair<Term, TValue>> GetValues()
        {
            throw new NotSupportedException("Internal node - has no values");
        }

        public ValueTask<IAsyncPathTreeParameterNode<TValue>> GetOrAddChildAsync(int index)
        {
            while (children.Count <= index)
            {
                children.Add(new AsyncPathTreeDictionaryNode<TValue>());
            }

            return ValueTask.FromResult(children[index]);
        }

        public ValueTask AddValueAsync(Term term, TValue value)
        {
            throw new NotSupportedException("Internal node - cannot have value");
        }
    }

    /// <summary>
    /// An <see cref="IAsyncPathTreeArgumentNode{TValue}"/> that represents a term element that has no children -
    /// a variable reference or a function with no parameters.
    /// </summary>
    private sealed class LeafNode : IAsyncPathTreeArgumentNode<TValue>
    {
        private static readonly ReadOnlyCollection<IAsyncPathTreeParameterNode<TValue>> emptyChildren = new(Array.Empty<AsyncPathTreeDictionaryNode<TValue>>());
        private readonly Dictionary<Term, TValue> values = new();

        public async IAsyncEnumerable<IAsyncPathTreeParameterNode<TValue>> GetChildren()
        {
            foreach (var child in emptyChildren)
            {
                yield return child;
            }
        }

        public ValueTask<IAsyncPathTreeParameterNode<TValue>> GetChildAsync(int index)
        {
            return ValueTask.FromResult(emptyChildren[index]);
        }

        public async IAsyncEnumerable<KeyValuePair<Term, TValue>> GetValues()
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }

        public ValueTask<IAsyncPathTreeParameterNode<TValue>> GetOrAddChildAsync(int index)
        {
            throw new NotSupportedException("Leaf node - cannot have children");
        }

        public async ValueTask AddValueAsync(Term term, TValue value)
        {
            if (!values.TryAdd(term, value))
            {
                throw new ArgumentException("Key already present", nameof(term));
            }
        }
    }
}
#pragma warning restore CS1998

