using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    public class SentenceUnifierTests
    {
        private static Function Mother(Term child) => new("Mother", child);

        private static Predicate Knows(Term knower, Term known) => new("Knows", knower, known);

        private static readonly Constant john = new("John");
        private static readonly Constant jane = new("Jane");
        private static readonly VariableDeclaration x = new("x");
        private static readonly VariableDeclaration y = new("y");

        private record TestCase(Sentence Sentence1, Sentence Sentence2, Dictionary<VariableReference, Term>? ExpectedUnifier = null);

        public static Test TryUnifyPositive => TestThat
            .GivenEachOf(() => new TestCase[]
            {
                new(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(john, jane),
                    ExpectedUnifier: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                    }),

                new(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, jane),
                    ExpectedUnifier: new Dictionary<VariableReference, Term>()
                    {
                        [x] = jane,
                        [y] = john,
                    }),

                new(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, Mother(y)),
                    ExpectedUnifier: new Dictionary<VariableReference, Term>()
                    {
                        // Book says that x should be Mother(john), but that's not what the algorithm they give
                        // produces. Easy enough to resolve (after all, y is john), but waiting and seeing how it pans out through usage..
                        [x] = Mother(john),
                        [y] = john,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, IDictionary<VariableReference, Term>? unifier) result;
                result.returnValue = new SentenceUnifier().TryUnify(tc.Sentence1, tc.Sentence2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue())
            .And((tc, r) => r.unifier.Should().Equal(tc.ExpectedUnifier));

        public static Test TryUnifyNegative => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(x, jane)),
            })
            .When(tc =>
            {
                (bool returnValue, IDictionary<VariableReference, Term>? unifier) result;
                result.returnValue = new SentenceUnifier().TryUnify(tc.Sentence1, tc.Sentence2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse())
            .And((tc, r) => r.unifier.Should().BeNull());
    }
}
