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
    public static class EqualityAxiominsingKnowledgeBaseTests
    {
        public static Test Smoke => TestThat
            .Given(() => new MockKnowledgeBase())
            .When(kb => EqualityAxiomisingKnowledgeBase.CreateAsync(kb).GetAwaiter().GetResult().TellAsync(ForAll(X, IsMale(Father(X)))).Wait())
            .ThenReturns()
            .And(kb =>
            {
                kb.Sentences.Should().BeEquivalentTo(
                    expectation: new Sentence[]
                    {
                        ForAll(X, AreEqual(X, X)), // Equality reflexivity
                        ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X))), // Equality commutativity
                        ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z))), // Equality transitivity
                        ForAll(X, IsMale(Father(X))), // Sentence that we told it
                        ForAll(X, Y, If(AreEqual(X, Y), Iff(IsMale(X), IsMale(Y)))), // Equality and IsMale predicate
                        ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Father(X), Father(Y)))), // Equality and Father function
                    },
                    config: EquivalencyOptions.UsingOnlyConsistencyForVariables);
            });

        public static Test GroundPredicate => TestThat
            .Given(() => new MockKnowledgeBase())
            .When(kb => EqualityAxiomisingKnowledgeBase.CreateAsync(kb).GetAwaiter().GetResult().TellAsync(new Predicate("GroundPredicate")).Wait())
            .ThenReturns()
            .And(kb =>
            {
                kb.Sentences.Should().BeEquivalentTo(
                    expectation: new Sentence[]
                    {
                        ForAll(X, AreEqual(X, X)), // Equality reflexivity
                        ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X))), // Equality commutativity
                        ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z))), // Equality transitivity
                        new Predicate("GroundPredicate"), // Sentence that we told it
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
