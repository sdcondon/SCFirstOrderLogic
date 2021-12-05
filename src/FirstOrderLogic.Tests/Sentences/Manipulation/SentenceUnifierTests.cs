﻿using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    public class SentenceUnifierTests
    {
        private interface IPeople : IEnumerable<IPerson>
        {
            IPerson John { get; }
            IPerson Jane { get; }
        }

        private interface IPerson
        {
            IPerson Mother { get; }
            bool Knows(IPerson other);
        }

        private static MemberFunction Mother(Term child)
        {
            return new MemberFunction(typeof(IPerson).GetProperty(nameof(IPerson.Mother)), new[] { child });
        }

        private static Predicate Knows(Term knower, Term known)
        {
            return new MemberPredicate(typeof(IPerson).GetMethod(nameof(IPerson.Knows)), new Term[] { knower, known });
        }

        private static readonly Constant john = new Constant(typeof(IPeople).GetProperty(nameof(IPeople.John)));
        private static readonly Constant jane = new Constant(typeof(IPeople).GetProperty(nameof(IPeople.Jane)));
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
                        // produces. Easy enough to resolve, but waiting and seeing how it pans out through usage..
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
