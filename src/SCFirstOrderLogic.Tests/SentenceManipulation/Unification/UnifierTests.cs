using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.Unification;

public class UnifierTests
{
    private static Function Mother(Term child) => new(nameof(Mother), child);
    private static Function Father(Term child) => new(nameof(Father), child);
    private static Predicate Knows(Term knower, Term known) => new(nameof(Knows), knower, known);

    private static readonly Constant john = new("John");
    private static readonly Constant jane = new("Jane");
    private static readonly VariableDeclaration x = new(nameof(x));
    private static readonly VariableDeclaration y = new(nameof(y));
    private static readonly VariableDeclaration a = new(nameof(a));
    private static readonly VariableDeclaration b = new(nameof(b));

    public static Test TryCreatePositive => TestThat
        .GivenEachOf(() => new TryCreatePositiveTestCase<Predicate>[]
        {
            new (
                Input1: Knows(john, x),
                Input2: Knows(john, jane),
                ExpectedSubstitutions: new()
                {
                    [x] = jane,
                },
                ExpectedUnified: Knows(john, jane)),

            new (
                Input1: Knows(john, x),
                Input2: Knows(y, jane),
                ExpectedSubstitutions: new()
                {
                    [x] = jane,
                    [y] = john,
                },
                ExpectedUnified: Knows(john, jane)),

            new ( // out of scope?
                Input1: Knows(john, x),
                Input2: Knows(y, Mother(y)),
                ExpectedSubstitutions: new()
                {
                    [x] = Mother(y),
                    [y] = john,
                },
                ExpectedUnified: Knows(john, Mother(john))),

            new ( // vars matched to vars - out of scope?
                Input1: Knows(x, x),
                Input2: Knows(y, john),
                ExpectedSubstitutions: new()
                {
                    [x] = y,
                    [y] = john,
                },
                ExpectedUnified: Knows(john, john)),

            new ( // vars matched to vars - out of scope?
                Input1: Knows(y, john),
                Input2: Knows(x, x),
                ExpectedSubstitutions: new()
                {
                    [x] = john,
                    [y] = x,
                },
                ExpectedUnified: Knows(john, john)),
        })
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
        .GivenEachOf(() => new TryCreateNegativeTestCase<Predicate>[]
        {
            new (
                Input1: Knows(john, x),
                Input2: Knows(x, jane)),

            new ( // trivial occurs check faiure
                Input1: Knows(john, x),
                Input2: Knows(john, Mother(x))),

            new ( // non-trivial occurs check failure
                Input1: Knows(x, Mother(x)),
                Input2: Knows(Father(y), y)),
        })
        .When(tc =>
        {
            (bool returnValue, VariableSubstitution? unifier) result;
            result.returnValue = Unifier.TryCreate(tc.Input1, tc.Input2, out result.unifier);
            return result;
        })
        .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
        .And((tc, r) => r.unifier.Should().BeNull());

    public static Test TryUpdatePositive => TestThat
        .GivenEachOf(() => new TryUpdatePositiveTestCase<Predicate>[]
        {
            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [x] = john,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = john,
                    [y] = jane,
                },
                ExpectedUnified: Knows(john, jane)),

            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [y] = jane,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = john,
                    [y] = jane,
                },
                ExpectedUnified: Knows(john, jane)),

            new (
                Input1: Knows(x, y),
                Input2: Knows(a, b),
                InitialSubstitutions: new()
                {
                    [x] = a,
                    [y] = b,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = a,
                    [y] = b,
                },
                ExpectedUnified: Knows(a, b)),
        })
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
        .GivenEachOf(() => new TryUpdatePositiveTestCase<Predicate>[]
        {
            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [x] = john,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = john,
                    [y] = jane,
                },
                ExpectedUnified: Knows(john, jane)),

            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [y] = jane,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = john,
                    [y] = jane,
                },
                ExpectedUnified: Knows(john, jane)),

            new (
                Input1: Knows(x, y),
                Input2: Knows(a, b),
                InitialSubstitutions: new()
                {
                    [x] = a,
                    [y] = b,
                },
                ExpectedSubstitutions: new()
                {
                    [x] = a,
                    [y] = b,
                },
                ExpectedUnified: Knows(a, b)),
        })
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
        .GivenEachOf(() => new TryUpdateNegativeTestCase<Predicate>[]
        {
            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [y] = john,
                }),

            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [x] = jane,
                }),
        })
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
        .GivenEachOf(() => new TryUpdateNegativeTestCase<Predicate>[]
        {
            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [y] = john,
                }),

            new (
                Input1: Knows(x, y),
                Input2: Knows(john, jane),
                InitialSubstitutions: new()
                {
                    [x] = jane,
                }),
        })
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
        .GivenEachOf(() => new TryCreatePositiveTestCase<Term>[]
        {
            new (
                Input1: x,
                Input2: Mother(john),
                ExpectedSubstitutions: new()
                {
                    [x] = Mother(john),
                },
                ExpectedUnified: Mother(john)),
        })
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
