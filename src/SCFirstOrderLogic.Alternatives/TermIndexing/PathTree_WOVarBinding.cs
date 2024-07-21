// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// <para>
/// An implementation of a path tree for <see cref="Term"/>s.
/// </para>
/// <para>
/// ALTERNATIVE: Lacks the variable binding consistency checks of the production version,
/// so will erroneously suggest that e.g. F(C1, C2) is an instance of F(X, X). But of 
/// course this comes with a performance improvement.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
public class PathTree_WOVarBinding<TValue>
{
    private readonly IPathTreeParameterNode<TValue> root;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree_WOVarBinding{TValue}"/> class.
    /// </summary>
    public PathTree_WOVarBinding()
        : this(new PathTreeDictionaryNode<TValue>(), Array.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree_WOVarBinding{TValue}"/> class with a specified root node.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    public PathTree_WOVarBinding(IPathTreeParameterNode<TValue> rootNode)
        : this(rootNode, Array.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree_WOVarBinding{TValue}"/> class with some initial content.
    /// </summary>
    /// <param name="content">The initial content of the tree.</param>
    public PathTree_WOVarBinding(IEnumerable<KeyValuePair<Term, TValue>> content)
        : this(new PathTreeDictionaryNode<TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree_WOVarBinding{TValue}"/> class with a specified root node and some initial content.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="content">The initial content of the tree.</param>
    public PathTree_WOVarBinding(IPathTreeParameterNode<TValue> rootNode, IEnumerable<KeyValuePair<Term, TValue>> content)
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

        IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
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

        IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IPathTreeArgumentNode<TValue> node, Term term)
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

        term = term.Ordinalise();
        return ExpandParameterNode(root, term).IntersectAll();

        IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
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

        IEnumerable<IEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IPathTreeArgumentNode<TValue> node, Term term)
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

        IEnumerable<KeyValuePair<Term, TValue>> ExpandVariableMatches(IReadOnlyDictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> nodes)
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
        return ExpandParameterNode(root, term);

        IEnumerable<KeyValuePair<Term, TValue>> ExpandParameterNode(IPathTreeParameterNode<TValue> node, Term term)
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

        IEnumerable<KeyValuePair<Term, TValue>> ExpandArgumentNode(IPathTreeArgumentNodeKey key, IPathTreeArgumentNode<TValue> node, Term term)
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
