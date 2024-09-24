using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class InstanceUnifierTests
{
    public static Test TryCreateFromPredicates_Positive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Predicate>>(() =>
        ([
            new (
                Generalisation: P(C, X),
                Instance: P(C, D),
                ExpectedBindings: new()
                {
                    [X] = D,
                },
                ExpectedUnified: P(C, D)),

            new (
                Generalisation: P(C, X),
                Instance: P(C, F(Y)),
                ExpectedBindings: new()
                {
                    [X] = F(Y),
                },
                ExpectedUnified: P(C, C)),

            new ( // vars matched to vars
                Generalisation: P(X, X),
                Instance: P(Y, C),
                ExpectedBindings: new()
                {
                    [X] = Y,
                    [Y] = C,
                },
                ExpectedUnified: P(C, C)),

            // NB: this will cause infinite recursion due to combination of lack of occurs check
            // in unifier, and lack of loop protection in substitution - both broadly on performance
            // grounds (this is low-level code - need to take performance somewhat seriously).
            // To be resolved one way or the other before considering making this type public - a few options here.
            ////new (
            ////    Generalisation: P(C, X),
            ////    Instance: P(C, F(X)),
            ////    ExpectedSubstitutions: new()
            ////    {
            ////        [X] = F(X),
            ////    },
            ////    ExpectedUnified: P(C, F(X))),
        ]))
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = InstanceUnifier.TryCreate(tc.Generalisation, tc.Instance, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Generalisation).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Instance).Should().Be(tc.ExpectedUnified));

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
        ([
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
                },
                ExpectedUnified: P(C, D)),

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
                },
                ExpectedUnified: P(C, D)),

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
                },
                ExpectedUnified: P(A, B)),
        ]))
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, new(tc.InitialBindings), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Generalisation).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Instance).Should().Be(tc.ExpectedUnified));

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
        ([
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
                },
                ExpectedUnified: P(C, D)),

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
                },
                ExpectedUnified: P(C, D)),

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
                },
                ExpectedUnified: P(A, B)),
        ]))
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialBindings);
            result.returnValue = InstanceUnifier.TryUpdate(tc.Generalisation, tc.Instance, ref result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier.ApplyTo(tc.Generalisation).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier.ApplyTo(tc.Instance).Should().Be(tc.ExpectedUnified));

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
        ([
            new (
                Generalisation: X,
                Instance: F(C),
                ExpectedBindings: new()
                {
                    [X] = F(C),
                },
                ExpectedUnified: F(C)),
        ]))
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Generalisation, tc.Instance, out result.unifier);
            return result;
        })
        .ThenReturns((_, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedBindings))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Generalisation).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Instance).Should().Be(tc.ExpectedUnified));

    private record TryCreatePositiveTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> ExpectedBindings,
        T ExpectedUnified);

    private record TryCreateNegativeTestCase<T>(
        T Generalisation,
        T Instance);

    private record TryUpdatePositiveTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> InitialBindings,
        Dictionary<VariableReference, Term> ExpectedBindings,
        T? ExpectedUnified);

    private record TryUpdateNegativeTestCase<T>(
        T Generalisation,
        T Instance,
        Dictionary<VariableReference, Term> InitialBindings);
}
