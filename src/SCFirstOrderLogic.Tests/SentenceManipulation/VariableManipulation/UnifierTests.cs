using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class UnifierTests
{
    public static Test TryCreateFromPredicates_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(C, X),
                Input2: P(C, D),
                ExpectedBindings: new()
                {
                    [X] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(C, X),
                Input2: P(Y, D),
                ExpectedBindings: new()
                {
                    [X] = D,
                    [Y] = C,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(C, X),
                Input2: P(Y, F(Y)),
                ExpectedBindings: new()
                {
                    [X] = F(Y),
                    [Y] = C,
                },
                ExpectedUnified: P(C, F(C))),

            new ( // vars matched to vars
                Input1: P(X, X),
                Input2: P(Y, C),
                ExpectedBindings: new()
                {
                    [X] = Y,
                    [Y] = C,
                },
                ExpectedUnified: P(C, C)),

            new ( // vars matched to vars
                Input1: P(Y, C),
                Input2: P(X, X),
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = X,
                },
                ExpectedUnified: P(C, C)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryCreateFromPredicates_Negative => TestThat
        .GivenEachOf<TryCreateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(C, X),
                Input2: P(X, D)),

            new ( // trivial occurs check faiure
                Input1: P(C, X),
                Input2: P(C, F(X))),

            new ( // non-trivial occurs check failure
                Input1: P(X, F(X)),
                Input2: P(G(Y), Y)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateFromPredicates_Positive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(X, Y),
                Input2: P(A, B),
                InitialBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedUnified: P(A, B)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryUpdateFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = C,
                }),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = D,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateRefFromPredicates_Positive => TestThat
    .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
    [
        new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Input1: P(X, Y),
                Input2: P(A, B),
                InitialBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedUnified: P(A, B)),
    ])
    .When(tc =>
    {
        (bool returnValue, VariableSubstitution unifier) result;
        result.unifier = new(tc.InitialBindings);
        result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
        return result;
    })
    .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
    .And((tc, r) => r.unifier.Bindings.Should().Equal(tc.ExpectedBindings))
    .And((tc, r) => r.unifier.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
    .And((tc, r) => r.unifier.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryUpdateRefFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = C,
                }),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = D,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Bindings.Should().BeEquivalentTo(tc.InitialBindings));

    public static Test TryCreateFromTerms_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Term>>(() =>
        [
            new (
                Input1: X,
                Input2: F(C),
                ExpectedBindings: new()
                {
                    [X] = F(C),
                },
                ExpectedUnified: F(C)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    private record TryCreatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> ExpectedBindings,
        T ExpectedUnified);

    private record TryCreateNegativeTestCase<T>(
        T Input1,
        T Input2);

    private record TryUpdatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialBindings,
        Dictionary<VariableReference, Term> ExpectedBindings,
        T? ExpectedUnified);

    private record TryUpdateNegativeTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialBindings);
}
