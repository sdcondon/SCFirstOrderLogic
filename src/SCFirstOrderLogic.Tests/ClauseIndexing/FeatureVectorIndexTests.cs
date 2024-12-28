using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ClauseIndexing.Features;
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using SCFirstOrderLogic.TestData;
using SCFirstOrderLogic.TestUtilities;
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
                SubsumptionFacts.AllNonEmptyClauses);

            return SubsumptionFacts.AllNonEmptyClauses.Select(c => new GetTestCase(
                index,
                c,
                SubsumptionFacts.GetSubsumedClauses(c),
                SubsumptionFacts.GetNonSubsumedClauses(c)));
        })
        .When(tc => tc.Index.GetSubsumed(tc.Query))
        .ThenReturns()
        .And((tc, rv) => tc.ExpectedPresent.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.ExpectedAbsent));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf(() =>
        {
            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                SubsumptionFacts.AllNonEmptyClauses);

            return SubsumptionFacts.AllNonEmptyClauses.Select(c => new GetTestCase(
                Index: index,
                Query: c,
                ExpectedPresent: SubsumptionFacts.GetSubsumingClauses(c),
                ExpectedAbsent: SubsumptionFacts.GetNonSubsumingClauses(c)));
        })
        .When(tc => tc.Index.GetSubsuming(tc.Query))
        .ThenReturns()
        .And((tc, rv) => tc.ExpectedPresent.Should().BeSubsetOf(rv))
        .And((tc, rv) => rv.Should().NotIntersectWith(tc.ExpectedAbsent));

    public static Test GetSubsumingBehaviourFuzz => TestThat
        .GivenEachOf(() =>
        {
            var content = Enumerable.Range(0, 100)
                .Select(i => CNFClauseHelper.MakeRandomClause())
                .Distinct(new VariableIdIgnorantEqualityComparer())
                .ToArray();

            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                content);

            return content.Select(c => new GetTestCaseExact(
                Index: index,
                Query: c,
                Expected: content.Where(o => o.Subsumes(c))));
        })
        .When(tc => tc.Index.GetSubsuming(tc.Query))
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

    public static Test GetSubsumedBehaviourFuzz => TestThat
        .GivenEachOf(() =>
        {
            var content = Enumerable.Range(0, 100)
                .Select(i => CNFClauseHelper.MakeRandomClause())
                .Distinct(new VariableIdIgnorantEqualityComparer())
                .ToArray();

            var index = new FeatureVectorIndex<OccurenceCountFeature>(
                OccurenceCountFeature.MakeFeatureVector,
                new FeatureVectorIndexListNode<OccurenceCountFeature, CNFClause>(OccurenceCountFeature.MakeFeatureComparer(Comparer<object>.Default)),
                content);

            return content.Select(c => new GetTestCaseExact(
                Index: index,
                Query: c,
                Expected: content.Where(o => o.IsSubsumedBy(c))));
        })
        .When(tc => tc.Index.GetSubsumed(tc.Query))
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

    private record AddTestCase(
        CNFClause[] PriorContent,
        CNFClause Add);

    private record TryReplaceSubsumedTestCase(
        CNFClause[] PriorContent,
        CNFClause Add,
        bool ExpectedReturnValue,
        CNFClause[] ExpectedContent);

    private record GetTestCase(
        FeatureVectorIndex<OccurenceCountFeature> Index,
        CNFClause Query,
        IEnumerable<CNFClause> ExpectedPresent,
        IEnumerable<CNFClause> ExpectedAbsent);

    private record GetTestCaseExact(
        FeatureVectorIndex<OccurenceCountFeature> Index,
        CNFClause Query,
        IEnumerable<CNFClause> Expected);
}
