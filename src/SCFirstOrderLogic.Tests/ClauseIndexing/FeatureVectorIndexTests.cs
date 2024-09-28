using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ClauseIndexing.Features;
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using SCFirstOrderLogic.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    public static Test NegativeAddBehaviour => TestThat
        .GivenEachOf<AddTestCase>(() =>
        [
            new([], CNFClause.Empty),
            new([new(P(U))], new(P(V)))
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default),
                tc.PriorContent);
            index.Add(tc.Add);
        })
        .ThenThrows((_, ex) => ex.Should().BeOfType<ArgumentException>());

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf(() =>
        {
            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => new GetTestCase(c, GetSubsumedClausesFromSubsumptionFacts(c), GetNonSubsumedClausesFromSubsumptionFacts(c)));
        })
        .When(tc =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default),
                AllNonEmptyClausesFromSubsumptionFacts);
            return index.GetSubsumed(tc.Query);
        })
        .ThenReturns()
        .And((tc, rv) => tc.Expected.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.NotExpected));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf(() =>
        {
            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => new GetTestCase(c, GetSubsumingClausesFromSubsumptionFacts(c), GetNonSubsumingClausesFromSubsumptionFacts(c)));
        })
        .When(tc =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default),
                AllNonEmptyClausesFromSubsumptionFacts);
            return index.GetSubsuming(tc.Query);
        })
        .ThenReturns()
        .And((tc, rv) => tc.Expected.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.NotExpected));

    private static IEnumerable<CNFClause> AllNonEmptyClausesFromSubsumptionFacts => SubsumptionFacts
        .All
        .SelectMany(f => new[] { f.X, f.Y })
        .Except([CNFClause.Empty])
        .Distinct(new VariableIdIgnorantEqualityComparer());

    private static CNFClause[] GetSubsumedClausesFromSubsumptionFacts(CNFClause subsumingClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumingClause) && f.IsYSubsumedByX)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumingClause) && f.IsXSubsumedByY)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    private static CNFClause[] GetNonSubsumedClausesFromSubsumptionFacts(CNFClause subsumingClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumingClause) && !f.IsYSubsumedByX)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumingClause) && !f.IsXSubsumedByY)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    private static CNFClause[] GetSubsumingClausesFromSubsumptionFacts(CNFClause subsumedClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumedClause) && f.IsXSubsumedByY)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumedClause) && f.IsYSubsumedByX)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    private static CNFClause[] GetNonSubsumingClausesFromSubsumptionFacts(CNFClause subsumedClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumedClause) && !f.IsXSubsumedByY)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumedClause) && !f.IsYSubsumedByX)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    private record GetTestCase(CNFClause Query, CNFClause[] Expected, CNFClause[] NotExpected);

    private record AddTestCase(CNFClause[] PriorContent, CNFClause Add);
}
