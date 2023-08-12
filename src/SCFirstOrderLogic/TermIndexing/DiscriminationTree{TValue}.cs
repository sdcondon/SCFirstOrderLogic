// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

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
    public class DiscriminationTree<TValue>
    {
        private readonly IDiscriminationTreeNode<TValue> root;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class.
        /// </summary>
        public DiscriminationTree()
            :  this(new DiscriminationTreeDictionaryNode<TValue>(), Enumerable.Empty<KeyValuePair<Term, TValue>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a specified root node.
        /// </summary>
        public DiscriminationTree(IDiscriminationTreeNode<TValue> root)
            : this(root, Enumerable.Empty<KeyValuePair<Term, TValue>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with some initial content.
        /// </summary>
        public DiscriminationTree(IEnumerable<KeyValuePair<Term, TValue>> content)
            : this(new DiscriminationTreeDictionaryNode<TValue>(), content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a specified root node and some initial content.
        /// </summary>
        public DiscriminationTree(IDiscriminationTreeNode<TValue> root, IEnumerable<KeyValuePair<Term, TValue>> content)
        {
            this.root = root;

            foreach (var kvp in content)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Adds a <see cref="Term"/> to the tree.
        /// </summary>
        /// <param name="term">The term to add.</param>
        /// <param name="value">The value to associate with the added term.</param>
        public void Add(Term term, TValue value)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new DiscriminationTreeElementInfoTransformation().ApplyTo(term).GetEnumerator();
            try
            {
                var currentNode = root;
                IDiscriminationTreeElementInfo? currentQueryElement = queryElements.MoveNext() ? queryElements.Current : null;
                while (currentQueryElement != null)
                {
                    var nextQueryElement = queryElements.MoveNext() ? queryElements.Current : null;

                    if (nextQueryElement != null)
                    {
                        currentNode = currentNode.GetOrAddInternalChild(currentQueryElement);
                    }
                    else
                    {
                        currentNode.AddLeafChild(currentQueryElement, value);
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
        /// <param name="value">Will be populated with the retrieved value.</param>
        /// <returns>True if and only if a value was successfully retrieved.</returns>
        public bool TryGetExact(Term term, out TValue? value)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var currentNode = root;
            foreach (var queryElement in new DiscriminationTreeElementInfoTransformation().ApplyTo(term))
            {
                if (!currentNode.Children.TryGetValue(queryElement, out currentNode!))
                {
                    value = default;
                    return false;
                }
            }

            // NB: can safely grab Value here because the current node must ALWAYS be a LeafNode at this point.
            // It is not possible for prefix element info enumerations to occur, because the the number of
            // elements in the path is always one more than the summation of the ChildElementCounts (NB: used in
            // equality check) of all encountered elements.
            value = currentNode.Value;
            return true;
        }

        /// <summary>
        /// Retrieves all values associated with instances of a given term. That is, all values associated with
        /// terms that can be obtained from the given term by applying a variable substitution to it.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IEnumerable<TValue> GetInstances(Term term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new DiscriminationTreeElementInfoTransformation().ApplyTo(term).ToList();

            IEnumerable<TValue> ExpandNodes(
                IReadOnlyDictionary<IDiscriminationTreeElementInfo, IDiscriminationTreeNode<TValue>> nodes,
                int queryElementIndex,
                VariableBindings variableBindings)
            {
                var queryElement = queryElements[queryElementIndex];

                if (queryElement is DiscriminationTreeVariableInfo)
                {
                    foreach (var value in ExpandVariableMatches(nodes, queryElementIndex, Enumerable.Empty<IDiscriminationTreeElementInfo>(), 1, variableBindings))
                    {
                        yield return value;
                    }
                }
                else if (nodes.TryGetValue(queryElement, out var childNode))
                {
                    foreach (var value in ExpandNode(childNode, queryElementIndex + 1, variableBindings))
                    {
                        yield return value;
                    }
                }
            }

            IEnumerable<TValue> ExpandVariableMatches(
                IReadOnlyDictionary<IDiscriminationTreeElementInfo, IDiscriminationTreeNode<TValue>> nodes,
                int queryElementIndex,
                IEnumerable<IDiscriminationTreeElementInfo> parentVariableMatch,
                int parentUnexploredBranchCount,
                VariableBindings priorVariableBindings)
            {
                foreach (var (elementInfo, node) in nodes)
                {
                    // NB: there's an obvious possible performance improvement here - cache
                    // the descendent nodes we need to jump to rather than figuring it out afresh
                    // each time. i.e. A "jump list". Might implement this at a later date - not a TODO for now.
                    var variableMatch = parentVariableMatch.Append(elementInfo);
                    var unexploredBranchCount = parentUnexploredBranchCount + elementInfo.ChildElementCount - 1;

                    if (unexploredBranchCount > 0)
                    {
                        foreach (var value in ExpandVariableMatches(
                            node.Children,
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

                        var isVariableMatch = VariableBindings.TryAddOrMatchBinding(
                            ((DiscriminationTreeVariableInfo)queryElements[queryElementIndex]).Ordinal,
                            variableMatch.ToArray(),
                            ref variableBindings);

                        if (isVariableMatch)
                        {
                            foreach (var value in ExpandNode(node, queryElementIndex + 1, variableBindings))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }

            IEnumerable<TValue> ExpandNode(
                IDiscriminationTreeNode<TValue> node,
                int queryElementIndex,
                VariableBindings variableBindings)
            {
                if (queryElementIndex < queryElements.Count)
                {
                    foreach (var value in ExpandNodes(node.Children, queryElementIndex, variableBindings))
                    {
                        yield return value;
                    }
                }
                else
                {
                    // We can safely grab Value here because node MUST be a LeafNode at this point - ultimately because of how
                    // ElementInfoTransformation works (which controls both the structure of the tree and queryElements here).
                    yield return node.Value;
                }
            }

            return ExpandNodes(root.Children, 0, new VariableBindings());
        }

        /// <summary>
        /// Retrieves all values associated with generalisations of a given term. That is, all values associated with
        /// terms from which the given term can be obtained by applying a variable substitution to them.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IEnumerable<TValue> GetGeneralisations(Term term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new DiscriminationTreeElementInfoTransformation().ApplyTo(term).ToArray();

            IEnumerable<TValue> ExpandNode(IDiscriminationTreeNode<TValue> node, int queryElementIndex, VariableBindings variableBindings)
            {
                foreach (var (childElement, childNode) in node.Children)
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
                        isVariableMatch = VariableBindings.TryAddOrMatchBinding(
                            variableInfo.Ordinal,
                            queryElements[queryElementIndex..(queryElementIndex + nextQueryElementOffset)],
                            ref childVariableBindings);
                    }

                    if (isVariableMatch || childElement.Equals(queryElements[queryElementIndex]))
                    {
                        var nextQueryElementIndex = queryElementIndex + nextQueryElementOffset;

                        if (nextQueryElementIndex < queryElements.Length)
                        {
                            foreach (var value in ExpandNode(childNode, nextQueryElementIndex, childVariableBindings))
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

            return ExpandNode(root, 0, new VariableBindings());
        }

        // public IEnumerable<TValue> GetUnifications(Term term) -- not yet..
    }
}
