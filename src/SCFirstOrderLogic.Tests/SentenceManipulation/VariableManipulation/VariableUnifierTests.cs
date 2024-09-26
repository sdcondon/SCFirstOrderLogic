using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class VariableUnifierTests
{
    public static Test TryCreateFromPredicates_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(C, F(X)),
                Input2: P(C, F(Y)),
                ExpectedBindings: new()
                {
                    [X] = Y,
                }),

            new (
                Input1: P(F(X), F(Y)),
                Input2: P(F(Y), F(X)),
                ExpectedBindings: new()
                {
                    [X] = Y,
                    [Y] = X,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = VariableUnifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryCreateFromPredicates_Negative => TestThat
        .GivenEachOf<TryCreateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(C, X),
                Input2: P(C, D)),

            new (
                Input1: P(C, X),
                Input2: P(Y, D)),

            new (
                Input1: P(C, X),
                Input2: P(Y, F(Y))),

            new (
                Input1: P(X, X),
                Input2: P(Y, C)),

            new (
                Input1: P(Y, C),
                Input2: P(X, X)),

            new (
                Input1: P(C, X),
                Input2: P(X, D)),

            new (
                Input1: P(C, X),
                Input2: P(C, F(X))),

            new (
                Input1: P(X, F(X)),
                Input2: P(G(Y), Y)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = VariableUnifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeFalse())
        .And((_, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateFromPredicates_Positive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(Y, X),
                InitialBindings: new()
                {
                    [X] = Y,
                },
                ExpectedBindings: new()
                {
                    [X] = Y,
                    [Y] = X,
                }),

            new (
                Input1: P(X, Y),
                Input2: P(U, V),
                InitialBindings: new()
                {
                    [X] = U,
                    [Y] = V,
                },
                ExpectedBindings: new()
                {
                    [X] = U,
                    [Y] = V,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = VariableUnifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryUpdateFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                }),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                }),

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
            result.returnValue = VariableUnifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeFalse())
        .And((_, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateRefFromPredicates_Positive => TestThat
    .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
    [
        new (
            Input1: P(X, Y),
            Input2: P(Y, X),
            InitialBindings: new()
            {
                [X] = Y,
            },
            ExpectedBindings: new()
            {
                [X] = Y,
                [Y] = X,
            }),

        new (
            Input1: P(X, Y),
            Input2: P(U, V),
            InitialBindings: new()
            {
                [X] = U,
                [Y] = V,
            },
            ExpectedBindings: new()
            {
                [X] = U,
                [Y] = V,
            }),
    ])
    .When(tc =>
    {
        (bool returnValue, VariableSubstitution unifier) result;
        result.unifier = new(tc.InitialBindings);
        result.returnValue = VariableUnifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
        return result;
    })
    .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
    .And((tc, r) => r.unifier.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryUpdateRefFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                }),

            new (
                Input1: P(X, Y),
                Input2: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                }),

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
            result.returnValue = VariableUnifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Bindings.Should().BeEquivalentTo(tc.InitialBindings));

    private record TryCreatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> ExpectedBindings);

    private record TryCreateNegativeTestCase<T>(
        T Input1,
        T Input2);

    private record TryUpdatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialBindings,
        Dictionary<VariableReference, Term> ExpectedBindings);

    private record TryUpdateNegativeTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialBindings);
}
