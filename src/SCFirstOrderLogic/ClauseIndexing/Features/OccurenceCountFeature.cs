// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing.Features;

/// <summary>
/// A clause indexing feature that is the occurence count of a particular identifier within positive or negative literals.
/// </summary>
/// <param name="Identifier">The identifier to which this feature relates, or null if it is for literal counts.</param>
/// <param name="IsInPositiveLiteral">A value indicating whether this feature relates to occurence counts in (or of) positive literals, or negative.</param>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public record OccurenceCountFeature(object? Identifier, bool IsInPositiveLiteral)
{
    private static readonly Func<object, bool> DefaultIdentifierFilter = i => i is not SkolemFunctionIdentifier && i is not EqualityIdentifier;
    private static readonly Func<CNFClause, IEnumerable<FeatureVectorComponent<OccurenceCountFeature>>> DefaultFeatureVectorSelector = MakeFeatureVectorSelector(DefaultIdentifierFilter);

    /// <summary>
    /// <para>
    /// Creates a feature vector consisting of: positive literal count, negative literal count, 
    /// occurence count of each occuring identifier among positive literals, and occurence count 
    /// of each occuring identifier among negative literals.
    /// </para>
    /// <para>
    /// Skolem function identifiers and the identifier for the equality predicate are not included in the returned vectors.
    /// </para>
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<FeatureVectorComponent<OccurenceCountFeature>> MakeFeatureVector(CNFClause clause)
    {
        return DefaultFeatureVectorSelector(clause);
    }

    /// <summary>
    /// Creates a feature vector selector delegate that gives vectors consisting of the max depth of each occuring identifier
    /// among positive literals, and the max depth of each occuring identifier among negative literals.
    /// </summary>
    /// <param name="identifierFilter">The filter to use to determine whether identifiers should be included in a vector.</param>
    /// <returns>A delegate for creating feature vectors.</returns>
    public static Func<CNFClause, IEnumerable<FeatureVectorComponent<OccurenceCountFeature>>> MakeFeatureVectorSelector(Func<object, bool> identifierFilter)
    {
        return clause =>
        {
            Dictionary<OccurenceCountFeature, int> featureVector = new();

            foreach (var literal in clause.Literals)
            {
                var literalCountFeature = new OccurenceCountFeature(null, literal.IsPositive);
                featureVector.TryGetValue(literalCountFeature, out var value);
                featureVector[literalCountFeature] = value + 1;

                literal.Predicate.Accept(new CreationVisitor(featureVector, literal.IsPositive, identifierFilter));
            }

            return featureVector.Select(kvp => new FeatureVectorComponent<OccurenceCountFeature>(kvp.Key, kvp.Value));
        };
    }

    /// <summary>
    /// <para>
    /// Makes a comparer of <see cref="OccurenceCountFeature"/>s that can be used to determine the ordering of nodes in a feature vector index.
    /// </para>
    /// <para>
    /// This overload creates a comparer that uses <see cref="Comparer.Default"/> to compare identifiers as part of doing its comparison.
    /// Note that <see cref="Comparer.Default"/> will throw if it encounters any object of a type that does not implement <see cref="IComparable"/>,
    /// and many types that *do* implement <see cref="IComparable"/> (including <see cref="string"/>) will throw when attempting to compare to objects
    /// of another type. This overload should only be used if all identifiers that it will encounter implement <see cref="IComparable"/> in such a 
    /// way that it will never throw when comparing to any other enountered identifier. For example, its safe enough to use if all of your identiers are
    /// <see cref="string"/>s, but not if some are <see cref="string"/>s and others are <see cref="int"/>s, or if any are of a type that does not implement
    /// <see cref="IComparable"/>.
    /// </para>
    /// </summary>
    /// <returns>A new <see cref="IComparer{T}"/>.</returns>
    public static IComparer<OccurenceCountFeature> MakeFeatureComparer()
    {
        return MakeFeatureComparer(Comparer.Default);
    }

    /// <summary>
    /// Makes a comparer of <see cref="MaxDepthFeature"/>s that can be used to determine the ordering of nodes in a feature vector index.
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
        private readonly Func<object, bool> identifierFilter;

        public CreationVisitor(IDictionary<OccurenceCountFeature, int> featureVector, bool forPositiveLiterals, Func<object, bool> identifierFilter)
        {
            this.featureVector = featureVector;
            this.forPositiveLiterals = forPositiveLiterals;
            this.identifierFilter = identifierFilter;
        }

        public override void Visit(Predicate predicate)
        {
            if (identifierFilter(predicate.Identifier))
            {
                AddOccurence(predicate.Identifier);
            }

            base.Visit(predicate);
        }

        public override void Visit(Function function)
        {
            if (identifierFilter(function.Identifier))
            {
                AddOccurence(function.Identifier);
            }

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
