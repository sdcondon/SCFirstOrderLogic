using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference.Chaining;
using System;
using System.Collections.Generic;
using System.Threading;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
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

        private static SimpleResolutionQuery MakeQuery(Sentence query, IEnumerable<Sentence> knowledge)
        {
            var knowledgeBase = new SimpleResolutionKnowledgeBase(new SimpleClauseStore(), SimpleResolutionKnowledgeBase.Filters.None, SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
            knowledgeBase.Tell(knowledge);
            return knowledgeBase.CreateQuery(query);
        }

        // This one needs equality (so not really a test of this KB alone), and in practice doesn't terminate in any reasonable time frame.
        // Would need smarter clause prioritisation, and possibly smarter treatment of equality (e.g. demodulation)
        // NB can vary across executions because priority comparison used not stable across executions because of hash code reliance
        ////public static Test KinshipExample => TestThat
        ////    .GivenTestContext()
        ////    .When(_ =>
        ////    {
        ////        var kb = new EqualityAxiomisingKnowledgeBase(new SimpleResolutionKnowledgeBase(new ListClauseStore(), ClausePairFilters.None, ClausePairPriorityComparisons.TotalLiteralCountMinimisation));
        ////        kb.TellAsync(KinshipDomain.Axioms).Wait();
        ////        var query = kb.CreateQueryAsync(ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X)))).Result;
        ////        query.CompleteAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token).Wait();
        ////        return query;
        ////    })
        ////    .ThenReturns()
        ////    .And((_, retVal) => retVal.Result.Should().Be(true))
        ////    .And((ctx, retVal) => ctx.WriteOutputLine(((SimpleResolutionQuery)retVal).Explain()));
    }
}
