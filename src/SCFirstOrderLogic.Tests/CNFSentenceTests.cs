using FluentAssertions;
using FlUnit;
using System;

namespace SCFirstOrderLogic;

public static class CNFSentenceTests
{
    private static Sentence P => new Predicate("P");
    private static Sentence Q => new Predicate("Q");
    private static Sentence R => new Predicate("R");

    private record EqualityTestCase(CNFSentence X, CNFSentence Y, bool ExpectedEquality);

    public static Test EqualityBehaviour => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: new(Array.Empty<CNFClause>()),
                Y: new(Array.Empty<CNFClause>()),
                ExpectedEquality: true),

            new(
                X: new(new[] { CNFClause.Empty }),
                Y: new(new[] { CNFClause.Empty }),
                ExpectedEquality: true),

            new(
                X: new(new CNFClause[] { new(P), new(Q) }),
                Y: new(new CNFClause[] { new(Q), new(P) }),
                ExpectedEquality: true),

            new(
                X: new(new CNFClause[] { new(P), new(Q) }),
                Y: new(new CNFClause[] { new(P), new(Q), new(R) }),
                ExpectedEquality: false),

            new(
                X: new(new CNFClause[] { new(P), new(Q), new(R) }),
                Y: new(new CNFClause[] { new(P), new(Q) }),
                ExpectedEquality: false),
        ])
        .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
        .ThenReturns()
        .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
        .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality)); // <- yeah yeah, strictly speaking not the right thing to be asserting, but..
}
