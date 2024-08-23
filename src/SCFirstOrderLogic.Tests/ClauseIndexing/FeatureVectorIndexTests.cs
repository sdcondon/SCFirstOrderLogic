using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ClauseIndexing.Features;
using SCFirstOrderLogic.TestData;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private record GetTestCase(Sentence Query, IEnumerable<Sentence> Expected);

    // TODO: shouldn't be allowed to add empty clause

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf(() =>
        {
            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => (clause: c, expectedSubsumedClauses: GetSubsumedClausesFromSubsumptionFacts(c)));
        })
        .When(tc =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default),
                AllNonEmptyClausesFromSubsumptionFacts);
            return index.GetSubsumed(tc.clause);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.expectedSubsumedClauses));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf(() =>
        {
            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => (clause: c, expectedSubsumingClauses: GetSubsumingClausesFromSubsumptionFacts(c)));
        })
        .When(tc =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default),
                AllNonEmptyClausesFromSubsumptionFacts);
            return index.GetSubsuming(tc.clause);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.expectedSubsumingClauses));

    private static IEnumerable<CNFClause> AllNonEmptyClausesFromSubsumptionFacts => SubsumptionFacts
        .All
        .SelectMany(f => new[] { f.X, f.Y })
        .Except([CNFClause.Empty])
        .Distinct();

    private static IEnumerable<CNFClause> GetSubsumedClausesFromSubsumptionFacts(CNFClause subsumingClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumingClause) && f.IsYSubsumedByX)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumingClause) && f.IsXSubsumedByY)
            .Select(f => f.X);

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct();
    }

    private static IEnumerable<CNFClause> GetSubsumingClausesFromSubsumptionFacts(CNFClause subsumedClause)
    {
        var xClauses = SubsumptionFacts
            .All
            .Where(f => f.X.Equals(subsumedClause) && f.IsXSubsumedByY)
            .Select(f => f.Y);

        var yClauses = SubsumptionFacts
            .All
            .Where(f => f.Y.Equals(subsumedClause) && f.IsYSubsumedByX)
            .Select(f => f.X);

        return xClauses.Concat(yClauses).Except([CNFClause.Empty]).Distinct();
    }
}
