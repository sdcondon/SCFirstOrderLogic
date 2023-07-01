﻿using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    public class LiteralUnifierTests
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
            .GivenEachOf(() => new TryUnifyPositiveTestCase[]
            {
                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(john, jane),
                    ExpectedSubstitutions: new()
                    {
                        [x] = jane,
                    },
                    ExpectedUnified: Knows(john, jane)),

                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, jane),
                    ExpectedSubstitutions: new()
                    {
                        [x] = jane,
                        [y] = john,
                    },
                    ExpectedUnified: Knows(john, jane)),

                new ( // out of scope?
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, Mother(y)),
                    ExpectedSubstitutions: new()
                    {
                        [x] = Mother(y),
                        [y] = john,
                    },
                    ExpectedUnified: Knows(john, Mother(john))),

                new ( // vars matched to vars - out of scope?
                    Literal1: Knows(x, x),
                    Literal2: Knows(y, john),
                    ExpectedSubstitutions: new()
                    {
                        [x] = y,
                        [y] = john,
                    },
                    ExpectedUnified: Knows(john, john)),

                new ( // vars matched to vars - out of scope?
                    Literal1: Knows(y, john),
                    Literal2: Knows(x, x),
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
                result.returnValue = Unifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal1).Should().Be(tc.ExpectedUnified))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal2).Should().Be(tc.ExpectedUnified));

        public static Test TryCreateNegative => TestThat
            .GivenEachOf(() => new TryUnifyNegativeTestCase[]
            {
                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(x, jane)),

                new ( // trivial occurs check faiure
                    Literal1: Knows(john, x),
                    Literal2: Knows(john, Mother(x))),

                new ( // non-trivial occurs check failure
                    Literal1: Knows(x, Mother(x)),
                    Literal2: Knows(Father(y), y)),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.returnValue = Unifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
            .And((tc, r) => r.unifier.Should().BeNull());

        public static Test TryUpdatePositive => TestThat
            .GivenEachOf(() => new TryUpdatePositiveTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
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
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
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
                    Literal1: Knows(x, y),
                    Literal2: Knows(a, b),
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
                result.returnValue = Unifier.TryUpdate(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal1).Should().Be(tc.ExpectedUnified))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal2).Should().Be(tc.ExpectedUnified));

        public static Test TryUpdateUnsafePositive => TestThat
            .GivenEachOf(() => new TryUpdatePositiveTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
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
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
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
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.unifier = new(tc.InitialSubstitutions);
                result.returnValue = Unifier.TryUpdateUnsafe(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal1).Should().Be(tc.ExpectedUnified))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal2).Should().Be(tc.ExpectedUnified));

        public static Test TryUpdateNegative => TestThat
            .GivenEachOf(() => new TryUpdateNegativeTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new()
                    {
                        [y] = john,
                    }),

                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new()
                    {
                        [x] = jane,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution unifier) result;
                result.unifier = new(tc.InitialSubstitutions);
                result.returnValue = Unifier.TryUpdate(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
            .And((tc, r) => r.unifier.Bindings.Should().BeEquivalentTo(tc.InitialSubstitutions));

        public static Test TryUpdateUnsafeNegative => TestThat
            .GivenEachOf(() => new TryUpdateNegativeTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new()
                    {
                        [y] = john,
                    }),

                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new()
                    {
                        [x] = jane,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.unifier = new(tc.InitialSubstitutions);
                result.returnValue = Unifier.TryUpdateUnsafe(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse());

        private record TryUnifyPositiveTestCase(
            Literal Literal1,
            Literal Literal2,
            Dictionary<VariableReference, Term> ExpectedSubstitutions,
            Literal ExpectedUnified);

        private record TryUnifyNegativeTestCase(
            Literal Literal1,
            Literal Literal2);

        private record TryUpdatePositiveTestCase(
            Literal Literal1,
            Literal Literal2,
            Dictionary<VariableReference, Term> InitialSubstitutions,
            Dictionary<VariableReference, Term> ExpectedSubstitutions,
            Literal? ExpectedUnified);

        private record TryUpdateNegativeTestCase(
            Literal Literal1,
            Literal Literal2,
            Dictionary<VariableReference, Term> InitialSubstitutions);
    }
}
