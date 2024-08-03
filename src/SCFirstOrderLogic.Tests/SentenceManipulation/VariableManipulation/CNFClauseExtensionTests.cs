using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation
{
    public static class CNFClauseExtensionTests
    {
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

        public static Test SubsumedByBehaviourBehaviour => TestThat
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

        private static Function C => new(nameof(C));
        private static Function D => new(nameof(D));
        private static OperablePredicate F(params OperableTerm[] arguments) => new(nameof(F), arguments);
        private static OperablePredicate G(params OperableTerm[] arguments) => new(nameof(G), arguments);
        private static OperablePredicate P(params OperableTerm[] arguments) => new(nameof(P), arguments);
        private static OperablePredicate Q(params OperableTerm[] arguments) => new(nameof(Q), arguments);

        private record SubsumptionTestCase(CNFClause X, CNFClause Y, bool Expected)
        {
            public SubsumptionTestCase(Sentence X, Sentence Y, bool Expected)
                : this(X.ToCNF().Clauses.Single(), Y.ToCNF().Clauses.Single(), Expected) { }

            public SubsumptionTestCase(CNFClause X, Sentence Y, bool Expected)
                : this(X, Y.ToCNF().Clauses.Single(), Expected) { }

            public SubsumptionTestCase(Sentence X, CNFClause Y, bool Expected)
                : this(X.ToCNF().Clauses.Single(), Y, Expected) { }
        }

        private record UnifiesWithAnyOfTestCase(Sentence Clause, IEnumerable<Sentence> Clauses, bool ExpectedResult);
    }
}
