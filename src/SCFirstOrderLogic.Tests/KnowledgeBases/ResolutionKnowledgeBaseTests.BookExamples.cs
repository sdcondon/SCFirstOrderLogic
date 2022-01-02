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
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(CrimeDomain.Axioms);
                return kb.Ask(IsCriminal(West));
            })
            .ThenReturns()
            .And(retVal => retVal.Should().Be(true));

        public static Test BookExample2 => TestThat
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(CuriousityAndTheCatDomain.Axioms);
                return kb.Ask(Kills(Curiousity, Tuna));
            })
            .ThenReturns()
            .And(retVal => retVal.Should().Be(true));
    }
}
