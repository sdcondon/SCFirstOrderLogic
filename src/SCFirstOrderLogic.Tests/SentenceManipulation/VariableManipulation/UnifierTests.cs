using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class UnifierTests
{
    private static Function John => new(nameof(John));
    private static Function Jane => new(nameof(Jane));
    private static Function Mother(Term child) => new(nameof(Mother), child);
    private static Function Father(Term child) => new(nameof(Father), child);
    private static Predicate Knows(Term knower, Term known) => new(nameof(Knows), knower, known);

    public static Test TryCreatePositive => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(John, X),
                Input2: Knows(John, Jane),
                ExpectedSubstitutions: new()
                {
                    [X] = Jane,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(John, X),
                Input2: Knows(Y, Jane),
                ExpectedSubstitutions: new()
                {
                    [X] = Jane,
                    [Y] = John,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(John, X),
                Input2: Knows(Y, Mother(Y)),
                ExpectedSubstitutions: new()
                {
                    [X] = Mother(Y),
                    [Y] = John,
                },
                ExpectedUnified: Knows(John, Mother(John))),

            new ( // vars matched to vars
                Input1: Knows(X, X),
                Input2: Knows(Y, John),
                ExpectedSubstitutions: new()
                {
                    [X] = Y,
                    [Y] = John,
                },
                ExpectedUnified: Knows(John, John)),

            new ( // vars matched to vars
                Input1: Knows(Y, John),
                Input2: Knows(X, X),
                ExpectedSubstitutions: new()
                {
                    [X] = John,
                    [Y] = X,
                },
                ExpectedUnified: Knows(John, John)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryCreateNegative => TestThat
        .GivenEachOf<TryCreateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(John, X),
                Input2: Knows(X, Jane)),

            new ( // trivial occurs check faiure
                Input1: Knows(John, X),
                Input2: Knows(John, Mother(X))),

            new ( // non-trivial occurs check failure
                Input1: Knows(X, Mother(X)),
                Input2: Knows(Father(Y), Y)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Should().BeNull());

    public static Test TryUpdatePositive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [X] = John,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = John,
                    [Y] = Jane,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [Y] = Jane,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = John,
                    [Y] = Jane,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(A, B),
                InitialSubstitutions: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedUnified: Knows(A, B)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialSubstitutions);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialSubstitutions), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryUpdateRefPositive => TestThat
        .GivenEachOf<TryUpdatePositiveTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [X] = John,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = John,
                    [Y] = Jane,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [Y] = Jane,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = John,
                    [Y] = Jane,
                },
                ExpectedUnified: Knows(John, Jane)),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(A, B),
                InitialSubstitutions: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedSubstitutions: new()
                {
                    [X] = A,
                    [Y] = B,
                },
                ExpectedUnified: Knows(A, B)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialSubstitutions);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier.Bindings.Should().Equal(tc.ExpectedSubstitutions))
        .And((tc, r) => r.unifier.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    public static Test TryUpdateNegative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [Y] = John,
                }),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [X] = Jane,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.unifier = new(tc.InitialSubstitutions);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, new(tc.InitialSubstitutions), out result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Should().BeNull());

    public static Test TryUpdateRefNegative => TestThat
        .GivenEachOf<TryUpdateNegativeTestCase<Predicate>>(() =>
        [
            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [Y] = John,
                }),

            new (
                Input1: Knows(X, Y),
                Input2: Knows(John, Jane),
                InitialSubstitutions: new()
                {
                    [X] = Jane,
                }),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution unifier) result;
            result.unifier = new(tc.InitialSubstitutions);
            result.returnValue = Unifier.TryUpdate(tc.Input1, tc.Input2, ref result.unifier);
            return result;
        })
        .ThenReturns()
        .And((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Bindings.Should().BeEquivalentTo(tc.InitialSubstitutions));

    public static Test TryCreatePositive_Terms => TestThat
        .GivenEachOf<TryCreatePositiveTestCase<Term>>(() =>
        [
            new (
                Input1: X,
                Input2: Mother(John),
                ExpectedSubstitutions: new()
                {
                    [X] = Mother(John),
                },
                ExpectedUnified: Mother(John)),
        ])
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
        .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input1).Should().Be(tc.ExpectedUnified))
        .And((tc, r) => r.unifier!.ApplyTo(tc.Input2).Should().Be(tc.ExpectedUnified));

    private record TryCreatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> ExpectedSubstitutions,
        T ExpectedUnified);

    private record TryCreateNegativeTestCase<T>(
        T Input1,
        T Input2);

    private record TryUpdatePositiveTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialSubstitutions,
        Dictionary<VariableReference, Term> ExpectedSubstitutions,
        T? ExpectedUnified);

    private record TryUpdateNegativeTestCase<T>(
        T Input1,
        T Input2,
        Dictionary<VariableReference, Term> InitialSubstitutions);
}
