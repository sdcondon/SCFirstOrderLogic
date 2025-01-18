// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// <para>
/// An implementation of a discrimination tree for <see cref="Term"/>s.
/// </para>
/// <para>
/// Each path from root to leaf in a discrimination tree consists of a depth-first traversal of the elements of the term that the leaf represents.
/// Discrimination trees are particularly well-suited to (i.e. performant at) looking up generalisations of a query term.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
// NB: not a TODO just yet, but - while it's not terrible - there are a few aspects of this
// class that aren't great from a performance perspective. Notably, while the recursive iterator
// approach used for the retrieval methods may be easy to understand, it will make a lot of heap
// allocations - increasing GC pressure. The priority thus far has just been to get it working.
public class DiscriminationTree<TValue>
{
    private readonly IDiscriminationTreeNode<TValue> root;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a new <see cref="DiscriminationTreeDictionaryNode{TValue}"/> root node and no initial content.
    /// </summary>
    public DiscriminationTree()
        : this(new DiscriminationTreeDictionaryNode<TValue>(), Enumerable.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a specified root node.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public DiscriminationTree(IDiscriminationTreeNode<TValue> root)
        : this(root, Enumerable.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a new <see cref="DiscriminationTreeDictionaryNode{TValue}"/> root node and some initial content.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public DiscriminationTree(IEnumerable<KeyValuePair<Term, TValue>> content)
        : this(new DiscriminationTreeDictionaryNode<TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with a specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public DiscriminationTree(IDiscriminationTreeNode<TValue> root, IEnumerable<KeyValuePair<Term, TValue>> content)
    {
        this.root = root ?? throw new ArgumentNullException(nameof(root));
        ArgumentNullException.ThrowIfNull(content);

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
        ArgumentNullException.ThrowIfNull(term);

        using var queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).GetEnumerator();

        var currentNode = root;
        IDiscriminationTreeNodeKey? currentQueryElement = queryElements.MoveNext() ? queryElements.Current : null;
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

    /// <summary>
    /// Determines whether an exact match to a given term is contained within the tree.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>True if and only if a value associated with the term is contained within the tree.</returns>
    public bool Contains(Term term)
    {
        return TryGetExact(term, out _);
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a specific term.
    /// </summary>
    /// <param name="term">The term to retrieve the associated value of.</param>
    /// <param name="value">Will be populated with the retrieved value.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public bool TryGetExact(Term term, [MaybeNullWhen(false)] out TValue value)
    {
        ArgumentNullException.ThrowIfNull(term);

        var currentNode = root;
        foreach (var queryElement in new DiscriminationTreeNodeKeyTransformation().ApplyTo(term))
        {
            if (!currentNode.Children.TryGetValue(queryElement, out currentNode!))
            {
                value = default;
                return false;
            }
        }

        // NB: we can safely grab Value here because the current node must ALWAYS be a leaf node at this point.
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
        ArgumentNullException.ThrowIfNull(term);

        var queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).ToList();
        return ExpandNodes(root.Children, 0, new DiscriminationTreeVariableBindings());

        IEnumerable<TValue> ExpandNodes(
            IReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> nodes,
            int queryElementIndex,
            DiscriminationTreeVariableBindings variableBindings)
        {
            var queryElement = queryElements[queryElementIndex];

            if (queryElement is DiscriminationTreeVariableNodeKey)
            {
                foreach (var value in ExpandVariableMatches(nodes, queryElementIndex, Enumerable.Empty<IDiscriminationTreeNodeKey>(), 1, variableBindings))
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
            IReadOnlyDictionary<IDiscriminationTreeNodeKey, IDiscriminationTreeNode<TValue>> nodes,
            int queryElementIndex,
            IEnumerable<IDiscriminationTreeNodeKey> parentVariableMatch,
            int parentUnexploredBranchCount,
            DiscriminationTreeVariableBindings priorVariableBindings)
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

                    var isVariableMatch = DiscriminationTreeVariableBindings.TryAddOrMatchBinding(
                        ((DiscriminationTreeVariableNodeKey)queryElements[queryElementIndex]).Ordinal,
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
            DiscriminationTreeVariableBindings variableBindings)
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
                // We can safely grab Value here because node MUST be a leaf node at this point - ultimately because of how
                // DiscriminationTreeNodeKeyTransformation works (which controls both the structure of the tree and queryElements here).
                yield return node.Value;
            }
        }
    }

    /// <summary>
    /// Retrieves all values associated with generalisations of a given term. That is, all values associated with
    /// terms from which the given term can be obtained by applying a variable substitution to them.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
    public IEnumerable<TValue> GetGeneralisations(Term term)
    {
        ArgumentNullException.ThrowIfNull(term);

        var queryElements = new DiscriminationTreeNodeKeyTransformation().ApplyTo(term).ToArray();
        return ExpandNode(root, 0, new DiscriminationTreeVariableBindings());

        IEnumerable<TValue> ExpandNode(IDiscriminationTreeNode<TValue> node, int queryElementIndex, DiscriminationTreeVariableBindings variableBindings)
        {
            foreach (var (childElement, childNode) in node.Children)
            {
                var isVariableMatch = false;
                var nextQueryElementOffset = 1;
                var childVariableBindings = variableBindings;

                if (childElement is DiscriminationTreeVariableNodeKey variableInfo)
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

                    // Now we need to verify subtree's consistency with any existing variable binding,
                    // add a binding if needed, and set isVariableMatch appropriately.
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
    }

    // public IEnumerable<TValue> GetUnifications(Term term) -- not yet..
}
