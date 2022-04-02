using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.KinshipDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class SimpleResolutionKnowledgeBaseTests
    {
        public static Test CrimeExample => TestThat
            .When(() =>
            {
                var kb = new SimpleResolutionKnowledgeBase(ClausePairFilters.None, ClausePairPriorityComparers.UnitPreference);
                kb.Tell(CrimeDomain.Axioms);
                return kb.Ask(IsCriminal(West));
            })
            .ThenReturns()
            .And(retVal => retVal.Should().Be(true));

        public static Test CuriousityAndTheCatExample => TestThat
            .When(() =>
            {
                var kb = new SimpleResolutionKnowledgeBase(ClausePairFilters.None, ClausePairPriorityComparers.UnitPreference);
                kb.Tell(CuriousityAndTheCatDomain.Axioms);
                var query = kb.CreateQuery(Kills(Curiousity, Tuna));
                query.Complete();
                return query;
            })
            .ThenReturns()
            .And(retVal => retVal.Result.Should().Be(true))
            .And(retVal => retVal.Explain().Should().BeNull());

        public static Test KinshipExample => TestThat
            .When(() =>
            {
                var kb = new SimpleResolutionKnowledgeBase(ClausePairFilters.None, ClausePairPriorityComparers.UnitPreference);
                kb.Tell(KinshipDomain.Axioms);
                var query = kb.CreateQuery(ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X))));
                query.Complete();
                return query;
            })
            .ThenReturns()
            .And(retVal => retVal.Result.Should().Be(false));
            //.And(retVal => retVal.Explain().Should().BeNull());
    }
}
