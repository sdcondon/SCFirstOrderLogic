using FluentAssertions;
using FlUnit;
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic;

public static class CNFFormulaTests
{
    private static Formula P => new Predicate("P");
    private static Formula Q => new Predicate("Q");
    private static Formula R => new Predicate("R");

    public static Test CtorBehavior_Negative => TestThat
        .When(() => new CNFFormula((IEnumerable<CNFClause>)[]))
        .ThenThrows()
        .And(e => e.Should().BeOfType<ArgumentException>());

    public static Test ToFormulaBehaviour => TestThat
        .GivenEachOf<ToFormulaTestCase>(() =>
        [
            new(
                CNFFormula: new([new(P)]),
                ExpectedFormula: P),

            new(
                CNFFormula: new([new([new(P), new(Q)]), new(R)]),
                ExpectedFormula: new Conjunction(new Disjunction(P, Q), R)),
        ])
        .When(tc => tc.CNFFormula.ToFormula())
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedFormula));

    public static Test EqualityBehaviour => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: new([CNFClause.Empty]),
                Y: new([CNFClause.Empty]),
                ExpectedEquality: true),

            new(
                X: new([new(P), new(Q)]),
                Y: new([new(Q), new(P)]),
                ExpectedEquality: true),

            new(
                X: new([new(P), new(Q)]),
                Y: new([new(P), new(Q), new(R)]),
                ExpectedEquality: false),

            new(
                X: new([new(P), new(Q), new(R)]),
                Y: new([new(P), new(Q)]),
                ExpectedEquality: false),
        ])
        .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
        .ThenReturns()
        .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
        .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality)); // <- yeah yeah, strictly speaking not the right thing to be asserting, but..

    private record ToFormulaTestCase(CNFFormula CNFFormula, Formula ExpectedFormula);

    private record EqualityTestCase(CNFFormula X, CNFFormula Y, bool ExpectedEquality);

}
