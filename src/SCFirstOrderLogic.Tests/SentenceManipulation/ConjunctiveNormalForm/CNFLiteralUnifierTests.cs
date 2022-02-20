using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public class CNFLiteralUnifierTests
    {
        private static Function Mother(Term child) => new("Mother", child);

        private static Predicate Knows(Term knower, Term known) => new("Knows", knower, known);

        private static readonly Constant john = new("John");
        private static readonly Constant jane = new("Jane");
        private static readonly VariableDeclaration x = new("x");
        private static readonly VariableDeclaration y = new("y");

        private record TestCase(CNFLiteral Literal1, CNFLiteral Literal2, Dictionary<VariableReference, Term>? ExpectedSubstitutions = null);

        public static Test TryUnifyPositive => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Literal1: Knows(john, x),
                    Literal2: Knows(john, jane),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                    }),

                new TestCase(
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, jane),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                        [y] = john,
                    }),

                new TestCase(
                    Literal1: Knows(john, x),
                    Literal2: Knows(y, Mother(y)),
                    ExpectedSubstitutions: new Dictionary<VariableReference, Term>()
                    {
                        // Book says that x should be Mother(john), but that's not what the algorithm they give
                        // produces. Easy enough to resolve (after all, y is john), but waiting and seeing how it pans out through usage..
                        [x] = Mother(y),
                        [y] = john,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, CNFLiteralUnifier? unifier) result;
                result.returnValue = CNFLiteralUnifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier!.Substitutions.Should().Equal(tc.ExpectedSubstitutions));

        public static Test TryUnifyNegative => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Literal1: Knows(john, x),
                    Literal2: Knows(x, jane)),
            })
            .When(tc =>
            {
                (bool returnValue, CNFLiteralUnifier? unifier) result;
                result.returnValue = CNFLiteralUnifier.TryCreate(tc.Literal1, tc.Literal2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
            .And((tc, r) => r.unifier.Should().BeNull());
    }
}
