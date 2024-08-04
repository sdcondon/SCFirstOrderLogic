// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// <para>
/// An implementation of a feature vector index for <see cref="CNFClause"/>s. Specifically, one for which the associated values are the clauses themselves.
/// </para>
/// <para>
/// Feature vector indexing is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TKeyElement">The type of the keys of the feature vectors.</typeparam>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
// TODO: Make a decision on how to handle comparability need for keys (to deal with implied zero values for absent keys)
public class FeatureVectorIndex<TKeyElement>
    where TKeyElement : notnull
{
    private readonly FeatureVectorIndex<TKeyElement, CNFClause> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector)
    {
        actualTree = new(featureVectorSelector);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IComparer<TKeyElement> elementComparer)
    {
        actualTree = new(featureVectorSelector, elementComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TKeyElement, CNFClause> root)
    {
        actualTree = new(featureVectorSelector, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TKeyElement,TValue}"/> root node and some initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="content">The initial content to be added to the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IEnumerable<CNFClause> content)
    {
        actualTree = new(featureVectorSelector, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IComparer<TKeyElement> elementComparer,
        IFeatureVectorIndexNode<TKeyElement, CNFClause> root)
    {
        actualTree = new(featureVectorSelector, elementComparer, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IComparer<TKeyElement> elementComparer,
        IEnumerable<CNFClause> content)
    {
        actualTree = new(featureVectorSelector, elementComparer, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default 
    /// comparer of the key element type to determine the ordering of elements in the tree.
    /// in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TKeyElement, CNFClause> root,
        IEnumerable<CNFClause> content)
    {
        actualTree = new(featureVectorSelector, root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TKeyElement, int>>> featureVectorSelector,
        IComparer<TKeyElement> elementComparer,
        IFeatureVectorIndexNode<TKeyElement, CNFClause> root,
        IEnumerable<CNFClause> content)
    {
        actualTree = new(featureVectorSelector, elementComparer, root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Adds a set to the trie.
    /// </summary>
    /// <param name="key">The set to add.</param>
    public void Add(CNFClause key)
    {
        actualTree.Add(key, key);
    }

    /// <summary>
    /// Removes a set from the trie.
    /// </summary>
    /// <param name="key">The set to remove.</param>
    /// <returns>A value indicating whether the set was present prior to this operation.</returns>
    public bool Remove(CNFClause key)
    {
        return actualTree.Remove(key);
    }

    /// <summary>
    /// Determines whether a given set (matched exactly) is present in the trie.
    /// </summary>
    /// <param name="key">The set to check for.</param>
    /// <returns>True if and only if the set is present in the trie.</returns>
    public bool Contains(CNFClause key) => actualTree.TryGet(key, out _);

    /// <summary>
    /// Returns an enumerable of each stored subset of a given set.
    /// </summary>
    /// <param name="key">The stored subsets of this set will be retrieved.</param>
    /// <returns>An enumerable each stored subset of the given set.</returns>
    public IEnumerable<CNFClause> GetSubsets(CNFClause key)
    {
        return actualTree.GetSubsets(key);
    }

    /// <summary>
    /// Returns an enumerable of teach stored superset a given set.
    /// </summary>
    /// <param name="key">The stored supersets of this set will be retrieved.</param>
    /// <returns>An enumerable of each stored superset the given set.</returns>
    public IEnumerable<CNFClause> GetSupersets(CNFClause key)
    {
        return actualTree.GetSupersets(key);
    }
}
