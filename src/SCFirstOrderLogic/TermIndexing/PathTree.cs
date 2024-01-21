// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// <para>
/// An implementation of a path tree for <see cref="Term"/>s - specifically, one for which the attached values are the terms themselves.
/// </para>
/// <para>
/// A path tree's structure is essentially the same as the structure of terms themselves. That is, a path tree is
/// essentially all of its stored terms merged together - with extra branching at (the root and) each function
/// parameter, to allow for all of the different argument values that have been stored. Attached to each leaf
/// of a path tree is a list of all (values associated with the) terms in which the leaf's path appears.
/// Path trees are good for looking up instances of a query term.
/// </para>
/// </summary>
public class PathTree
{
    private readonly PathTree<Term> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree"/> class.
    /// </summary>
    public PathTree()
    {
        actualTree = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree"/> class with a specific root node.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    public PathTree(IPathTreeParameterNode<Term> rootNode)
    {
        actualTree = new(rootNode);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree"/> class with some initial content.
    /// </summary>
    /// <param name="content">The initial content of the tree.</param>
    public PathTree(IEnumerable<Term> content)
    {
        actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathTree"/> class with a specific root node and some initial content.
    /// </summary>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="content">The initial content of the tree.</param>
    public PathTree(IPathTreeParameterNode<Term> rootNode, IEnumerable<Term> content)
    {
        actualTree = new(rootNode, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Adds a <see cref="Term"/> to the tree.
    /// </summary>
    /// <param name="term">The term to add.</param>
    public void Add(Term term) => actualTree.Add(term, term);

    /// <summary>
    /// Determines whether an exact match to a given term is contained within the tree.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>True if and only if the term is contained within the tree.</returns>
    public bool Contains(Term term) => actualTree.TryGetExact(term, out _);

    /// <summary>
    /// Retrieves all instances of a given term. That is, all terms that can be
    /// obtained from the given term by applying a variable substitution to it.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IEnumerable<Term> GetInstances(Term term) => actualTree.GetInstances(term).Select(kvp => kvp.Value);

    /// <summary>
    /// Retrieves all generalisations of a given term. That is, all terms from which
    /// the given term can be obtained by applying a variable substitution to them.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IEnumerable<Term> GetGeneralisations(Term term) => actualTree.GetGeneralisations(term).Select(kvp => kvp.Value);
}
