using FluentAssertions;
using FlUnit;
////using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingOperableSentenceFactory;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
////using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingOperableSentenceFactory.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory.CuriousityAndTheCatDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class SimpleResolutionKnowledgeBaseTests
    {
        public static Test PositiveScenarios => TestThat
            .GivenTestContext()
            .AndEachOf(() => new SimpleResolutionQuery[]
            {
                // trivial
                MakeQuery(
                    query: IsKing(John),
                    knowledge: new Sentence[]
                    {
                        IsKing(John)
                    }),

                // single conjunct, single step
                MakeQuery(
                    query: IsEvil(John),
                    knowledge: new Sentence[]
                    {
                        IsGreedy(John),
                        AllGreedyAreEvil
                    }),

                // two conjuncts, single step
                MakeQuery(
                    query: IsEvil(John),
                    knowledge: new Sentence[]
                    {
                        IsGreedy(John),
                        IsKing(John),
                        AllGreedyKingsAreEvil
                    }),

                // Two applicable rules, each with two conjuncts, single step
                MakeQuery(
                    query: IsEvil(X),
                    knowledge: new Sentence[]
                    {
                        IsKing(John),
                        IsGreedy(Mary),
                        IsQueen(Mary),
                        AllGreedyKingsAreEvil,
                        AllGreedyQueensAreEvil,
                    }),

                // Multiple possible substitutions
                MakeQuery(
                    query: IsKing(X),
                    knowledge: new Sentence[]
                    {
                        IsKing(John),
                        IsKing(Richard),
                    }),

                // Uses same var twice in same proof
                MakeQuery(
                    query: Knows(John, Mary),
                    knowledge: new Sentence[]
                    {
                        AllGreedyAreEvil,
                        AllEvilKnowEachOther,
                        IsGreedy(John),
                        IsGreedy(Mary),
                    }),

                // More complex - Crime example domain
                MakeQuery(
                    query: IsCriminal(West),
                    knowledge: CrimeDomain.Axioms),

                // More complex with some non-definite clauses - curiousity and the cat example domain
                MakeQuery(
                    query: Kills(Curiousity, Tuna),
                    knowledge: CuriousityAndTheCatDomain.Axioms),
            })
            .When((cxt, query) => query.Execute())
            .ThenReturns()
            .And((_, _, rv) => rv.Should().BeTrue())
            .And((_, query, _) => query.Result.Should().BeTrue())
            .And((cxt, query, _) => cxt.WriteOutput(query.ResultExplanation));

        public static Test NegativeScenarios => TestThat
            .GivenEachOf(() => new SimpleResolutionQuery[]
            {
                // no matching clause
                MakeQuery(
                    query: IsEvil(X),
                    knowledge: new Sentence[]
                    {
                        IsKing(John),
                        IsGreedy(John),
                    }),

                // clause with not all conjuncts satisfied
                MakeQuery(
                    query: IsEvil(X),
                    knowledge: new Sentence[]
                    {
                        IsKing(John),
                        AllGreedyKingsAreEvil,
                    }),

                // no unifier will work - x is either John or Richard - it can't be both:
                MakeQuery(
                    query: IsEvil(X),
                    knowledge: new Sentence[]
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

        public static Test RepeatedQueryExecution => TestThat
            .Given(() =>
            {
                var knowledgeBase = new SimpleResolutionKnowledgeBase(
                    new HashSetClauseStore(),
                    SimpleResolutionKnowledgeBase.Filters.None,
                    SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
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

        private static SimpleResolutionQuery MakeQuery(Sentence query, IEnumerable<Sentence> knowledge)
        {
            var knowledgeBase = new SimpleResolutionKnowledgeBase(
                new HashSetClauseStore(knowledge),
                SimpleResolutionKnowledgeBase.Filters.None,
                SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
            return knowledgeBase.CreateQuery(query);
        }

        // This one needs equality (so not really a test of this KB alone), and in practice doesn't terminate in any reasonable time frame.
        // Would need smarter clause prioritisation, and possibly smarter treatment of equality (e.g. demodulation)
        // NB can vary across executions because priority comparison used not stable across executions because of hash code reliance
        ////public static Test KinshipExample => TestThat
        ////    .GivenTestContext()
        ////    .When(_ =>
        ////    {
        ////        var innerKb = new SimpleResolutionKnowledgeBase(
        ////            new SimpleClauseStore(),
        ////            SimpleResolutionKnowledgeBase.Filters.None,
        ////            SimpleResolutionKnowledgeBase.PriorityComparisons.TotalLiteralCountMinimisation);
        ////        var kb = EqualityAxiomisingKnowledgeBase.CreateAsync(innerKb).GetAwaiter().GetResult();
        ////        kb.TellAsync(KinshipDomain.Axioms).Wait();
        ////        var query = kb.CreateQueryAsync(ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X)))).GetAwaiter().GetResult();
        ////        query.ExecuteAsync().Wait();
        ////        return query;
        ////    })
        ////    .ThenReturns()
        ////    .And((_, retVal) => retVal.Result.Should().Be(true))
        ////    .And((ctx, retVal) => ctx.WriteOutputLine(((SimpleResolutionQuery)retVal).ResultExplanation));
    }
}
