using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.FormulaCreation.Specialised.GenericDomainOperableFormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

public static class VariableSubstitutionTests
{
    public static Test ApplyToSentenceBehaviour => TestThat
        .GivenEachOf<ApplyToSentenceTestCase>(() =>
        [
            new(
                Bindings: [],
                Input: P(C, X),
                Expected: P(C, X)),

            new(
                Bindings: new()
                {
                    [Y] = D,
                },
                Input: P(C, X),
                Expected: P(C, X)),

            new(
                Bindings: new()
                {
                    [X] = D,
                },
                Input: P(C, X),
                Expected: P(C, D)),

            new(
                Bindings: new()
                {
                    [X] = F(Y),
                },
                Input: P(C, X),
                Expected: P(C, F(Y))),

            new(
                Bindings: new()
                {
                    [X] = Y,
                    [Y] = D,
                },
                Input: P(C, X),
                Expected: P(C, D)),

            // TODO-ROBUSTNESS: Yeah, these cause an infinite loop as-is. Relatively simple fix, but this
            // is low level code, and I want to allow people to opt out when there's no risk of it (for performance).
            // And when they don't, there's probably scope for deciding between an exception and just breaking, as below. 
            ////new(
            ////    Bindings: new()
            ////    {
            ////        [X] = F(X),
            ////    },
            ////    InputTerm: P(C, X),
            ////    Expected: P(C, F(X))),
            ////
            ////new(
            ////    Bindings: new()
            ////    {
            ////        [X] = F(Y),
            ////        [Y] = F(X),
            ////    },
            ////    InputTerm: P(C, X),
            ////    Expected: P(C, F(F(X)))),
        ])
        .When(tc => new VariableSubstitution(tc.Bindings).ApplyTo(tc.Input))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test ApplyToCNFSentenceBehaviour => TestThat
        .GivenEachOf<ApplyToSentenceTestCase>(() =>
        [
            new(
                Bindings: [],
                Input: P(C, X),
                Expected: P(C, X)),

            new(
                Bindings: new()
                {
                    [Y] = D,
                },
                Input: P(C, X),
                Expected: P(C, X)),

            new(
                Bindings: new()
                {
                    [X] = D,
                },
                Input: P(C, X),
                Expected: P(C, D)),

            new(
                Bindings: new()
                {
                    [X] = F(Y),
                },
                Input: P(C, X),
                Expected: P(C, F(Y))),

            new(
                Bindings: new()
                {
                    [X] = Y,
                    [Y] = D,
                },
                Input: P(C, X),
                Expected: P(C, D)),

            // TODO-ROBUSTNESS: Yeah, these cause an infinite loop as-is. Relatively simple fix, but this
            // is low level code, and I want to allow people to opt out when there's no risk of it (for performance).
            // And when they don't, there's probably scope for deciding between an exception and just breaking, as below. 
            ////new(
            ////    Bindings: new()
            ////    {
            ////        [X] = F(X),
            ////    },
            ////    InputTerm: P(C, X),
            ////    Expected: P(C, F(X))),
            ////
            ////new(
            ////    Bindings: new()
            ////    {
            ////        [X] = F(Y),
            ////        [Y] = F(X),
            ////    },
            ////    InputTerm: P(C, X),
            ////    Expected: P(C, F(F(X)))),
        ])
        .When(tc => new VariableSubstitution(tc.Bindings).ApplyTo(new CNFFormula(tc.Input)))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(new CNFFormula(tc.Expected)));

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

    private record ApplyToSentenceTestCase(
        Dictionary<VariableReference, Term> Bindings,
        Formula Input,
        Formula Expected);

    private record ApplyToCNFSentenceTestCase(
        Dictionary<VariableReference, Term> Bindings,
        CNFFormula Input,
        CNFFormula Expected);

    private record EqualityTestCase(
        VariableSubstitution X,
        VariableSubstitution Y,
        bool ExpectedEquality);
}
