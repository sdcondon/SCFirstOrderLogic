using FluentAssertions;
using FlUnit;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.KinshipDomain;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class EqualityAxiominsingKnowledgeBaseTests
    {
        public static Test Smoke => TestThat
            .Given(() => new MockKnowledgeBase())
            .When(kb => new EqualityAxiomisingKnowledgeBase(kb).TellAsync(ForAll(X, IsMale(Father(X)))).Wait())
            .ThenReturns()
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, IsMale(Father(X)))))
            // will fail if it used different variable names - but meh, will do for now
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, AreEqual(X, X))))
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X)))))
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z)))))
            // again, variable choice dependence. Various things we can do to sort this out.
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, If(AreEqual(X, Y), Iff(IsMale(X), IsMale(Y))))))
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Father(X), Father(Y))))));

        public static Test GroundPredicate => TestThat
            .Given(() => new MockKnowledgeBase())
            .When(kb => new EqualityAxiomisingKnowledgeBase(kb).TellAsync(new Predicate("GroundPredicate")).Wait())
            .ThenReturns()
            .And(kb => kb.Sentences.Should().Contain(new Predicate("GroundPredicate")))
            // will fail if it used different variable names - but meh, will do for now
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, AreEqual(X, X))))
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X)))))
            .And(kb => kb.Sentences.Should().Contain(ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z)))));

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
