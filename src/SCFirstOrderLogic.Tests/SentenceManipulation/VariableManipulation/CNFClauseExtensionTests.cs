using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using SCFirstOrderLogic.TestData;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation
{
    public static class CNFClauseExtensionTests
    {
        public static Test SubsumesBehaviour => TestThat
            .GivenEachOf(() => SubsumptionFacts.All)
            .When(tc => (tc.X.Subsumes(tc.Y), tc.Y.Subsumes(tc.X)))
            .ThenReturns()
            .And((tc, rv) => rv.Should().Be((tc.IsYSubsumedByX, tc.IsXSubsumedByY)));

        public static Test SubsumedByBehaviour => TestThat
            .GivenEachOf(() => SubsumptionFacts.All)
            .When(tc => (tc.X.IsSubsumedBy(tc.Y), tc.Y.IsSubsumedBy(tc.X)))
            .ThenReturns()
            .And((tc, rv) => rv.Should().Be((tc.IsXSubsumedByY, tc.IsYSubsumedByX)));

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

        private static OperablePredicate P(params OperableTerm[] arguments) => new(nameof(P), arguments);
        private static OperablePredicate Q(params OperableTerm[] arguments) => new(nameof(Q), arguments);

        private record UnifiesWithAnyOfTestCase(Sentence Clause, IEnumerable<Sentence> Clauses, bool ExpectedResult);
    }
}
