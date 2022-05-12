using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference.Unification;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class SimpleResolutionKnowledgeBaseTests
    {
        public static Test CrimeExample => TestThat
            .GivenTestContext()
            .When(_ =>
            {
                var kb = new SimpleResolutionKnowledgeBase(new ListClauseStore(), ClausePairFilters.None, ClausePairPriorityComparisons.UnitPreference);
                kb.TellAsync(CrimeDomain.Axioms).Wait();
                var query = kb.CreateQueryAsync(IsCriminal(West)).Result;
                query.CompleteAsync().Wait();
                return query;
            })
            .ThenReturns()
            .And((_, retVal) => retVal.Result.Should().Be(true))
            .And((ctx, retVal) => ctx.WriteOutputLine(retVal.Explain()));

        public static Test CuriousityAndTheCatExample => TestThat
            .GivenTestContext()
            .When(_ =>
            {
                var kb = new SimpleResolutionKnowledgeBase(new ListClauseStore(), ClausePairFilters.None, ClausePairPriorityComparisons.UnitPreference);
                kb.TellAsync(CuriousityAndTheCatDomain.Axioms).Wait();
                var query = kb.CreateQueryAsync(Kills(Curiousity, Tuna)).Result;
                query.CompleteAsync().Wait();
                return query;
            })
            .ThenReturns()
            .And((_, retVal) => retVal.Result.Should().Be(true))
            .And((ctx, retVal) => ctx.WriteOutputLine(retVal.Explain()));

        // TODO-BUG: Some* of the explanations this comes up with are just plain wrong.
        // Is this type of query not something resolution can cope with? Or just a mistake?
        // Perhaps build up explanation logic? Maybe try to improve (create a legend of?) Skolem function labelling. 
        // *(NB can vary across executions because unit preference not stable across executions because of hash code reliance)
        public static Test KinshipExample => TestThat
            .GivenTestContext()
            .When(_ =>
            {
                var kb = new SimpleResolutionKnowledgeBase(new ListClauseStore(), ClausePairFilters.None, ClausePairPriorityComparisons.UnitPreference);
                kb.TellAsync(KinshipDomain.Axioms).Wait();
                var query = kb.CreateQueryAsync(ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X)))).Result;
                query.CompleteAsync().Wait();
                return query;
            })
            .ThenReturns()
            .And((_, retVal) => retVal.Result.Should().Be(true))
            .And((ctx, retVal) => ctx.WriteOutputLine(retVal.Explain()));
    }
}
