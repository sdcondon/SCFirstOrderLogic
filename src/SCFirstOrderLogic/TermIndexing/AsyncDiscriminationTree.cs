// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// An implementation of a discrimination tree for <see cref="Term"/>s - specifically, one for which the attached values are the terms themselves.
/// </summary>
/// <seealso href="https://www.google.com/search?q=discrimination+tree"/>
public class AsyncDiscriminationTree
{
    private readonly AsyncDiscriminationTree<Term> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDiscriminationTree"/> class that is empty to begin with.
    /// </summary>
    public AsyncDiscriminationTree(IAsyncDiscriminationTreeNode<Term> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDiscriminationTree"/> class with some initial content.
    /// </summary>
    public AsyncDiscriminationTree(IAsyncDiscriminationTreeNode<Term> root, IEnumerable<Term> content)
    {
        actualTree = new(root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Adds a <see cref="Term"/> to the tree.
    /// </summary>
    /// <param name="term">The term to add.</param>
    public Task AddAsync(Term term) => actualTree.AddAsync(term, term);

    /// <summary>
    /// Determines whether an exact match to a given term is contained within the tree.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>True if and only if the term is contained within the tree.</returns>
    public Task<bool> ContainsAsync(Term term) => actualTree.ContainsAsync(term);

    /// <summary>
    /// Retrieves all instances of a given term. That is, all terms that can be
    /// obtained from the given term by applying a variable substitution to it.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IAsyncEnumerable<Term> GetInstances(Term term) => actualTree.GetInstances(term);

    /// <summary>
    /// Retrieves all generalisations of a given term. That is, all terms from which
    /// the given term can be obtained by applying a variable substitution to them.
    /// </summary>
    /// <param name="term">The term to query for.</param>
    /// <returns>An enumerable of all matching terms.</returns>
    public IAsyncEnumerable<Term> GetGeneralisations(Term term) => actualTree.GetGeneralisations(term);
}
