using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.LanguageIntegration;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation
{
    public class SentenceUnifierTests
    {
        private static Function Mother(Term child) => new Function("Mother", child);

        private static Predicate Knows(Term knower, Term known) => new Predicate("Knows", knower, known);

        private static readonly Constant john = new Constant("John");
        private static readonly Constant jane = new Constant("Jane");
        private static readonly Variable x = new Variable(new VariableDeclaration("x"));
        private static readonly Variable y = new Variable(new VariableDeclaration("y"));

        private record TestCase(Sentence Sentence1, Sentence Sentence2, Dictionary<Variable, Term> ExpectedUnifier = null);

        public static Test TryUnifyPositive => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(john, jane),
                    ExpectedUnifier: new Dictionary<Variable, Term>()
                    {
                        [x] = jane,
                    }),

                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, jane),
                    ExpectedUnifier: new Dictionary<Variable, Term>()
                    {
                        [x] = jane,
                        [y] = john,
                    }),

                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, Mother(y)),
                    ExpectedUnifier: new Dictionary<Variable, Term>()
                    {
                        // Book says that x should be Mother(john), but that's not what the algorithm they give
                        // produces. Easy enough to resolve (after all, y is john), but waiting and seeing how it pans out through usage..
                        [x] = Mother(y),
                        [y] = john,
                    }),
            })
            .When(tc =>
            {
                (bool returnValue, IDictionary<Variable, Term> unifier) result;
                result.returnValue = new SentenceUnifier().TryUnify(tc.Sentence1, tc.Sentence2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeTrue(), "Return value")
            .And((tc, r) => r.unifier.Should().Equal(tc.ExpectedUnifier), "Unifier");

        public static Test TryUnifyNegative => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(x, jane)),
            })
            .When(tc =>
            {
                (bool returnValue, IDictionary<Variable, Term> unifier) result;
                result.returnValue = new SentenceUnifier().TryUnify(tc.Sentence1, tc.Sentence2, out result.unifier);
                return result;
            })
            .ThenReturns((tc, r) => r.returnValue.Should().BeFalse(), "Return value")
            .And((tc, r) => r.unifier.Should().BeNull(), "Unifier");
    }
}
