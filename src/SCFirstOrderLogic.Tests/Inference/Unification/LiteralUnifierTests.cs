using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.Unification
{
    public class LiteralUnifierTests
    {
        private static Function Mother(Term child) => new(nameof(Mother), child);

        private static Predicate Knows(Term knower, Term known) => new(nameof(Knows), knower, known);

        private static readonly Constant john = new("John");
        private static readonly Constant jane = new("Jane");
        private static readonly VariableDeclaration x = new(nameof(x));
        private static readonly VariableDeclaration y = new(nameof(y));

        private record TryUnifyPositiveTestCase(CNFLiteral Literal1, CNFLiteral Literal2, Dictionary<VariableReference, Term> ExpectedSubstitutions, CNFLiteral ExpectedUnified);

        public static Test TryCreatePositive => TestThat
            .GivenEachOf(() => new TryUnifyPositiveTestCase[]
            {
                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(john, jane),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                    },
                    ExpectedUnified: Knows(john, jane)),

                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, jane),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                        [y] = john,
                    },
                    ExpectedUnified: Knows(john, jane)),

                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, Mother(y)),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = Mother(john),
                        [y] = john,
                    },
                    ExpectedUnified: Knows(john, Mother(john))),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.returnValue = LiteralUnifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal1).Should().Be(tc.ExpectedUnified))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal2).Should().Be(tc.ExpectedUnified));

        private record TryUnifyNegativeTestCase(CNFLiteral Literal1, CNFLiteral Literal2);

        public static Test TryCreateNegative => TestThat
            .GivenEachOf(() => new TryUnifyNegativeTestCase[]
            {
                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(x, jane)),

                new (
                    Literal1: Knows(john, x),
                    Literal2: Knows(john, Mother(x))),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.returnValue = LiteralUnifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
            .And((tc, r) => r.unifier.Should().BeNull());

        private record TryUpdatePositiveTestCase(CNFLiteral Literal1, CNFLiteral Literal2, Dictionary<VariableReference, Term> InitialSubstitutions, Dictionary<VariableReference, Term> ExpectedSubstitutions, CNFLiteral? ExpectedUnified);

        public static Test TryUpdatePositive => TestThat
            .GivenEachOf(() => new TryUpdatePositiveTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = john,
                    },
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = john,
                        [y] = jane,
                    },
                    ExpectedUnified: Knows(john, jane)),

                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [y] = jane,
                    },
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
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
                result.returnValue = LiteralUnifier.TryUpdate(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Bindings.Should().Equal(tc.ExpectedSubstitutions))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal1).Should().Be(tc.ExpectedUnified))
            .And((tc, r) => r.unifier!.ApplyTo(tc.Literal2).Should().Be(tc.ExpectedUnified));

        private record TryUpdateNegativeTestCase(CNFLiteral Literal1, CNFLiteral Literal2, Dictionary<VariableReference, Term> InitialSubstitutions);

        public static Test TryUpdateNegative => TestThat
            .GivenEachOf(() => new TryUpdateNegativeTestCase[]
            {
                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [y] = john,
                    }),

                new (
                    Literal1: Knows(x, y),
                    Literal2: Knows(john, jane),
                    InitialSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, VariableSubstitution? unifier) result;
                result.unifier = new(tc.InitialSubstitutions);
                result.returnValue = LiteralUnifier.TryUpdate(tc.Literal1, tc.Literal2, result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse());
    }
}
