using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    public class UnifierTests
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

        private static Function<IPeople, IPerson> Mother(Term<IPeople, IPerson> child)
        {
            return new Function<IPeople, IPerson>(typeof(IPerson).GetProperty(nameof(IPerson.Mother)), new[] { child });
        }

        private static Predicate<IPeople, IPerson> Knows(Term<IPeople, IPerson> knower, Term<IPeople, IPerson> known)
        {
            return new Predicate<IPeople, IPerson>(typeof(IPerson).GetMethod(nameof(IPerson.Knows)), new Term<IPeople, IPerson>[] { knower, known });
        }

        private static readonly Constant<IPeople, IPerson> john = new Constant<IPeople, IPerson>(typeof(IPeople).GetProperty(nameof(IPeople.John)));
        private static readonly Constant<IPeople, IPerson> jane = new Constant<IPeople, IPerson>(typeof(IPeople).GetProperty(nameof(IPeople.Jane)));
        private static readonly Variable<IPeople, IPerson> x = new Variable<IPeople, IPerson>("x");
        private static readonly Variable<IPeople, IPerson> y = new Variable<IPeople, IPerson>("y");

        private record TestCase(Sentence<IPeople, IPerson> Sentence1, Sentence<IPeople, IPerson> Sentence2, Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>> ExpectedUnifier);

        public static Test TryUnify => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(john, jane),
                    ExpectedUnifier: new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
                    {
                        [x] = jane,
                    }),

                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, jane),
                    ExpectedUnifier: new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
                    {
                        [x] = jane,
                        [y] = john,
                    }),

                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(y, Mother(y)),
                    ExpectedUnifier: new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
                    {
                        [x] = Mother(john),
                        [y] = john,
                    }),

                new TestCase(
                    Sentence1: Knows(john, x),
                    Sentence2: Knows(x, jane),
                    ExpectedUnifier: null),
            })
            .When(tc =>
            {
                (bool returnValue, IDictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>> unifier) result;
                result.returnValue = new SentenceUnifier<IPeople, IPerson>().TryUnify(tc.Sentence1, tc.Sentence2, out result.unifier);
                return result;
            })
            .Then((tc, r) => r.returnValue.Should().Be(tc.ExpectedUnifier != null ? true : false), "Return value")
            .And((tc, r) => r.unifier.Should().BeEquivalentTo(tc.ExpectedUnifier), "Unifier");
    }
}
