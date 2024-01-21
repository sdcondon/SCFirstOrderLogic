// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
// For this async case, a key change to consider here is kicking off the exploration of multiple
// branches at the same time (within some specified parallelism limit).
public class AsyncPathTree<TValue>
{
    private readonly IAsyncPathTreeParameterNode<TValue> root;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPathTree{TValue}"/> class with a specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    public AsyncPathTree(IAsyncPathTreeParameterNode<TValue> rootNode)
        : this(rootNode, Array.Empty<KeyValuePair<Term, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPathTree{TValue}"/> class with a specified root node and some initial content.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="content">The initial content of the tree.</param>
    public AsyncPathTree(IAsyncPathTreeParameterNode<TValue> rootNode, IEnumerable<KeyValuePair<Term, TValue>> content)
    {
        this.root = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
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

        term = term.Ordinalise();
        await term.Accept(new TermAdditionVisitor(term, value), root);
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a specific term.
    /// </summary>
    /// <param name="term">The term to retrieve the associated value of.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public async Task<(bool isSucceeded, TValue? value)> TryGetExactAsync(Term term)
    {
        ArgumentNullException.ThrowIfNull(term);

        term = term.Ordinalise();
        var (key, value) = await ExpandParameterNode(root, term).TryGetCommonValueAsync();
        return (key, value.Value);

        static async IAsyncEnumerable<IAsyncEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IAsyncPathTreeParameterNode<TValue> node, Term term)
        {
            var child = await node.TryGetChildAsync(term.ToNodeKey());

            if (child != null)
            {
                await foreach (var values in ExpandArgumentNode(child, term))
                {
                    yield return values;
                }
            }
            else
            {
                yield return EmptyAsyncEnumerable<KeyValuePair<Term, TValue>>.Instance;
            }
        }

        static async IAsyncEnumerable<IAsyncEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IAsyncPathTreeArgumentNode<TValue> node, Term term)
        {
            if (term is Function function && function.Arguments.Count > 0)
            {
                for (var i = 0; i < function.Arguments.Count; i++)
                {
                    await foreach (var values in ExpandParameterNode(await node.GetChildAsync(i), function.Arguments[i]))
                    {
                        yield return values;
                    }
                }
            }
            else
            {
                yield return node.GetValues();
            }
        }
    }

    /// <summary>
    /// Retrieves all values associated with instances of a given term. That is, all values associated with
    /// terms that can be obtained from the given term by applying a variable substitution to it.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
    public IAsyncEnumerable<KeyValuePair<Term, TValue>> GetInstances(Term term)
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

        static async IAsyncEnumerable<IAsyncEnumerable<KeyValuePair<Term, TValue>>> ExpandParameterNode(IAsyncPathTreeParameterNode<TValue> node, Term term)
        {
            if (term is VariableReference variable)
            {
                yield return ExpandVariableMatches(node.GetChildren());
            }
            else
            {
                var child = await node.TryGetChildAsync(term.ToNodeKey());

                if (child != null)
                {
                    await foreach (var values in ExpandArgumentNode(child, term))
                    {
                        yield return values;
                    }
                }
                else
                {
                    yield return EmptyAsyncEnumerable<KeyValuePair<Term, TValue>>.Instance;
                }
            }
        }

        static async IAsyncEnumerable<IAsyncEnumerable<KeyValuePair<Term, TValue>>> ExpandArgumentNode(IAsyncPathTreeArgumentNode<TValue> node, Term term)
        {
            if (term is Function function && function.Arguments.Count > 0)
            {
                for (var i = 0; i < function.Arguments.Count; i++)
                {
                    await foreach (var values in ExpandParameterNode(await node.GetChildAsync(i), function.Arguments[i]))
                    {
                        yield return values;
                    }
                }
            }
            else
            {
                yield return node.GetValues();
            }
        }

        static async IAsyncEnumerable<KeyValuePair<Term, TValue>> ExpandVariableMatches(IAsyncEnumerable<KeyValuePair<IPathTreeArgumentNodeKey, IAsyncPathTreeArgumentNode<TValue>>> nodes)
        {
            await foreach (var (key, node) in nodes)
            {
                if (key.ChildElementCount > 0)
                {
                    for (var i = 0; i < key.ChildElementCount; i++)
                    {
                        await foreach (var value in ExpandVariableMatches((await node.GetChildAsync(i)).GetChildren()))
                        {
                            yield return value;
                        }
                    }
                }
                else
                {
                    await foreach (var value in node.GetValues())
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
    public IAsyncEnumerable<KeyValuePair<Term, TValue>> GetGeneralisations(Term term)
    {
        ArgumentNullException.ThrowIfNull(term);

        term = term.Ordinalise();
        return ExpandParameterNode(root, term).Where(a => term.IsInstanceOf(a.Key));

        static async IAsyncEnumerable<KeyValuePair<Term, TValue>> ExpandParameterNode(IAsyncPathTreeParameterNode<TValue> node, Term term)
        {
            await foreach (var (childKey, childNode) in node.GetChildren())
            {
                if (childKey is PathTreeVariableNodeKey || childKey.Equals(term.ToNodeKey()))
                {
                    await foreach (var value in ExpandArgumentNode(childKey, childNode, term))
                    {
                        yield return value;
                    }
                }
            }
        }

        static IAsyncEnumerable<KeyValuePair<Term, TValue>> ExpandArgumentNode(IPathTreeArgumentNodeKey key, IAsyncPathTreeArgumentNode<TValue> node, Term term)
        {
            if (key.ChildElementCount > 0)
            {
                return node.GetChildren()
                    .Select((n, i) => ExpandParameterNode(n, (term as Function)!.Arguments[i]))
                    .IntersectAll();
            }
            else
            {
                return node.GetValues();
            }
        }
    }

    // Not yet, but shouldn't be *too* tough - essentially just need to combine instances and generalisations logic.
    ////public IEnumerable<KeyValuePair<Term, TValue>> GetUnifications(Term term)

    /// <summary>
    /// Term visitor that adds the term as descendents of the path tree parameter node passed as visitation state.
    /// </summary>
    private class TermAdditionVisitor : ITermTransformation<ValueTask, IAsyncPathTreeParameterNode<TValue>>
    {
        private readonly TValue value;
        private readonly Term term;

        public TermAdditionVisitor(Term term, TValue value) => (this.term, this.value) = (term, value);

        public async ValueTask ApplyTo(Constant constant, IAsyncPathTreeParameterNode<TValue> state)
        {
            var node = await state.GetOrAddChildAsync(new PathTreeConstantNodeKey(constant.Identifier));
            await node.AddValueAsync(term, value);
        }

        public async ValueTask ApplyTo(Function function, IAsyncPathTreeParameterNode<TValue> state)
        {
            var functionArgCount = function.Arguments.Count;
            var node = await state.GetOrAddChildAsync(new PathTreeFunctionNodeKey(function.Identifier, functionArgCount));
            if (functionArgCount > 0)
            {
                for (int i = 0; i < function.Arguments.Count; i++)
                {
                    var parameterNode = await node.GetOrAddChildAsync(i);
                    await function.Arguments[i].Accept(this, parameterNode);
                }
            }
            else
            {
                await node.AddValueAsync(term, value);
            }
        }

        public async ValueTask ApplyTo(VariableReference variable, IAsyncPathTreeParameterNode<TValue> state)
        {
            var node = await state.GetOrAddChildAsync(new PathTreeVariableNodeKey((int)variable.Identifier));
            await node.AddValueAsync(term, value);
        }
    }
}
