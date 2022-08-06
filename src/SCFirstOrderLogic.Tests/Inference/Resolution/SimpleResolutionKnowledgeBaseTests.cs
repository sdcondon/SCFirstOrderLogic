using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference.Unification;
using System;
using System.Threading;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class SimpleResolutionKnowledgeBaseTests
    {
        public static Test CrimeExample => TestThat
            .GivenTestContext()
            .When(_ =>
            {
                var kb = new SimpleResolutionKnowledgeBase(new SimpleClauseStore(), SimpleResolutionKnowledgeBase.Filters.None, SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
                kb.Tell(CrimeDomain.Axioms);
                var query = kb.CreateQuery(IsCriminal(West));
                query.Execute();
                return query;
            })
            .ThenReturns()
            .And((_, retVal) => retVal.Result.Should().Be(true))
            .And((ctx, retVal) => ctx.WriteOutputLine(retVal.Explain()));

        public static Test CuriousityAndTheCatExample => TestThat
            .GivenTestContext()
            .When(_ =>
            {
                var kb = new SimpleResolutionKnowledgeBase(new SimpleClauseStore(), SimpleResolutionKnowledgeBase.Filters.None, SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
                kb.Tell(CuriousityAndTheCatDomain.Axioms);
                var query = kb.CreateQuery(Kills(Curiousity, Tuna));
                query.Execute();
                return query;
            })
            .ThenReturns()
            .And((_, retVal) => retVal.Result.Should().Be(true))
            .And((ctx, retVal) => ctx.WriteOutputLine(retVal.Explain()));

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
