using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using System.Collections.Generic;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.BackwardChaining;

public static class BackwardChainingKB_FromAIaMATests
{
    public static Test PositiveScenarios => TestThat
        .GivenTestContext()
        .AndEachOf(() => new BackwardChainingKB_FromAIaMA.Query[]
        {
            // trivial
            MakeQuery(
                query: IsKing(John),
                kb: new Sentence[]
                {
                    IsKing(John)
                }),

            // single conjunct, single step
            MakeQuery(
                query: IsEvil(John),
                kb: new Sentence[]
                {
                    IsGreedy(John),
                    AllGreedyAreEvil
                }),

            // two conjuncts, single step
            MakeQuery(
                query: IsEvil(John),
                kb: new Sentence[]
                {
                    IsGreedy(John),
                    IsKing(John),
                    AllGreedyKingsAreEvil
                }),

            // two conjuncts, single step, with a red herring
            MakeQuery(
                query: IsEvil(X),
                kb: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(Mary),
                    IsQueen(Mary),
                    AllGreedyKingsAreEvil,
                    AllGreedyQueensAreEvil,
                }),

            // Simple multiple proofs
            MakeQuery(
                query: IsKing(X),
                kb: new Sentence[]
                {
                    IsKing(John),
                    IsKing(Richard),
                }),

            // Uses same var twice in same proof
            MakeQuery(
                query: Knows(John, Mary),
                kb: new Sentence[]
                {
                    AllGreedyAreEvil,
                    AllEvilKnowEachOther,
                    IsGreedy(John),
                    IsGreedy(Mary),
                }),

            // More complex - Crime example domain
            MakeQuery(
                query: CrimeDomain.IsCriminal(ColonelWest),
                kb: CrimeDomain.Axioms),
        })
        .When((cxt, query) => query.Execute())
        .ThenReturns()
        .And((_, _, rv) => rv.Should().BeTrue())
        .And((_, query, _) => query.Result.Should().BeTrue())
        .And((cxt, query, _) => cxt.WriteOutputLine(query.ResultExplanation)); // Going to replace with full proof trees, so no point asserting on subs for now.

    public static Test NegativeScenarios => TestThat
        .GivenEachOf(() => new BackwardChainingKB_FromAIaMA.Query[]
        {
            // no matching clause
            MakeQuery(
                query: IsEvil(X),
                kb: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(John),
                }),

            // clause with not all conjuncts satisfied
            MakeQuery(
                query: IsEvil(X),
                kb: new Sentence[]
                {
                    IsKing(John),
                    AllGreedyKingsAreEvil,
                }),

            // no unifier will work - x is either John or Richard - it can't be both:
            MakeQuery(
                query: IsEvil(X),
                kb: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(Richard),
                    AllGreedyKingsAreEvil,
                }),
        })
        .When(query => query.Execute())
        .ThenReturns()
        .And((_, rv) => rv.Should().BeFalse())
        .And((query, _) => query.Result.Should().BeFalse());

    private static BackwardChainingKB_FromAIaMA.Query MakeQuery(Sentence query, IEnumerable<Sentence> kb)
    {
        var knowledgeBase = new BackwardChainingKB_FromAIaMA();
        knowledgeBase.Tell(kb);
        return knowledgeBase.CreateQueryAsync(query).GetAwaiter().GetResult();
    }
}
