// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// <para>
/// An implementation of a feature vector index for <see cref="CNFClause"/>s.
/// </para>
/// <para>
/// Feature vector indexing is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public class FeatureVectorIndex<TValue>
{
    private readonly IFeatureVectorIndexNode<TValue> root;
    private readonly Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class.
    /// </summary>
    /// <param name="featureVectorSelector"></param>
    public FeatureVectorIndex(Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector)
        : this(featureVectorSelector, new FeatureVectorIndexDictionaryNode<TValue>(), Enumerable.Empty<KeyValuePair<CNFClause, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with a specified root node.
    /// </summary>
    /// <param name="featureSelector"></param>
    /// <param name="root">The root node of the index.</param>
    public FeatureVectorIndex(Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector, IFeatureVectorIndexNode<TValue> root)
        : this(featureVectorSelector, root, Enumerable.Empty<KeyValuePair<CNFClause, TValue>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with some initial content.
    /// </summary>
    /// <param name="featureSelector"></param>
    /// <param name="content">The initial content to be added to the index.</param>
    public FeatureVectorIndex(Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector, IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, new FeatureVectorIndexDictionaryNode<TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class with a specified root node and some initial content.
    /// </summary>
    /// <param name="featureVectorSelector"></param>
    /// <param name="root">The root node of the index.</param>
    /// <param name="content">The initial content to be added to the index.</param>
    public FeatureVectorIndex(Func<CNFClause, IEnumerable<KeyValuePair<object, int>>> featureVectorSelector, IFeatureVectorIndexNode<TValue> root, IEnumerable<KeyValuePair<CNFClause, TValue>> content)
    {
        this.root = root ?? throw new ArgumentNullException(nameof(root));
        this.featureVectorSelector = featureVectorSelector ?? throw new ArgumentNullException(nameof(featureVectorSelector));

        foreach (var (key, value) in content)
        {
            Add(key, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Add(CNFClause clause, TValue value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<TValue> GetSubsuming(CNFClause clause)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<TValue> GetSubsumed(CNFClause clause)
    {
        throw new NotImplementedException();
    }
}
