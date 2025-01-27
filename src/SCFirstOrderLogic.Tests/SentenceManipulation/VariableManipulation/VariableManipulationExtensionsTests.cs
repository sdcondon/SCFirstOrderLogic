using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.Specialised.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public static class VariableManipulationExtensionsTests
{
    public static Test OrdinaliseBehaviourTests => TestThat
        .GivenEachOf<OrdinaliseTestCase>(() =>
        [
            new(Input: F(), Expected: F()),
            new(Input: F(X), Expected: F(Var(0))),
            new(Input: F(G(X, Y), G(X, Z)), Expected: F(G(Var(0), Var(1)), G(Var(0), Var(2)))),
        ])
        .When(tc => tc.Input.Ordinalise())
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test IsInstanceOfBehaviourTests => TestThat
        .GivenEachOf<BinaryTestCase>(() =>
        [
            new(X: F(), Y: F(), Expected: true),
            new(X: F(C), Y: F(X), Expected: true),
            new(X: F(G(Y, C)), Y: F(X), Expected: true),
            new(X: F(X), Y: F(C), Expected: false),
            new(X: F(X, C), Y: F(C, X), Expected: false),
        ])
        .When(tc => tc.X.IsInstanceOf(tc.Y))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test IsGeneralisationOfBehaviourTests => TestThat
        .GivenEachOf<BinaryTestCase>(() =>
        [
            new(X: F(), Y: F(), Expected: true),
            new(X: F(X), Y: F(C), Expected: true),
            new(X: F(X), Y: F(G(Y, C)), Expected: true),
            new(X: F(C), Y: F(X), Expected: false),
            new(X: F(X, C), Y: F(C, X), Expected: false),
        ])
        .When(tc => tc.X.IsGeneralisationOf(tc.Y))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test SubsumesBehaviour => TestThat
        .GivenEachOf<SubsumptionTestCase>(() =>
        [
            new (X: P(),             Y: P(),         Expected: true),
            new (X: P(X),            Y: P(C),        Expected: true),
            new (X: P(X),            Y: P(C) | P(D), Expected: true),
            new (X: P(X),            Y: P(C) | Q(X), Expected: true),
            new (X: P(C),            Y: P(C),        Expected: true),
            new (X: P(C),            Y: P(C) | P(D), Expected: true),
            new (X: P(X) | Q(Y),     Y: P(C) | Q(D), Expected: true),
            new (X: P(X) | Q(Y),     Y: P(C) | Q(Z), Expected: true),
            new (X: P(X) | Q(X),     Y: P(C) | Q(C), Expected: true),
            new (X: P(X) | Q(X),     Y: P(C) | Q(D), Expected: false),
            new (X: P(X),            Y: Q(C),        Expected: false),
            new (X: P(X) | Q(Y),     Y: P(C),        Expected: false),
            new (X: P(C),            Y: P(X),        Expected: false),
            new (X: CNFClause.Empty, Y: P(),         Expected: false),
        ])
        .When(tc => tc.X.Subsumes(tc.Y))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test SubsumedByBehaviour => TestThat
        .GivenEachOf<SubsumptionTestCase>(() =>
        [
            new (X: P(),         Y: P(),             Expected: true),
            new (X: P(C),        Y: P(X),            Expected: true),
            new (X: P(C) | P(D), Y: P(X),            Expected: true),
            new (X: P(C) | Q(X), Y: P(X),            Expected: true),
            new (X: P(C),        Y: P(C),            Expected: true),
            new (X: P(C) | P(D), Y: P(C),            Expected: true),
            new (X: P(C) | Q(D), Y: P(X) | Q(Y),     Expected: true),
            new (X: P(C) | Q(Z), Y: P(X) | Q(Y),     Expected: true),
            new (X: P(C) | Q(C), Y: P(X) | Q(X),     Expected: true),
            new (X: P(C) | Q(D), Y: P(X) | Q(X),     Expected: false),
            new (X: Q(C),        Y: P(X),            Expected: false),
            new (X: P(C),        Y: P(X) | Q(Y),     Expected: false),
            new (X: P(X),        Y: P(C),            Expected: false),
            new (X: P(),         Y: CNFClause.Empty, Expected: false),
        ])
        .When(tc => tc.X.IsSubsumedBy(tc.Y))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test UnifiesWithAnyOfBehaviour => TestThat
        .GivenEachOf<UnifiesWithAnyOfTestCase>(() =>
        [
            new (
                Clause: P(X, Y),
                Clauses: [],
                ExpectedResult: false),

            new (
                Clause: P(X, Y),
                Clauses: [P(A, B)],
                ExpectedResult: true),

            new (
                Clause: P(X, Y) | Q(X, Y),
                Clauses: [P(A, B) | Q(A, B)],
                ExpectedResult: true),
        ])
        .When(tc => tc.Clause.ToCNF().Clauses.Single().UnifiesWithAnyOf(tc.Clauses.Select(s => s.ToCNF().Clauses.Single())))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.ExpectedResult));

    private record UnifiesWithAnyOfTestCase(Sentence Clause, IEnumerable<Sentence> Clauses, bool ExpectedResult);

    private record OrdinaliseTestCase(Term Input, Term Expected);

    private record BinaryTestCase(Term X, Term Y, bool Expected);

    private record SubsumptionTestCase(CNFClause X, CNFClause Y, bool Expected)
    {
        public SubsumptionTestCase(Sentence X, Sentence Y, bool Expected)
            : this(new CNFClause(X), new CNFClause(Y), Expected) { }

        public SubsumptionTestCase(CNFClause X, Sentence Y, bool Expected)
            : this(X, new CNFClause(Y), Expected) { }

        public SubsumptionTestCase(Sentence X, CNFClause Y, bool Expected)
            : this(new CNFClause(X), Y, Expected) { }
    }
}
