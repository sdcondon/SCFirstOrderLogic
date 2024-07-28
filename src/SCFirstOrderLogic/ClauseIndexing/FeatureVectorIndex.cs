// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// <para>
/// An implementation of a feature vector index for <see cref="CNFClause"/>s. Specifically, one for which the attached values are the clauses themselves.
/// </para>
/// <para>
/// Feature vector indexing is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public class FeatureVectorIndex
{
    private readonly FeatureVectorIndex<CNFClause> innerIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class.
    /// </summary>
    /// <param name="featureVectorSelector">Delegate to use to retrieve the feature vector for any given clause.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector)
    {
        innerIndex = new(featureVectorSelector);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with a specified root node.
    /// </summary>
    /// <param name="featureVectorSelector">Delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the index.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<CNFClause> root)
    {
        innerIndex = new(featureVectorSelector, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with some initial content.
    /// </summary>
    /// <param name="featureVectorSelector">Delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="content">The initial content to be added to the index.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, content.Select(c => KeyValuePair.Create(c, c)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with a specified root node and some initial content.
    /// </summary>
    /// <param name="featureVectorSelector">Delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the index.</param>
    /// <param name="content">The initial content to be added to the index.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<CNFClause> root,
        IEnumerable<CNFClause> content)
    {
        innerIndex = new(featureVectorSelector, root, content.Select(c => KeyValuePair.Create(c, c)));
    }

    /// <summary>
    /// Adds a clause to the index.
    /// </summary>
    /// <param name="clause">The clause to be added.</param>
    public void Add(CNFClause clause) => innerIndex.Add(clause, clause);

    /// <summary>
    /// Gets all of the stored clauses that subsume a given clause. 
    /// </summary>
    /// <param name="clause">The query clause.</param>
    /// <returns>An enumerable of all of the stored clauses that subsume the query clause.</returns>
    public IEnumerable<CNFClause> GetSubsuming(CNFClause clause) => innerIndex.GetSubsuming(clause);

    /// <summary>
    /// Gets all of the stored clauses that are subsumed by a given clause. 
    /// </summary>
    /// <param name="clause">The query clause.</param>
    /// <returns>An enumerable of all of the stored clauses that are subsumed by the query clause.</returns>
    public IEnumerable<CNFClause> GetSubsumed(CNFClause clause) => innerIndex.GetSubsumed(clause);
}
