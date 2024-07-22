using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// 
/// </summary>
public class FeatureVectorIndex
{
    private readonly IFeatureVectorIndexNode root;
    private readonly Func<CNFClause, object> featureSelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex"/> class.
    /// </summary>
    /// <param name="featureSelector"></param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(Func<CNFClause, object> featureSelector, IFeatureVectorIndexNode root)
    {
        this.root = root ?? throw new ArgumentNullException(nameof(root));
        this.featureSelector = featureSelector ?? throw new ArgumentNullException(nameof(featureSelector));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Add(CNFClause clause)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<CNFClause> GetSubsumingClauses(CNFClause clause)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clause"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<CNFClause> GetSubsumedClauses(CNFClause clause)
    {
        throw new NotImplementedException();
    }
}
