// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing.Features;

/// <summary>
/// Identifying record for a clause indexing feature that is the occurence count of a particular identifier within positive or negative literals.
/// </summary>
/// <param name="Identifier">The identifier to which this feature relates, or null if it is for literal counts.</param>
/// <param name="IsInPositiveLiteral">A value indicating whether this feature relates to occurence counts in (or of) positive literals, or negative.</param>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public record OccurenceCountFeature(object? Identifier, bool IsInPositiveLiteral)
{
    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of: 
    /// positive literal count, negative literal count, occurence count of each occuring
    /// identifier among positive literals, and occurence count of each occuring identifier
    /// among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<OccurenceCountFeature, int>> MakeFeatureVector(CNFClause clause)
    {
        Dictionary<OccurenceCountFeature, int> featureVector = new();

        foreach (var literal in clause.Literals)
        {
            var literalCountFeature = new OccurenceCountFeature(null, literal.IsPositive);
            featureVector.TryGetValue(literalCountFeature, out var value);
            featureVector[literalCountFeature] = value + 1;

            literal.Predicate.Accept(new CreationVisitor(featureVector, literal.IsPositive));
        }

        return featureVector;
    }

    /// <summary>
    /// Makes a comparer that can be used (to determine the ordering of nodes in the index) with the features included
    /// in feature vectors created by <see cref="MakeFeatureVector(CNFClause)"/>.
    /// </summary>
    /// <param name="identifierComparer">The comparer to use to compare identifiers.</param>
    /// <returns>A new <see cref="IComparer{T}"/>.</returns>
    public static IComparer<OccurenceCountFeature> MakeFeatureComparer(IComparer identifierComparer)
    {
        return Comparer<OccurenceCountFeature>.Create((x, y) =>
        {
            if (x.Identifier != null && y.Identifier != null)
            {
                var identifierComparison = identifierComparer.Compare(x.Identifier, y.Identifier);
                if (identifierComparison != 0)
                {
                    return identifierComparison;
                }
                else
                {
                    // NB: this an arbitrary decision. We just need a consistent comparison - there's no
                    // reason to think that positive literals are more or less informative than negative ones.
                    return x.IsInPositiveLiteral.CompareTo(y.IsInPositiveLiteral);
                }
            }
            else
            {
                // literal counts are of low informativeness, so we score them lower than identifier occurence counts:
                return (x.Identifier != null, y.Identifier != null) switch
                {
                    (false, false) => x.IsInPositiveLiteral.CompareTo(y.IsInPositiveLiteral),
                    (false, true) => -1,
                    (true, false) => 1,
                    (true, true) => x.IsInPositiveLiteral.CompareTo(y.IsInPositiveLiteral) // todo: will never happen. refactor me. 
                };
            }
        });
    }

    private class CreationVisitor : RecursiveSentenceVisitor
    {
        private readonly IDictionary<OccurenceCountFeature, int> featureVector;
        private readonly bool forPositiveLiterals;

        public CreationVisitor(IDictionary<OccurenceCountFeature, int> featureVector, bool forPositiveLiterals)
        {
            this.featureVector = featureVector;
            this.forPositiveLiterals = forPositiveLiterals;
        }

        public override void Visit(Predicate predicate)
        {
            AddOccurence(predicate.Identifier);
            base.Visit(predicate);
        }

        public override void Visit(Function function)
        {
            AddOccurence(function.Identifier);
            base.Visit(function);
        }

        private void AddOccurence(object identifier)
        {
            var feature = new OccurenceCountFeature(identifier, forPositiveLiterals);
            featureVector.TryGetValue(feature, out var value);
            featureVector[feature] = value + 1;
        }
    }
}
