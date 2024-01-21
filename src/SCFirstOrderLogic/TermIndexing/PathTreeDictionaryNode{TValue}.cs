// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// An implementation of <see cref="IPathTreeParameterNode{TValue}"/> that stores everything in memory.
    /// </summary>
    public sealed class PathTreeDictionaryNode<TValue> : IPathTreeParameterNode<TValue>
    {
        private readonly Dictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> children = new();

        /// <inheritdoc/>
        // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
        // users from casting. Would be more mem for a real edge case.. 
        public IReadOnlyDictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> Children => children;

        /// <inheritdoc/>
        public IPathTreeArgumentNode<TValue> GetOrAddChild(IPathTreeArgumentNodeKey key)
        {
            if (!children.TryGetValue(key, out var node))
            {
                node = key.ChildElementCount > 0 ? new InternalNode() : new LeafNode();
                children.Add(key, node);
            }

            return node;
        }

        /// <summary>
        /// An <see cref="IPathTreeArgumentNode{TValue}"/> that represents a term element that has children -
        /// i.e. a function with a non-zero number of parameters.
        /// </summary>
        private sealed class InternalNode : IPathTreeArgumentNode<TValue>
        {
            private readonly List<PathTreeDictionaryNode<TValue>> children = new();

            // NB: we don't bother wrapping children in a read-only class to stop unscrupulous
            // users from casting. Would be more mem for a real edge case.. 
            public IReadOnlyList<IPathTreeParameterNode<TValue>> Children => children;

            public IEnumerable<KeyValuePair<Term, TValue>> Values => throw new NotSupportedException("Internal node - has no values");

            public IPathTreeParameterNode<TValue> GetOrAddChild(int index)
            {
                while (children.Count <= index)
                {
                    children.Add(new PathTreeDictionaryNode<TValue>());
                }

                return children[index];
            }

            public void AddValue(Term term, TValue value)
            {
                throw new NotSupportedException("Internal node - cannot have value");
            }
        }

        /// <summary>
        /// An <see cref="IPathTreeArgumentNode{TValue}"/> that represents a term element that has no children -
        /// a variable reference, a constant, or a function with no parameters.
        /// </summary>
        private sealed class LeafNode : IPathTreeArgumentNode<TValue>
        {
            private static readonly ReadOnlyCollection<PathTreeDictionaryNode<TValue>> emptyChildren = new(Array.Empty<PathTreeDictionaryNode<TValue>>());
            private readonly Dictionary<Term, TValue> values = new();

            public IReadOnlyList<IPathTreeParameterNode<TValue>> Children => emptyChildren;

            // NB: we don't bother wrapping values in a read-only class to stop unscrupulous
            // users from casting. Would be more mem for a real edge case.
            public IEnumerable<KeyValuePair<Term, TValue>> Values => values;

            public IPathTreeParameterNode<TValue> GetOrAddChild(int index)
            {
                throw new NotSupportedException("Leaf node - cannot have children");
            }

            public void AddValue(Term term, TValue value)
            {
                if (!values.TryAdd(term, value))
                {
                    throw new ArgumentException("Key already present", nameof(term));
                }
            }
        }
    }
}
