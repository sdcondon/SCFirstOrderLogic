// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// <para>
/// An implementation of a discrimination tree for <see cref="Term"/>s - specifically, one for which the attached values are the terms themselves.
/// </para>
/// <para>
/// Each path from root to leaf in a discrimination tree consists of a depth-first traversal of the elements of the term that the leaf represents.
/// Discrimination trees are particularly well-suited to (i.e. performant at) looking up generalisations of a query term.
/// </para>
/// </summary>
public class DiscriminationTree
{
    private readonly DiscriminationTree<Term> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree"/> class with a new <see cref="DiscriminationTreeDictionaryNode{TValue}"/> root node and no initial content.
    /// </summary>
    public DiscriminationTree()
    {
        actualTree = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree"/> class with a specified root node.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public DiscriminationTree(IDiscriminationTreeNode<Term> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree"/> class with a new <see cref="DiscriminationTreeDictionaryNode{TValue}"/> root node and some initial content.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public DiscriminationTree(IEnumerable<Term> content)
    {
        actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscriminationTree"/> class with a specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public DiscriminationTree(IDiscriminationTreeNode<Term> root, IEnumerable<Term> content)
    {
        actualTree = new(root, content.Select(t => KeyValuePair.Create(t, t)));
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
    public bool Contains(Term term) => actualTree.Contains(term);

    /// <summary>
    /// Retrieves all instances of a given term. That is, all terms that can be
    /// obtained from the given term by applying a variable substitution to it.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IEnumerable<Term> GetInstances(Term term) => actualTree.GetInstances(term);

    /// <summary>
    /// Retrieves all generalisations of a given term. That is, all terms from which
    /// the given term can be obtained by applying a variable substitution to them.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IEnumerable<Term> GetGeneralisations(Term term) => actualTree.GetGeneralisations(term);
}
