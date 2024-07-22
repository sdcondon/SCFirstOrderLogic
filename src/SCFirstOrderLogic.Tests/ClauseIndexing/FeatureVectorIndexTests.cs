using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private record GetTestCase(IEnumerable<CNFClause> Content, CNFClause Query, IEnumerable<CNFClause> Expected);

    public static Test GetSubsumedClausesBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
        ])
        .When(tc =>
        {
            return Enumerable.Empty<CNFClause>();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

    public static Test GetSubsumingClausesBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
        ])
        .When(tc =>
        {
            return Enumerable.Empty<CNFClause>();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));
}
