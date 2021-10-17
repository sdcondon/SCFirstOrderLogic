using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    public class UnifierTests
    {
        private static readonly Variable<IPeople, IPerson> x = new Variable<IPeople, IPerson>("x");
        private static readonly Variable<IPeople, IPerson> y = new Variable<IPeople, IPerson>("y");
        private static readonly Constant<IPeople, IPerson> john = new Constant<IPeople, IPerson>(typeof(IPeople).GetProperty(nameof(IPeople.John)));
        private static readonly Constant<IPeople, IPerson> jane = new Constant<IPeople, IPerson>(typeof(IPeople).GetProperty(nameof(IPeople.Jane)));

        private static Function<IPeople, IPerson> Mother(Term<IPeople, IPerson> child)
        {
            return new Function<IPeople, IPerson>(typeof(IPerson).GetProperty(nameof(IPerson.Mother)), new[] { child });
        }

        private static Predicate<IPeople, IPerson> Knows(Term<IPeople, IPerson> knower, Term<IPeople, IPerson> known)
        {
            return new Predicate<IPeople, IPerson>(typeof(IPerson).GetMethod(nameof(IPerson.Knows)), new Term<IPeople, IPerson>[] { knower, known });
        }

        [Fact]
        public void JohnKnowsX_And_JohnKnowsJane()
        {
            new SentenceUnifier<IPeople, IPerson>()
                .TryUnify(Knows(john, x), Knows(john, jane), out var unifier)
                .Should()
                .BeTrue();

            unifier.Should().BeEquivalentTo(new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
            {
                [x] = jane,
            });
        }

        [Fact]
        public void JohnKnowsX_And_YKnowsJane()
        {
            new SentenceUnifier<IPeople, IPerson>()
                .TryUnify(Knows(john, x), Knows(y, jane), out var unifier)
                .Should()
                .BeTrue();

            unifier.Should().BeEquivalentTo(new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
            {
                [x] = jane,
                [y] = john,
            });
        }

        [Fact]
        public void JohnKnowsX_And_YKnowsMotherOfY()
        {
            new SentenceUnifier<IPeople, IPerson>()
                .TryUnify(Knows(john, x), Knows(y, Mother(y)), out var unifier)
                .Should()
                .BeTrue();

            unifier.Should().BeEquivalentTo(new Dictionary<Variable<IPeople, IPerson>, Term<IPeople, IPerson>>()
            {
                [x] = Mother(john),
                [y] = john,
            });
        }

        [Fact]
        public void JohnKnowsX_And_XKnowsJane()
        {
            new SentenceUnifier<IPeople, IPerson>()
                .TryUnify(Knows(john, x), Knows(x, jane), out var _)
                .Should()
                .BeFalse();
        }

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
    }
}
