using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingOperableSentenceFactory;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingOperableSentenceFactory.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory.CuriousityAndTheCatDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.Resolution;

public static class ResolutionKnowledgeBaseTests
{
    public static Test PositiveScenarios => TestThat
        .GivenTestContext()
        .AndEachOf(() => new StrategyFactory[]
        {
            new(NewDelegateResolutionStrategy),
            //new(NewLinearResolutionStrategy),
        })
        .AndEachOf(() => new TestCase[]
        {
            new( // trivial
                query: IsKing(John),
                knowledge: new Sentence[]
                {
                    IsKing(John)
                }),

            new( // single conjunct, single step
                query: IsEvil(John),
                knowledge: new Sentence[]
                {
                    IsGreedy(John),
                    AllGreedyAreEvil
                }),

            new( // Two conjuncts, single step
                query: IsEvil(John),
                knowledge: new Sentence[]
                {
                    IsGreedy(John),
                    IsKing(John),
                    AllGreedyKingsAreEvil
                }),

            new( // Two applicable rules, each with two conjuncts, single step
                query: ThereExists(X, IsEvil(X)),
                knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(Mary),
                    IsQueen(Mary),
                    AllGreedyKingsAreEvil,
                    AllGreedyQueensAreEvil,
                }),

            new( // Multiple possible substitutions
                query: ThereExists(X, IsKing(X)),
                knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsKing(Richard),
                }),

            new( // Uses same var twice in same proof
                query: Knows(John, Mary),
                knowledge: new Sentence[]
                {
                    AllGreedyAreEvil,
                    AllEvilKnowEachOther,
                    IsGreedy(John),
                    IsGreedy(Mary),
                }),

            new( // More complex - Crime example domain
                query: IsCriminal(ColonelWest),
                knowledge: CrimeDomain.Axioms),

            new( // More complex with some non-definite clauses - curiousity and the cat example domain
                query: Kills(Curiousity, Tuna),
                knowledge: CuriousityAndTheCatDomain.Axioms),
        })
        .When((_, sf, tc) =>
        {
            var knowledgeBase = new ResolutionKnowledgeBase(sf.makeStrategy());
            knowledgeBase.Tell(tc.knowledge);
            var query = knowledgeBase.CreateQuery(tc.query);
            query.Execute();
            return query;
        })
        .ThenReturns()
        .And((_, _, _, q) => q.Result.Should().BeTrue())
        .And((cxt, _, _, q) => cxt.WriteOutput(q.ResultExplanation));

    public static Test NegativeScenarios => TestThat
        .GivenEachOf(() => new StrategyFactory[]
        {
            new(NewDelegateResolutionStrategy),
            //new(NewLinearResolutionStrategy),
        })
        .AndEachOf(() => new TestCase[]
        {
            new( // no matching clause
                query: IsEvil(John),
                knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(John),
                }),

            new( // clause with not all conjuncts satisfied
                query: IsEvil(John),
                knowledge: new Sentence[]
                {
                    IsKing(John),
                    AllGreedyKingsAreEvil,
                }),

            new( // no unifier will work - x is either John or Richard - it can't be both:
                query: ThereExists(X, IsEvil(X)),
                knowledge: new Sentence[]
                {
                    IsKing(John),
                    IsGreedy(Richard),
                    AllGreedyKingsAreEvil,
                }),
        })
        .When((sf, tc) =>
        {
            var knowledgeBase = new ResolutionKnowledgeBase(sf.makeStrategy());
            knowledgeBase.Tell(tc.knowledge);
            var query = knowledgeBase.CreateQuery(tc.query);
            query.Execute();
            return query;
        })
        .ThenReturns()
        .And((_, _, q) => q.Result.Should().BeFalse());

    public static Test RepeatedQueryExecution => TestThat
        .Given(() =>
        {
            var knowledgeBase = new ResolutionKnowledgeBase(new DelegateResolutionStrategy(
                new HashSetClauseStore(),
                DelegateResolutionStrategy.Filters.None,
                DelegateResolutionStrategy.PriorityComparisons.UnitPreference));

            return knowledgeBase.CreateQuery(IsGreedy(John));
        })
        .When(q =>
        {
            var task1 = q.ExecuteAsync();
            var task2 = q.ExecuteAsync();

            try
            {
                Task.WhenAll(task1, task2).GetAwaiter().GetResult();
            }
            catch (InvalidOperationException) { }

            return (task1, task2);
        })
        .ThenReturns((q, rv) =>
        {
            (rv.task1.IsFaulted ^ rv.task2.IsFaulted).Should().BeTrue();
        });

    // This is a difficult query. Would need more complex algo to deal with it
    // in a timely fashion. Better way of handling equality, better prioritisation, etc.
    ////public static Test KinshipExample => TestThat
    ////    .GivenTestContext()
    ////    .When(_ =>
    ////    {
    ////        var resolutionStrategy = new LinearResolutionStrategy(new HashSetClauseStore());
    ////        var innerKb = new ResolutionKnowledgeBase(resolutionStrategy);
    ////        var kb = EqualityAxiomisingKnowledgeBase.CreateAsync(innerKb).GetAwaiter().GetResult();
    ////        kb.TellAsync(KinshipDomain.Axioms).Wait();
    ////        var query = kb.CreateQueryAsync(ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X)))).GetAwaiter().GetResult();
    ////        query.ExecuteAsync().Wait();
    ////        return query;
    ////    })
    ////    .ThenReturns()
    ////    .And((_, retVal) => retVal.Result.Should().Be(true))
    ////    .And((ctx, retVal) => ctx.WriteOutputLine(((ResolutionQuery)retVal).ResultExplanation));

    private static IResolutionStrategy NewDelegateResolutionStrategy() => new DelegateResolutionStrategy(
        new HashSetClauseStore(),
        DelegateResolutionStrategy.Filters.None,
        DelegateResolutionStrategy.PriorityComparisons.UnitPreference);

    //// private static IResolutionStrategy NewLinearResolutionStrategy() => new LinearResolutionStrategy(new HashSetClauseStore());

    private record StrategyFactory(
        Func<IResolutionStrategy> makeStrategy,
        [CallerArgumentExpression("makeStrategy")] string? makeStrategyExpression = null)
    {
        public override string ToString() => makeStrategyExpression!;
    }

    private record TestCase(
        Sentence query,
        IEnumerable<Sentence> knowledge,
        [CallerArgumentExpression("knowledge")] string? knowledgeExpression = null)
    {
        public override string ToString() => $"{query}; given knowledge: {knowledgeExpression}";
    }
}
