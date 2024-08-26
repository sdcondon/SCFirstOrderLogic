using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ClauseIndexing.Features;
using SCFirstOrderLogic.TestData;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private record GetTestCase(CNFClause Query, CNFClause[] Expected, CNFClause[] NotExpected);

    // TODO: shouldn't be allowed to add empty clause

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
        .Distinct();

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

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct().ToArray();
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

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct().ToArray();
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

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct().ToArray();
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

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct().ToArray();
    }
}
