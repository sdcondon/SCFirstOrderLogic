﻿// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// An implementation of a discrimination tree for <see cref="Term"/>s.
    /// </summary>
    /// <typeparam name="TValue">The type of value attached for each term.</typeparam>
    /// <seealso href="https://www.google.com/search?q=discrimination+tree"/>
    // NB: not a TODO just yet, but - while it's not terrible - there are a few aspects of this
    // class that aren't great from a performance perspective. Notably, while the recursive iterator
    // approach used for the retrieval methods may be easy to understand, it will make a lot of heap
    // allocations - increasing GC pressure. The priority thus far has just been to get it working.
    public class AsyncDiscriminationTree<TValue>
    {
        private readonly IAsyncDiscriminationTreeNode<TValue> root;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDiscriminationTree{TValue}"/> class that is empty to begin with.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        public AsyncDiscriminationTree(IAsyncDiscriminationTreeNode<TValue> root)
            : this(root, Enumerable.Empty<KeyValuePair<Term, TValue>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDiscriminationTree{TValue}"/> class with some initial content.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="content">The initial content to add to the tree.</param>
        public AsyncDiscriminationTree(IAsyncDiscriminationTreeNode<TValue> root, IEnumerable<KeyValuePair<Term, TValue>> content)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            ArgumentNullException.ThrowIfNull(content);

            foreach (var kvp in content)
            {
                AddAsync(kvp.Key, kvp.Value).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Adds a <see cref="Term"/> to the tree.
        /// </summary>
        /// <param name="term">The term to add.</param>
        /// <param name="value">The value to associate with the added term.</param>
        public async Task AddAsync(Term term, TValue value)
        {
            ArgumentNullException.ThrowIfNull(term);
            var queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).GetEnumerator();

            try
            {
                IAsyncDiscriminationTreeNode<TValue> currentNode = root;
                IDiscriminationTreeElementInfo? currentQueryElement = queryElements.MoveNext() ? queryElements.Current : null;
                while (currentQueryElement != null)
                {
                    var nextQueryElement = queryElements.MoveNext() ? queryElements.Current : null;

                    if (nextQueryElement != null)
                    {
                        currentNode = await currentNode.GetOrAddInternalChildAsync(currentQueryElement);
                    }
                    else
                    {
                        await currentNode.AddLeafChildAsync(currentQueryElement, value);
                    }

                    currentQueryElement = nextQueryElement;
                }
            }
            finally
            {
                queryElements.Dispose();
            }
        }

        /// <summary>
        /// Attempts to retrieve the value associated with a specific term.
        /// </summary>
        /// <param name="term">The term to retrieve the associated value of.</param>
        /// <returns>A value tuple indicated whether or not the query found a matching term, and if so, what the value associated with that term is.</returns>
        public async Task<(bool isSucceeded, TValue? value)> TryGetExactAsync(Term term)
        {
            ArgumentNullException.ThrowIfNull(term);

            IAsyncDiscriminationTreeNode<TValue> currentNode = root;
            foreach (var queryElement in new DiscriminationTreeNodeKeyTransformation().ApplyTo(term))
            {
                var child = await currentNode.TryGetChildAsync(queryElement);

                if (child == null)
                {
                    return (false, default);
                }

                currentNode = child!;
            }

            // NB: can safely grab Value here because the current node must ALWAYS be a leaf node at this point.
            // It is not possible for prefix element info enumerations to occur, because the the number of
            // elements in the path is always one more than the summation of the ChildElementCounts (NB: used in
            // equality check) of all encountered elements.
            return (true, currentNode.Value);
        }

        /// <summary>
        /// Retrieves all values associated with instances of a given term. That is, all values associated with
        /// terms that can be obtained from the given term by applying a variable substitution to it.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IAsyncEnumerable<TValue> GetInstances(Term term)
        {
            ArgumentNullException.ThrowIfNull(term);
            List<IDiscriminationTreeElementInfo> queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).ToList();

            async IAsyncEnumerable<TValue> ExpandVariableMatches(
                IAsyncEnumerable<KeyValuePair<IDiscriminationTreeElementInfo, IAsyncDiscriminationTreeNode<TValue>>> nodes,
                int queryElementIndex,
                IEnumerable<IDiscriminationTreeElementInfo> parentVariableMatch,
                int parentUnexploredBranchCount,
                DiscriminationTreeVariableBindings priorVariableBindings)
            {
                await foreach (var (elementInfo, node) in nodes)
                {
                    // NB: there's an obvious possible performance improvement here - cache
                    // the descendent nodes we need to jump to rather than figuring it out afresh
                    // each time. i.e. A "jump list". Might implement this at a later date - not a TODO for now.
                    var variableMatch = parentVariableMatch.Append(elementInfo);
                    var unexploredBranchCount = parentUnexploredBranchCount + elementInfo.ChildElementCount - 1;

                    if (unexploredBranchCount > 0)
                    {
                        await foreach (var value in ExpandVariableMatches(
                            node.GetChildren(),
                            queryElementIndex,
                            variableMatch,
                            unexploredBranchCount,
                            priorVariableBindings))
                        {
                            yield return value;
                        }
                    }
                    else
                    {
                        var variableBindings = priorVariableBindings;

                        var isVariableMatch = DiscriminationTreeVariableBindings.TryAddOrMatchBinding(
                            ((DiscriminationTreeVariableInfo)queryElements[queryElementIndex]).Ordinal,
                            variableMatch.ToArray(),
                            ref variableBindings);

                        if (isVariableMatch)
                        {
                            await foreach (var value in ExpandNode(node, queryElementIndex + 1, variableBindings))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }

            async IAsyncEnumerable<TValue> ExpandNode(
                IAsyncDiscriminationTreeNode<TValue> node,
                int queryElementIndex,
                DiscriminationTreeVariableBindings variableBindings)
            {
                if (queryElementIndex < queryElements.Count)
                {
                    var queryElement = queryElements[queryElementIndex];

                    if (queryElement is DiscriminationTreeVariableInfo)
                    {
                        await foreach (var value in ExpandVariableMatches(node.GetChildren(), queryElementIndex, Enumerable.Empty<IDiscriminationTreeElementInfo>(), 1, variableBindings))
                        {
                            yield return value;
                        }
                    }
                    else
                    {
                        var childNode = await node.TryGetChildAsync(queryElement);

                        if (childNode != null)
                        {
                            await foreach (var value in ExpandNode(childNode!, queryElementIndex + 1, variableBindings))
                            {
                                yield return value;
                            }
                        }
                    }
                }
                else
                {
                    // We can safely grab Value here because node MUST be a LeafNode at this point - ultimately because of how
                    // ElementInfoTransformation works (which controls both the structure of the tree and queryElements here).
                    yield return node.Value;
                }
            }

            return ExpandNode(root, 0, new DiscriminationTreeVariableBindings());
        }

        /// <summary>
        /// Retrieves all values associated with generalisations of a given term. That is, all values associated with
        /// terms from which the given term can be obtained by applying a variable substitution to them.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IAsyncEnumerable<TValue> GetGeneralisations(Term term)
        {
            ArgumentNullException.ThrowIfNull(term);
            var queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).ToArray();

            async IAsyncEnumerable<TValue> ExpandNode(IAsyncDiscriminationTreeNode<TValue> node, int queryElementIndex, DiscriminationTreeVariableBindings variableBindings)
            {
                await foreach (var (childElement, childNode) in node.GetChildren())
                {
                    bool isVariableMatch = false;
                    var nextQueryElementOffset = 1;
                    var childVariableBindings = variableBindings;

                    if (childElement is DiscriminationTreeVariableInfo variableInfo)
                    {
                        // Here we are setting nextQueryInfoOffset to the size of the whole subtree
                        // with queryInfos[queryInfoIndex] at its root (i.e. the subtree that we are
                        // matching to the variable).
                        var unexploredBranchCount = queryElements[queryElementIndex].ChildElementCount;
                        while (unexploredBranchCount > 0)
                        {
                            unexploredBranchCount += queryElements[queryElementIndex + nextQueryElementOffset].ChildElementCount - 1;
                            nextQueryElementOffset++;
                        }

                        // Now we need to verify subtree's consistency with any existing variable binding, add a binding if needed,
                        // and set isVariableMatch appropriately.
                        isVariableMatch = DiscriminationTreeVariableBindings.TryAddOrMatchBinding(
                            variableInfo.Ordinal,
                            queryElements[queryElementIndex..(queryElementIndex + nextQueryElementOffset)],
                            ref childVariableBindings);
                    }

                    if (isVariableMatch || childElement.Equals(queryElements[queryElementIndex]))
                    {
                        var nextQueryElementIndex = queryElementIndex + nextQueryElementOffset;

                        if (nextQueryElementIndex < queryElements.Length)
                        {
                            await foreach (var value in ExpandNode(childNode, nextQueryElementIndex, childVariableBindings))
                            {
                                yield return value;
                            }
                        }
                        else
                        {
                            // We can safely grab Value here because childNode MUST be a LeafNode at this point - ultimately because of how
                            // ElementInfoTransformation works (which controls both the structure of the tree and queryElements here).
                            yield return childNode.Value;
                        }
                    }
                }
            }

            return ExpandNode(root, 0, new DiscriminationTreeVariableBindings());
        }

        // public IEnumerable<TValue> GetUnifications(Term term) -- not yet..
    }
}