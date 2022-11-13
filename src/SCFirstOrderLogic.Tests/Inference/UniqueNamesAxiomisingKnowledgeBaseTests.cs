using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.TestUtilities;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceFactory.KinshipDomain;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    public static class UniqueNamesAxiominsingKnowledgeBaseTests
    {
        public static Test Smoke => TestThat
            .Given(() => new MockKnowledgeBase())
            .When(kb =>
            {
                var sut = new UniqueNamesAxiomisingKnowledgeBase(kb);
                sut.TellAsync(IsMale(new Constant("Bob"))).Wait();
                sut.TellAsync(IsMale(new Constant("Larry"))).Wait();
                sut.TellAsync(Not(IsMale(new Constant("Alex")))).Wait();
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
}
