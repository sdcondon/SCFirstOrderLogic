// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// <para>
/// An implementation of a path tree for <see cref="Term"/>s.
/// </para>
/// <para>
/// A path tree's structure is essentially the same as the structure of terms themselves. That is, a path tree is
/// essentially all of its stored terms merged together - with extra branching at (the root and) each function
/// parameter, to allow for all of the different argument values that have been stored. Attached to each leaf
/// of a path tree is a list of all (values associated with the) terms in which the leaf's path appears.
/// Path trees are good for looking up instances of a query term.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
public class PathTree<TValue>
{
    private readonly IPathTreeParameterNode<TValue> root;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree{TValue}"/> class with a new <see cref="PathTreeDictionaryNode{TValue}"/> root node and no initial content.
    /// </summary>
    public PathTree()
        : this(new PathTreeDictionaryNode<TValue>(), Array.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree{TValue}"/> class with a specified root node.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    public PathTree(IPathTreeParameterNode<TValue> rootNode)
        : this(rootNode, Array.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree{TValue}"/> class with a new <see cref="PathTreeDictionaryNode{TValue}"/> root node and some initial content.
    /// </summary>
    /// <param name="content">The initial content of the tree.</param>
    public PathTree(IEnumerable<KeyValuePair<Term, TValue>> content)
        : this(new PathTreeDictionaryNode<TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree{TValue}"/> class with a specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public PathTree(IPathTreeParameterNode<TValue> rootNode, IEnumerable<KeyValuePair<Term, TValue>> content)
    {
        this.root = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
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

        term = term.Ordinalise();
        term.Accept(new TermAdditionVisitor(term, value), root);
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a specific term.
    /// </summary>
    /// <param name="term">The term to retrieve the associated value of.</param>
    /// <param name="value">Will be populated with the retrieved value.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public bool TryGetExact(Term term, [MaybeNullWhen(false)] out KeyValuePair<Term, TValue> value)
    {
        ArgumentNullException.ThrowIfNull(term);

        term = term.Ordinalise();
        return ExpandParameterNode(root, term).TryGetCommonValue(out value);

        static IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
        {
            if (node.Children.TryGetValue(term.ToNodeKey(), out var child))
            {
                foreach (var values in ExpandArgumentNode(child, term))
                {
                    yield return values;
                }
            }
            else
            {
                yield return Enumerable.Empty<KeyValuePair<Term, TValue>>();
            }
        }

        static IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IPathTreeArgumentNode<TValue> node, Term term)
        {
            if (term is Function function && function.Arguments.Count > 0)
            {
                for (var i = 0; i < function.Arguments.Count; i++)
                {
                    foreach (var values in ExpandParameterNode(node.Children[i], function.Arguments[i]))
                    {
                        yield return values;
                    }
                }
            }
            else
            {
                yield return node.Values;
            }
        }
    }

    /// <summary>
    /// Retrieves all values associated with instances of a given term. That is, all values associated with
    /// terms that can be obtained from the given term by applying a variable substitution to it.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
    public IEnumerable<KeyValuePair<Term, TValue>> GetInstances(Term term)
    {
        ArgumentNullException.ThrowIfNull(term);

        // TODO-PERFORMANCE: yeah, just filtering on non-unifiable stuff at the end probably less efficient
        // than it could be - should be able to figure out non-matches within the recursion? Though
        // reconstructing tree terms bound to variables in the query on the way "out" would be a bit awkward.
        // Revisit me!
        term = term.Ordinalise();
        return ExpandParameterNode(root, term)
            .IntersectAll()
            .Where(a => a.Key.IsInstanceOf(term));

        static IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
        {
            if (term is VariableReference variable)
            {
                yield return ExpandVariableMatches(node.Children).Distinct();
            }
            else if (node.Children.TryGetValue(term.ToNodeKey(), out var child))
            {
                foreach (var values in ExpandArgumentNode(child, term))
                {
                    yield return values;
                }
            }
            else
            {
                yield return Enumerable.Empty<KeyValuePair<Term, TValue>>();
            }
        }

        static IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IPathTreeArgumentNode<TValue> node, Term term)
        {
            if (term is Function function && function.Arguments.Count > 0)
            {
                for (var i = 0; i < function.Arguments.Count; i++)
                {
                    foreach (var values in ExpandParameterNode(node.Children[i], function.Arguments[i]))
                    {
                        yield return values;
                    }
                }
            }
            else
            {
                yield return node.Values;
            }
        }

        static IEnumerable<KeyValuePair<Term, TValue>> ExpandVariableMatches(IReadOnlyDictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> nodes)
        {
            foreach (var (key, node) in nodes)
            {
                if (key.ChildElementCount > 0)
                {
                    for (var i = 0; i < key.ChildElementCount; i++)
                    {
                        foreach (var value in ExpandVariableMatches(node.Children[i].Children))
                        {
                            yield return value;
                        }
                    }
                }
                else
                {
                    foreach (var value in node.Values)
                    {
                        yield return value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retrieves all values associated with generalisations of a given term. That is, all values associated with
    /// terms from which the given term can be obtained by applying a variable substitution to them.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
    public IEnumerable<KeyValuePair<Term, TValue>> GetGeneralisations(Term term)
    {
        ArgumentNullException.ThrowIfNull(term);

        term = term.Ordinalise();
        return ExpandParameterNode(root, term).Where(a => term.IsInstanceOf(a.Key));

        static IEnumerable<KeyValuePair<Term, TValue>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
        {
            foreach (var (childKey, childNode) in node.Children)
            {
                if (childKey is PathTreeVariableNodeKey || childKey.Equals(term.ToNodeKey()))
                {
                    foreach (var value in ExpandArgumentNode(childKey, childNode, term))
                    {
                        yield return value;
                    }
                }
            }
        }

        static IEnumerable<KeyValuePair<Term, TValue>> ExpandArgumentNode(IPathTreeArgumentNodeKey key, IPathTreeArgumentNode<TValue> node, Term term)
        {
            if (key.ChildElementCount > 0)
            {
                return node.Children
                    .Select((n, i) => ExpandParameterNode(n, (term as Function)!.Arguments[i]))
                    .IntersectAll();
            }
            else
            {
                return node.Values;
            }
        }
    }

    // Not yet, but shouldn't be *too* tough - essentially just need to combine instances and generalisations logic.
    ////public IEnumerable<KeyValuePair<Term, TValue>> GetUnifications(Term term)

    /// <summary>
    /// Term visitor that adds the term as descendents of the path tree parameter node passed as visitation state.
    /// </summary>
    private class TermAdditionVisitor : ITermVisitor<IPathTreeParameterNode<TValue>>
    {
        private readonly TValue value;
        private readonly Term term;

        public TermAdditionVisitor(Term term, TValue value) => (this.term, this.value) = (term, value);

        public void Visit(Function function, IPathTreeParameterNode<TValue> state)
        {
            var functionArgCount = function.Arguments.Count;
            var node = state.GetOrAddChild(new PathTreeFunctionNodeKey(function.Identifier, functionArgCount));
            if (functionArgCount > 0)
            {
                for (int i = 0; i < function.Arguments.Count; i++)
                {
                    var parameterNode = node.GetOrAddChild(i);
                    function.Arguments[i].Accept(this, parameterNode);
                }
            }
            else
            {
                node.AddValue(term, value);
            }
        }

        public void Visit(VariableReference variable, IPathTreeParameterNode<TValue> state)
        {
            var node = state.GetOrAddChild(new PathTreeVariableNodeKey((int)variable.Identifier));
            node.AddValue(term, value);
        }
    }
}
