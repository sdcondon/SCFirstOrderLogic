using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public static class VariableSubstitutionTests
{
    // TODO: Application of substitution tested via Unifier tests - shouldn't be

    private record EqualityTestCase(VariableSubstitution X, VariableSubstitution Y, bool ExpectedEquality);

    public static Test EqualityBehaviour => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: new([]),
                Y: new([]),
                ExpectedEquality: true),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Function("C")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Function("C")
                }),
                ExpectedEquality: true),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Function("C1")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Function("C2")
                }),
                ExpectedEquality: false),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Function("C1")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Function("C1"),
                    [new VariableReference("V2")] = new Function("C2")
                }),
                ExpectedEquality: false),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V2")] = new Function("C2"),
                    [new VariableReference("V1")] = new Function("C1"),   
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Function("C1")
                }),
                ExpectedEquality: false),
        ])
        .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
        .ThenReturns()
        .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
        .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality));
}
