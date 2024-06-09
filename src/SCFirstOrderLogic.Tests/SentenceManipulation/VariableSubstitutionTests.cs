using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation;

public static class VariableSubstitutionTests
{
    // NB: Application of substitution tested via Unifier tests - probably shouldn't be

    private record EqualityTestCase(VariableSubstitution X, VariableSubstitution Y, bool ExpectedEquality);

    public static Test EqualityBehaviour => TestThat
        .GivenEachOf(() => new EqualityTestCase[]
        {
            new(
                X: new([]),
                Y: new([]),
                ExpectedEquality: true),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Constant("C")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Constant("C")
                }),
                ExpectedEquality: true),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Constant("C1")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V")] = new Constant("C2")
                }),
                ExpectedEquality: false),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Constant("C1")
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Constant("C1"),
                    [new VariableReference("V2")] = new Constant("C2")
                }),
                ExpectedEquality: false),

            new(
                X: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V2")] = new Constant("C2"),
                    [new VariableReference("V1")] = new Constant("C1"),   
                }),
                Y: new(new Dictionary<VariableReference, Term>()
                {
                    [new VariableReference("V1")] = new Constant("C1")
                }),
                ExpectedEquality: false),
        })
        .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
        .ThenReturns()
        .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
        .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality));
}
