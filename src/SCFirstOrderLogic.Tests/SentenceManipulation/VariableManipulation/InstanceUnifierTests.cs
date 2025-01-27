using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.SentenceCreation.Specialised.GenericDomainOperableSentenceFactory;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class InstanceUnifierTests
{
    public static Test TryCreateFromPredicates_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(C, X),
                Instance: P(C, D),
                ExpectedBindings: new()
                {
                    [X] = D,
                }),

            new (
                Generalisation: P(C, X),
                Instance: P(C, F(Y)),
                ExpectedBindings: new()
                {
                    [X] = F(Y),
                }),

            new ( // vars matched to vars
                Generalisation: P(X, X),
                Instance: P(Y, C),
                ExpectedBindings: new()
                {
                    [X] = Y,
                    [Y] = C,
                }),

            // NB: at the time of writing this will cause infinite recursion if actually applied - since there's no loop protection.
            // Need to decide whether I'd prefer to leverage loop protection, or add an occurs check (and thus require standardisation
            // where needed to use it). In the meantime, this is why the type is internal.
            new (
                Generalisation: P(C, X),
                Instance: P(C, F(X)),
                ExpectedBindings: new()
                {
                    [X] = F(X),
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = InstanceUnifier.TryCreate(tc.Generalisation, tc.Instance, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryCreateFromPredicates_Negative => TestThat
        .GivenEachOf<TryCreateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(C, X),
                Instance: P(X, D)),

            new (
                Generalisation: P(C, X),
                Instance: P(Y, D)),

            new (
                Generalisation: P(C, X),
                Instance: P(Y, F(Y))),

            new (
                Generalisation: P(X, F(X)),
                Instance: P(G(Y), Y)),

            new ( // vars matched to vars
                Generalisation: P(Y, C),
                Instance: P(X, X)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = InstanceUnifier.TryCreate(tc.Generalisation, tc.Instance, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeFalse())
        .And((_, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateFromPredicates_Positive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(A, B),
                InitialBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryUpdateFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [Y] = C,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [X] = D,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeFalse())
        .And((_, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateRefFromPredicates_Positive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [X] = C,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [Y] = D,
                },
                ExpectedBindings: new()
                {
                    [X] = C,
                    [Y] = D,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(A, B),
                InitialBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedBindings: new()
                {
                    [X] = A,
                    [Y] = B,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, ref result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier.Bindings.Should().Equal(tc.ExpectedBindings));

    public static Test TryUpdateRefFromPredicates_Negative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [Y] = C,
                }),

            new (
                Generalisation: P(X, Y),
                Instance: P(C, D),
                InitialBindings: new()
                {
                    [X] = D,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, ref result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Bindings.Should().BeEquivalentTo(tc.InitialBindings));

    public static Test TryCreateFromTerms_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Term>>(() =>
        [
            new (
                Generalisation: X,
                Instance: F(C),
                ExpectedBindings: new()
                {
                    [X] = F(C),
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Generalisation, tc.Instance, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings));

    private record TryCreatePositiveTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> ExpectedBindings);

    private record TryCreateNegativeTestCase<T>(
        T Generalisation,
        T Instance);

    private record TryUpdatePositiveTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> InitialBindings,
        Dictionary<VariableReference, Term> ExpectedBindings);

    private record TryUpdateNegativeTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> InitialBindings);
}
