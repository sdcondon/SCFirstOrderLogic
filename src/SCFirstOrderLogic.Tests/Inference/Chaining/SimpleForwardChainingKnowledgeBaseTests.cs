using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class SimpleForwardChainingKnowledgeBaseTests
    {
        public static Test CrimeDomainExample => TestThat
            .Given(() =>
            {
                var kb = new SimpleForwardChainingKnowledgeBase();
                kb.Tell(CrimeDomain.Axioms);

                return kb.CreateQueryAsync(IsCriminal(West)).Result;
            })
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeTrue())
            .And((query, _) => query.Result.Should().BeTrue());
    }
}
