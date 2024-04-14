using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.BackwardChaining;

public static class BackwardChainingKB_WithoutClauseStoreTests
{
    public static Test PositiveScenarios => TestThat
        .GivenTestContext()
        .AndEachOfAsync<BackwardChainingKB_WithoutClauseStore.Query>(async () =>
        [
            // trivial
            await MakeQueryAsync(
                query: IsKing(John),
                kb:
                [
                    IsKing(John)
                ]),

            // single conjunct, single step
            await MakeQueryAsync(
                query: IsEvil(John),
                kb:
                [
                    IsGreedy(John),
                    AllGreedyAreEvil
                ]),

            // two conjuncts, single step
            await MakeQueryAsync(
                query: IsEvil(John),
                kb:
                [
                    IsGreedy(John),
                    IsKing(John),
                    AllGreedyKingsAreEvil
                ]),

            // two conjuncts, single step, with a red herring
            await MakeQueryAsync(
                query: IsEvil(X),
                kb:
                [
                    IsKing(John),
                    IsGreedy(Mary),
                    IsQueen(Mary),
                    AllGreedyKingsAreEvil,
                    AllGreedyQueensAreEvil,
                ]),

            // Simple multiple proofs
            await MakeQueryAsync(
                query: IsKing(X),
                kb:
                [
                    IsKing(John),
                    IsKing(Richard),
                ]),

            // Uses same var twice in same proof
            await MakeQueryAsync(
                query: Knows(John, Mary),
                kb:
                [
                    AllGreedyAreEvil,
                    AllEvilKnowEachOther,
                    IsGreedy(John),
                    IsGreedy(Mary),
                ]),

            // More complex - Crime example domain
            await MakeQueryAsync(
                query: CrimeDomain.IsCriminal(ColonelWest),
                kb: CrimeDomain.Axioms),
        ])
        .When((_, query) => query.Execute())
        .ThenReturns()
        .And((_, _, rv) => rv.Should().BeTrue())
        .And((_, query, _) => query.Result.Should().BeTrue())
        .And((cxt, query, _) => cxt.WriteOutputLine(query.ResultExplanation));

    public static Test NegativeScenarios => TestThat
        .GivenEachOfAsync<BackwardChainingKB_WithoutClauseStore.Query>(async () =>
        [
            // no matching clause
            await MakeQueryAsync(
                query: IsEvil(X),
                kb:
                [
                    IsKing(John),
                    IsGreedy(John),
                ]),

            // clause with not all conjuncts satisfied
            await MakeQueryAsync(
                query: IsEvil(X),
                kb:
                [
                    IsKing(John),
                    AllGreedyKingsAreEvil,
                ]),

            // no unifier will work - x is either John or Richard - it can't be both:
            await MakeQueryAsync(
                query: IsEvil(X),
                kb:
                [
                    IsKing(John),
                    IsGreedy(Richard),
                    AllGreedyKingsAreEvil,
                ]),
        ])
        .When(query => query.Execute())
        .ThenReturns()
        .And((_, rv) => rv.Should().BeFalse())
        .And((query, _) => query.Result.Should().BeFalse());

    private static async Task<BackwardChainingKB_WithoutClauseStore.Query> MakeQueryAsync(Sentence query, IEnumerable<Sentence> kb)
    {
        var knowledgeBase = new BackwardChainingKB_WithoutClauseStore();
        knowledgeBase.Tell(kb);
        return await knowledgeBase.CreateQueryAsync(query);
    }
}
