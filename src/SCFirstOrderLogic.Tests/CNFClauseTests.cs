using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.TestUtilities;
using System;
using static SCFirstOrderLogic.FormulaCreation.OperableFormulaFactory;

namespace SCFirstOrderLogic;

public static class CNFClauseTests
{
    private static OperablePredicate P => new("P");
    private static OperablePredicate Q => new("Q");
    private static OperablePredicate R => new("R");

    private static OperablePredicate HashCodeCollisionX => new(new HashCodeCollision("X"));
    private static OperablePredicate HashCodeCollisionY => new(new HashCodeCollision("Y"));

    public static Test ToFormulaBehaviour_Positive => TestThat
        .GivenEachOf<ToFormulaTestCase>(() =>
        [
            new(
                CNFClause: new(P),
                ExpectedFormula: P),

            new(
                CNFClause: new(P | Q),
                ExpectedFormula: new Disjunction(P, Q)),
        ])
        .When(tc => tc.CNFClause.ToFormula())
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedFormula));

    public static Test ToFormulaBehaviour_EmptyClause => TestThat
        .When(() => CNFClause.Empty.ToFormula())
        .ThenThrows()
        .And(e => e.Should().BeOfType<InvalidOperationException>());

    public static Test EqualityBehaviour => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: new(Array.Empty<Literal>()),
                Y: new(Array.Empty<Literal>()),
                ExpectedEquality: true),

            new(
                X: new(P),
                Y: new(P),
                ExpectedEquality: true),

            new(
                X: new(P | Q),
                Y: new(Q | P),
                ExpectedEquality: true),

            new(
                X: new(HashCodeCollisionX | HashCodeCollisionY),
                Y: new(HashCodeCollisionY | HashCodeCollisionX),
                ExpectedEquality: true),

            new(
                X: new(P | Q),
                Y: new(P | Q | R),
                ExpectedEquality: false),

            new(
                X: new(P | Q | R),
                Y: new(P | Q),
                ExpectedEquality: false),
        ])
        .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
        .ThenReturns()
        .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
        .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality)); // <- yeah yeah, strictly speaking not the right thing to be asserting, but..

    private record ToFormulaTestCase(CNFClause CNFClause, Formula ExpectedFormula);

    private record EqualityTestCase(CNFClause X, CNFClause Y, bool ExpectedEquality);

}
