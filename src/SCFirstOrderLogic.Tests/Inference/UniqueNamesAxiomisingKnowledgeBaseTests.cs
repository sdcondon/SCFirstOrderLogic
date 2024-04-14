using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.TestUtilities;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceFactory.KinshipDomain;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference;

public static class UniqueNamesAxiomisingKnowledgeBaseTests
{
    public static Test Smoke => TestThat
        .Given(() => new MockKnowledgeBase())
        .WhenAsync(async kb =>
        {
            var sut = new UniqueNamesAxiomisingKnowledgeBase(kb);
            await sut.TellAsync(IsMale(new Constant("Bob")));
            await sut.TellAsync(IsMale(new Constant("Larry")));
            await sut.TellAsync(Not(IsMale(new Constant("Alex"))));
        })
        .ThenReturns()
        .And(kb =>
        {
            kb.Sentences.Should().BeEquivalentTo(
                expectation: new Sentence[]
                {
                    IsMale(new Constant("Bob")), // Sentence that we told it
                    IsMale(new Constant("Larry")), // Sentence that we told it
                    Not(IsMale(new Constant("Alex"))), // Sentence that we told it
                    Not(AreEqual(new Constant("Larry"), new Constant("Bob"))),
                    Not(AreEqual(new Constant("Alex"), new Constant("Bob"))),
                    Not(AreEqual(new Constant("Alex"), new Constant("Larry"))),
                },
                config: EquivalencyOptions.UsingOnlyConsistencyForVariables);
        });

    private class MockKnowledgeBase : IKnowledgeBase
    {
        public Collection<Sentence> Sentences { get; } = new Collection<Sentence>();

        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            Sentences.Add(sentence);
            return Task.CompletedTask;
        }

        public Task<IQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
