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
/// Feature vector indexing (in this context, at least) is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
public class FeatureVectorIndex<TFeature>
    where TFeature : notnull
{
    private readonly FeatureVectorIndex<TFeature, CNFClause> innerIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector)
    {
        innerIndex = new(featureVectorSelector);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer)
    {
        innerIndex = new(featureVectorSelector, featureComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TFeature, CNFClause> root)
    {
        innerIndex = new(featureVectorSelector, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="content">The initial content to be added to the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IFeatureVectorIndexNode<TFeature, CNFClause> root)
    {
        innerIndex = new(featureVectorSelector, featureComparer, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, featureComparer, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default 
    /// comparer of the key element type to determine the ordering of elements in the tree.
    /// in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TFeature, CNFClause> root,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature}"/> class with a 
    /// specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IFeatureVectorIndexNode<TFeature, CNFClause> root,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, featureComparer, root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Adds a clause to the index.
    /// </summary>
    /// <param name="key">The clause to add.</param>
    public void Add(CNFClause key)
    {
        innerIndex.Add(key, key);
    }

    /// <summary>
    /// Removes a clause from the index.
    /// </summary>
    /// <param name="key">The clause to remove.</param>
    /// <returns>A value indicating whether the clause was present prior to this operation.</returns>
    public bool Remove(CNFClause key)
    {
        return innerIndex.Remove(key);
    }

    /// <summary>
    /// Determines whether a given clause (matched exactly) is present in the index.
    /// </summary>
    /// <param name="key">The clause to check for.</param>
    /// <returns>True if and only if the clause is present in the index.</returns>
    public bool Contains(CNFClause key) => innerIndex.TryGet(key, out _);

    /// <summary>
    /// Returns an enumerable of each stored clause that subsumes a given clause.
    /// </summary>
    /// <param name="clause">The stored clauses that subsume this clause will be retrieved.</param>
    /// <returns>An enumerable each stored clause that subsumes the given clause.</returns>
    public IEnumerable<CNFClause> GetSubsuming(CNFClause clause)
    {
        return innerIndex.GetSubsuming(clause);
    }

    /// <summary>
    /// Returns an enumerable of each stored clause that is subsumed by a given clause.
    /// </summary>
    /// <param name="clause">The stored clauses that are subsumed by this clause will be retrieved.</param>
    /// <returns>An enumerable of each stored clause that is subsumed by the given clause.</returns>
    public IEnumerable<CNFClause> GetSubsumed(CNFClause clause)
    {
        return innerIndex.GetSubsumed(clause);
    }
}
