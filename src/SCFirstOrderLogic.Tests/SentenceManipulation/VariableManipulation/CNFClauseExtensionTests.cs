using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

////namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation
////{
////    public static class CNFClauseExtensionTests
////    {
////        public static Test SubsumesBehaviour => TestThat
////            .GivenEachOf<SubsumptionTestCase>(() =>
////            [
////                new (X: P(),         Y: P(),         Expected: true),
////                new (X: P(X),        Y: P(C),        Expected: true),
////                new (X: P(X),        Y: P(C) | P(D), Expected: true),
////                new (X: P(X),        Y: P(C) | Q(X), Expected: true),
////                new (X: P(C),        Y: P(C),        Expected: true),
////                new (X: P(C),        Y: P(C) | P(D), Expected: true),
////                new (X: P(X) | Q(Y), Y: P(C) | Q(D), Expected: true),
////                new (X: P(X) | Q(Y), Y: P(C) | Q(Z), Expected: true),
////                new (X: P(X) | Q(X), Y: P(C) | Q(C), Expected: true),
////                new (X: P(X) | Q(X), Y: P(C) | Q(D), Expected: false),
////                new (X: P(X),        Y: Q(C),        Expected: false),
////                new (X: P(X) | Q(Y), Y: P(C),        Expected: false),
////                new (X: P(C),        Y: P(X),        Expected: false),
////            ])
////            .When(tc => tc.X.ToCNF().Clauses.Single().Subsumes(tc.Y.ToCNF().Clauses.Single()))
////            .ThenReturns()
////            .And((tc, rv) => rv.Should().Be(tc.Expected));

////        public static Test SubsumedByBehaviourBehaviour => TestThat
////            .GivenEachOf<SubsumptionTestCase>(() =>
////            [
////                new (X: P(),         Y: P(),         Expected: true),
////                new (X: P(C),        Y: P(X),        Expected: true),
////                new (X: P(C) | P(D), Y: P(X),        Expected: true),
////                new (X: P(C) | Q(X), Y: P(X),        Expected: true),
////                new (X: P(C),        Y: P(C),        Expected: true),
////                new (X: P(C) | P(D), Y: P(C),        Expected: true),
////                new (X: P(C) | Q(D), Y: P(X) | Q(Y), Expected: true),
////                new (X: P(C) | Q(Z), Y: P(X) | Q(Y), Expected: true),
////                new (X: P(C) | Q(C), Y: P(X) | Q(X), Expected: true),
////                new (X: P(C) | Q(D), Y: P(X) | Q(X), Expected: false),
////                new (X: Q(C),        Y: P(X),        Expected: false),
////                new (X: P(C),        Y: P(X) | Q(Y), Expected: false),
////                new (X: P(X),        Y: P(C),        Expected: false),
////            ])
////            .When(tc => tc.X.ToCNF().Clauses.Single().IsSubsumedBy(tc.Y.ToCNF().Clauses.Single()))
////            .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

////        private static Function C => new(nameof(C));
////        private static Function D => new(nameof(D));
////        private static OperablePredicate F(params OperableTerm[] arguments) => new(nameof(F), arguments);
////        private static OperablePredicate G(params OperableTerm[] arguments) => new(nameof(G), arguments);
////        private static OperablePredicate P(params OperableTerm[] arguments) => new(nameof(P), arguments);
////        private static OperablePredicate Q(params OperableTerm[] arguments) => new(nameof(Q), arguments);

////        private record SubsumptionTestCase(Sentence X, Sentence Y, bool Expected);
////    }
////}

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    public static class CNFClauseExtensionTests
    {
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
