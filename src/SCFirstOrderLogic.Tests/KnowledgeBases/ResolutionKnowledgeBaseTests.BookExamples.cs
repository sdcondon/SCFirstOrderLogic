using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;

namespace SCFirstOrderLogic.KnowledgeBases
{
    public static partial class ResolutionKnowledgeBaseTests
    {
        public static Test BookExample1 => TestThat
            .Given(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(CrimeDomain.Axioms);
                return kb;
            })
            .When(kb => kb.Ask(IsCriminal(West)))
            .ThenReturns()
            .And((_, retVal) => retVal.Should().Be(true));

        public static Test BookExample2 => TestThat
            .Given(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(CuriousityAndTheCatDomain.Axioms);
                return kb;
            })
            .When(kb => kb.Ask(Kills(Curiousity, Tuna)))
            .ThenReturns()
            .And((_, retVal) => retVal.Should().Be(true));
    }
}
