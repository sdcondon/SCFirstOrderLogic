using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.TestUtilities;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceFactory.KinshipDomain;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference;

public static class EqualityAxiomisingKnowledgeBaseTests
{
    public static Test Smoke => TestThat
        .GivenEachOf(() => new TestCase[]
        {
            new( // Unary predicate and function
                Sentence: ForAll(X, IsMale(Father(X))),
                ExpectedKnowledge: new Sentence[]
                {
                    ForAll(X, AreEqual(X, X)), // Equality reflexivity
                    ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X))), // Equality commutativity
                    ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z))), // Equality transitivity

                    ForAll(X, IsMale(Father(X))), // Sentence that we told it

                    ForAll(X, Y, If(AreEqual(X, Y), Iff(IsMale(X), IsMale(Y)))), // Equality and IsMale predicate
                    ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Father(X), Father(Y)))), // Equality and Father function
                }),

            new( // Ground predicate
                Sentence: new Predicate("MyGroundPredicate"),
                ExpectedKnowledge: new Sentence[]
                {
                    ForAll(X, AreEqual(X, X)), // Equality reflexivity
                    ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X))), // Equality commutativity
                    ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z))), // Equality transitivity

                    new Predicate("MyGroundPredicate"), // Sentence that we told it
                }),

            new( // (Unary predicate and) Ground function
                Sentence: IsMale(new Function("MyGroundFunction")),
                ExpectedKnowledge: new Sentence[]
                {
                    ForAll(X, AreEqual(X, X)), // Equality reflexivity
                    ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X))), // Equality commutativity
                    ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z))), // Equality transitivity

                    IsMale(new Function("MyGroundFunction")), // Sentence that we told it

                    ForAll(X, Y, If(AreEqual(X, Y), Iff(IsMale(X), IsMale(Y)))), // Equality and IsMale predicate
                }),
        })
        .WhenAsync(async tc => await tc.KB.TellAsync(tc.Sentence))
        .ThenReturns()
        .And(tc =>
        {
            tc.InnerKB.Sentences.Should().BeEquivalentTo(
                expectation: tc.ExpectedKnowledge,
                config: EquivalencyOptions.UsingOnlyConsistencyForVariables);
        });

    private record TestCase(Sentence Sentence, Sentence[] ExpectedKnowledge)
    {
        private EqualityAxiomisingKnowledgeBase? kb;

        public MockKnowledgeBase InnerKB { get; } = new MockKnowledgeBase();

        public EqualityAxiomisingKnowledgeBase KB => kb ??= EqualityAxiomisingKnowledgeBase.CreateAsync(InnerKB).GetAwaiter().GetResult();
    }

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
