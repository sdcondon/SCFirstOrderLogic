using FluentAssertions;
using FlUnit;
using System.Linq;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static partial class CNFSentenceTests
    {
        private static Term C => new Constant(nameof(C));
        private static Sentence S(Term t) => new Predicate(nameof(S), new[] { t });
        private static Sentence T(Term t) => new Predicate(nameof(T), new[] { t });
        private static Sentence U(Term t) => new Predicate(nameof(U), new[] { t });

        private record TestCase(Sentence Sentence1, Sentence Sentence2, Sentence? ExpectedResolvent = null);

        public static Test ResolutionOfResolvableClauses => TestThat
            .GivenEachOf(() => new TestCase[]
            {
                // Modus Ponens resolution with a constant
                new(
                    Sentence1: If(S(C), T(C)), // ¬S(C) T(C)
                    Sentence2: S(C),
                    ExpectedResolvent: T(C)),

                // Modus Ponens resolution on a globally quantified variable & a constant
                new(
                    Sentence1: ForAll(X, If(S(X), T(X))),
                    Sentence2: S(C),
                    ExpectedResolvent: T(C)),

                // Modus Ponens resolution on a constant & a globally quantified variable
                new(
                    Sentence1: If(S(C), T(C)),
                    Sentence2: ForAll(X, S(X)),
                    ExpectedResolvent: T(C)),

                // Modus Ponens resolution on a globally quantified variable
                new(
                    Sentence1: ForAll(X, If(S(X), T(X))),
                    Sentence2: ForAll(X, S(X)),
                    ExpectedResolvent: ForAll(X, T(X))),

                // More complicated - with a constant
                new(
                    Sentence1: If(S(C), T(C)),
                    Sentence2: Or(S(C), U(C)),
                    ExpectedResolvent: Or(T(C), U(C))),
            })
            .When(g =>
            {
                return CNFClause.Resolve(
                    new CNFSentence(g.Sentence1).Clauses.Single(),
                    new CNFSentence(g.Sentence2).Clauses.Single());
            })
            .ThenReturns((g, r) => r.Single().Should().Be(new CNFSentence(g.ExpectedResolvent!).Clauses.Single()));

        public static Test ResolutionOfUnresolvableClauses => TestThat
            .GivenEachOf(() => new TestCase[]
            {
                new(
                    Sentence1: S(C),
                    Sentence2: T(C)),
            })
            .When(g =>
            {
                return CNFClause.Resolve(
                    new CNFSentence(g.Sentence1).Clauses.Single(),
                    new CNFSentence(g.Sentence2).Clauses.Single());
            })
            .ThenReturns((g, r) => r.Should().BeEmpty());

        public static Test ResolutionOfComplementaryUnitClauses => TestThat
            .GivenEachOf(() => new TestCase[]
            {
                new(
                    Sentence1: S(C),
                    Sentence2: Not(S(C))),
            })
            .When(g =>
            {
                return CNFClause.Resolve(
                    new CNFSentence(g.Sentence1).Clauses.Single(),
                    new CNFSentence(g.Sentence2).Clauses.Single());
            })
            .ThenReturns((g, r) => r.Single().Should().Be(CNFClause.Empty));

        public static Test ResolutionOfMultiplyResolvableClauses => TestThat
            .Given(() =>
            {
                return new TestCase(
                    Sentence1: Or(S(C), Not(T(C))),
                    Sentence2: Or(Not(S(C)), T(C)));
            })
            .When(g =>
            {
                return CNFClause.Resolve(
                    new CNFSentence(g.Sentence1).Clauses.Single(),
                    new CNFSentence(g.Sentence2).Clauses.Single());
            })
            .ThenReturns((g, r) => r.Should().BeEquivalentTo(new[]
            {
                // Both of these resolvents are trivially true - so largely useless - should the method return no resolvents in this case?
                // Are all cases where more than one resolvent would be returned useless? Should the method return a (potentially null) clause instead of a enumerable?
                new CNFSentence(Or(S(C), Not(S(C)))).Clauses.Single(),
                new CNFSentence(Or(T(C), Not(T(C)))).Clauses.Single()
            }));
    }
}
