// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing.Features;

/// <summary>
/// Identifying record for a clause indexing feature that is the (1-based) max depth of a particular identifier within positive or negative literals.
/// </summary>
/// <param name="Identifier">The identifier to which this feature relates.</param>
/// <param name="IsInPositiveLiteral">A value indicating whether this feature relates to max depths in positive literals, or negative.</param>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public record MaxDepthFeature(object Identifier, bool IsInPositiveLiteral)
{
    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of the max depth
    /// of each occuring identifier among positive literals, and the max depth of each occuring 
    /// identifier among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<MaxDepthFeature, int>> MakeFeatureVector(CNFClause clause)
    {
        Dictionary<MaxDepthFeature, int> featureVector = new();

        foreach (var literal in clause.Literals)
        {
            literal.Predicate.Accept(new CreationVisitor(featureVector, literal.IsPositive), 1);
        }

        return featureVector;
    }

    /// <summary>
    /// Makes a comparer that can be used (to determine the ordering of nodes in the index) with the features included
    /// in feature vectors created by <see cref="MakeFeatureVector(CNFClause)"/>.
    /// </summary>
    /// <param name="identifierComparer">The comparer to use to compare identifiers.</param>
    /// <returns>A new <see cref="IComparer{T}"/>.</returns>
    public static IComparer<MaxDepthFeature> MakeFeatureComparer(IComparer identifierComparer)
    {
        return Comparer<MaxDepthFeature>.Create((x, y) =>
        {
            var identifierComparison = identifierComparer.Compare(x.Identifier, y.Identifier);
            if (identifierComparison != 0)
            {
                return identifierComparison;
            }
            else
            {
                // NB: this an arbitrary decision. We just need a consistent comparison - there's no
                // reason to think that positive literals are more or less informative tha negative ones.
                return x.IsInPositiveLiteral.CompareTo(y.IsInPositiveLiteral);
            }
        });
    }

    private class CreationVisitor : RecursiveSentenceVisitor<int>
    {
        private readonly IDictionary<MaxDepthFeature, int> featureVector;
        private readonly bool forPositiveLiterals;

        public CreationVisitor(IDictionary<MaxDepthFeature, int> featureVector, bool forPositiveLiterals)
        {
            this.featureVector = featureVector;
            this.forPositiveLiterals = forPositiveLiterals;
        }

        public override void Visit(Predicate predicate, int depth)
        {
            UpdateMaxDepth(predicate.Identifier, depth);
            base.Visit(predicate, depth + 1);
        }

        public override void Visit(Function function, int depth)
        {
            UpdateMaxDepth(function.Identifier, depth);
            base.Visit(function, depth + 1);
        }

        private void UpdateMaxDepth(object identifier, int depth)
        {
            var feature = new MaxDepthFeature(identifier, forPositiveLiterals);
            featureVector.TryGetValue(feature, out var maxDepth);

            if (depth > maxDepth)
            {
                featureVector[feature] = depth;
            }
        }
    }
}
