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
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                tc.PriorContent);
            index.Add(tc.Add);
        })
        .ThenThrows((_, ex) => ex.Should().BeOfType<ArgumentException>());

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf(() =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                AllNonEmptyClausesFromSubsumptionFacts);

            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => new GetTestCase(index, c, GetSubsumedClausesFromSubsumptionFacts(c), GetNonSubsumedClausesFromSubsumptionFacts(c)));
        })
        .When(tc => tc.Index.GetSubsumed(tc.Query))
        .ThenReturns()
        .And((tc, rv) => tc.Expected.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.NotExpected));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf(() =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                AllNonEmptyClausesFromSubsumptionFacts);

            return AllNonEmptyClausesFromSubsumptionFacts
                .Select(c => new GetTestCase(index, c, GetSubsumingClausesFromSubsumptionFacts(c), GetNonSubsumingClausesFromSubsumptionFacts(c)));
        })
        .When(tc => tc.Index.GetSubsuming(tc.Query))
        .ThenReturns()
        .And((tc, rv) => tc.Expected.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.NotExpected));

    public static Test GetSubsumingBehaviourFuzz => TestThat
        .GivenEachOf(() =>
        {
            var content = Enumerable.Range(0, 100)
                .Select(i => MakeRandomClause())
                .Distinct(new VariableIdIgnorantEqualityComparer())
                .ToArray();

            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                content);

            return content
                .Select(c => new GetTestCaseExact(index, c, content.Where(o => o.Subsumes(c))));
        })
        .When(tc => tc.Index.GetSubsuming(tc.Query))
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

    public static Test GetSubsumedBehaviourFuzz => TestThat
        .GivenEachOf(() =>
        {
            var content = Enumerable.Range(0, 100)
                .Select(i => MakeRandomClause())
                .Distinct(new VariableIdIgnorantEqualityComparer())
                .ToArray();

            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                content);

            return content
                .Select(c => new GetTestCaseExact(index, c, content.Where(o => o.IsSubsumedBy(c))));
        })
        .When(tc => tc.Index.GetSubsumed(tc.Query))
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

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

    private static CNFClause MakeRandomClause()
    {
        return new CNFClause(Enumerable
            .Range(0, Random.Shared.Next(1, 2))
            .Select(i => new Literal(MakeRandomLiteral())));

        Sentence MakeRandomLiteral()
        {
            return Random.Shared.Next(1, 12) switch
            {
                1 => P(),
                2 => !P(),
                3 => Q(),
                4 => !Q(),
                5 => P(MakeRandomTerm()),
                6 => !P(MakeRandomTerm()),
                7 => P(MakeRandomTerm(), MakeRandomTerm()),
                8 => !P(MakeRandomTerm(), MakeRandomTerm()),
                9 => Q(MakeRandomTerm()),
                10 => !Q(MakeRandomTerm()),
                11 => Q(MakeRandomTerm(), MakeRandomTerm()),
                12 => !Q(MakeRandomTerm(), MakeRandomTerm()),
                _ => throw new Exception()
            };
        }

        Term MakeRandomTerm()
        {
            return Random.Shared.Next(1, 14) switch
            {
                1 => C,
                2 => D,
                3 => F(),
                4 => G(),
                5 => U,
                6 => V,
                7 => W,
                8 => X,
                9 => Y,
                10 => Z,
                11 => F(MakeRandomTerm()),
                12 => F(MakeRandomTerm(), MakeRandomTerm()),
                13 => G(MakeRandomTerm()),
                14 => H(MakeRandomTerm(), MakeRandomTerm()),
                _ => throw new Exception()
            };
        }
    }

    private record GetTestCase(
        FeatureVectorIndex<OccurenceCountFeature> Index,
        CNFClause Query,
        IEnumerable<CNFClause> Expected,
        IEnumerable<CNFClause> NotExpected);

    private record GetTestCaseExact(
        FeatureVectorIndex<OccurenceCountFeature> Index,
        CNFClause Query,
        IEnumerable<CNFClause> Expected);

    private record AddTestCase(
        CNFClause[] PriorContent,
        CNFClause Add);
}
