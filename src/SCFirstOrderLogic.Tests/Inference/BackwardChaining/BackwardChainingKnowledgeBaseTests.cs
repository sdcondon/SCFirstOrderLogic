using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.BackwardChaining;

public static class BackwardChainingKnowledgeBaseTests
{
    public static Test PositiveScenarios => TestThat
        .GivenTestContext()
        .AndEachOf(() => new TestCase[]
        {
            new(
                Label: "Trivial",
                Query: IsKing(John),
                Knowledge: new Sentence[]
                {
                    IsKing(John)
                }),

            new(
                Label: "single conjunct, single step",
                Query: IsEvil(John),
                Knowledge: new Sentence[]
                {
                    IsGreedy(John),
                    AllGreedyAreEvil
                }),

            new(
                Label: "Two conjuncts, single step",
                Query: IsEvil(John),
                Knowledge: new Sentence[]
                {
                    IsGreedy(John),
                    IsKing(John),
                    AllGreedyKingsAreEvil
                }),

            new(
                Label: "Two applicable rules, each with two conjuncts, single step",
                Query: IsEvil(X),
                Knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(Mary),
                    IsQueen(Mary),
                    AllGreedyKingsAreEvil,
                    AllGreedyQueensAreEvil,
                }),

            new(
                Label: "Simple multiple possible substitutions",
                Query: IsKing(X),
                Knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsKing(Richard),
                }),

            new(
                Label: "Uses same var twice in same proof",
                Query: Knows(John, Mary),
                Knowledge: new Sentence[]
                {
                    AllGreedyAreEvil,
                    AllEvilKnowEachOther,
                    IsGreedy(John),
                    IsGreedy(Mary),
                }),

            new(
                Label: "Crime example domain",
                Query: IsCriminal(ColonelWest),
                Knowledge: CrimeDomain.Axioms),
        })
        .When((_, tc) =>
        {
            var knowledgeBase = new BackwardChainingKnowledgeBase(new DictionaryClauseStore());
            knowledgeBase.Tell(tc.Knowledge);

            var query = knowledgeBase.CreateQuery(tc.Query);
            query.Execute();

            return query;
        })
        .ThenReturns()
        .And((_, _, query) => query.Result.Should().BeTrue())
        .And((cxt, _, query) => cxt.WriteOutputLine(query.ResultExplanation));

    public static Test NegativeScenarios => TestThat
        .GivenEachOf(() => new TestCase[]
        {
            new(
                Label: "No matching clause",
                Query: IsEvil(X),
                Knowledge:
                [
                    IsKing(John),
                    IsGreedy(John),
                ]),

            new(
                Label: "clause with not all conjuncts satisfied",
                Query: IsEvil(X),
                Knowledge:
                [
                    IsKing(John),
                    AllGreedyKingsAreEvil,
                ]),

            new(
                Label: "No unifier will work - x is either John or Richard - it can't be both",
                Query: IsEvil(X),
                Knowledge:
                [
                    IsKing(John),
                    IsGreedy(Richard),
                    AllGreedyKingsAreEvil,
                ]),
        })
        .When(tc =>
        {
            var knowledgeBase = new BackwardChainingKnowledgeBase(new DictionaryClauseStore(tc.Knowledge));
            var query = knowledgeBase.CreateQuery(tc.Query);
            query.Execute();

            return query;
        })
        .ThenReturns()
        .And((_, query) => query.Result.Should().BeFalse());

    public static Test RepeatedQueryExecution => TestThat
        .Given(() =>
        {
            var knowledgeBase = new BackwardChainingKnowledgeBase(new DictionaryClauseStore());
            return knowledgeBase.CreateQuery(IsGreedy(John));
        })
        .WhenAsync(async q =>
        {
            var task1 = q.ExecuteAsync();
            var task2 = q.ExecuteAsync();

            try
            {
                await Task.WhenAll(task1, task2);
            }
            catch (InvalidOperationException) { }

            return (task1, task2);
        })
        .ThenReturns((q, rv) =>
        {
            (rv.task1.IsFaulted ^ rv.task2.IsFaulted).Should().BeTrue();
        });

    private record TestCase(string Label, Sentence Query, IEnumerable<Sentence> Knowledge)
    {
        public override string ToString() => Label;
    }
}
