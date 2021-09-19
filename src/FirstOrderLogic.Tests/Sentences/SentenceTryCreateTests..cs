using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    public partial class SentenceTryCreateTests
    {
        private static readonly MemberInfo groundPredicate1 = typeof(ISimpleDomain).GetProperty(nameof(ISimpleDomain.GroundPredicate1));
        private static readonly MemberInfo groundPredicate2 = typeof(ISimpleDomain).GetProperty(nameof(ISimpleDomain.GroundPredicate2));
        private static readonly MemberInfo constant1 = typeof(ISimpleDomain).GetProperty(nameof(ISimpleDomain.Constant1));
        private static readonly MemberInfo parent = typeof(ISimpleElement).GetProperty(nameof(ISimpleElement.Parent));
        private static readonly IList<Term<ISimpleDomain, ISimpleElement>> emptyArgList = Array.Empty<Term<ISimpleDomain, ISimpleElement>>();

        [Fact]
        public void Conjunction_AlsoPredicate()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => d.GroundPredicate1 && d.GroundPredicate2, out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Conjunction<ISimpleDomain, ISimpleElement>(
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate1, emptyArgList),
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate2, emptyArgList)));
        }

        [Fact]
        public void Disjunction_AlsoPredicate()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => d.GroundPredicate1 || d.GroundPredicate2, out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Disjunction<ISimpleDomain, ISimpleElement>(
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate1, emptyArgList),
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate2, emptyArgList)));
        }

        [Fact]
        public void Equality_AlsoFunctionAndConstant()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => d.Constant1.Parent == d.Constant1, out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Equality<ISimpleDomain, ISimpleElement>(
                new Function<ISimpleDomain, ISimpleElement>(parent, new[] { new Constant<ISimpleDomain, ISimpleElement>(constant1) }),
                new Constant<ISimpleDomain, ISimpleElement>(constant1)));
        }

        [Fact]
        public void Equivalence_AlsoPredicate()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => Iff(d.GroundPredicate1, d.GroundPredicate2), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Equivalence<ISimpleDomain, ISimpleElement>(
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate1, emptyArgList),
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate2, emptyArgList)));
        }

        [Fact]
        public void ExistentialQuantification_AlsoEqualityFunctionVariableAndConstant()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => d.Any(x => x.Parent == d.Constant1), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new ExistentialQuantification<ISimpleDomain, ISimpleElement>(
                new Variable<ISimpleDomain, ISimpleElement>("x"),
                new Equality<ISimpleDomain, ISimpleElement>(
                    new Function<ISimpleDomain, ISimpleElement>(parent, new[] { new Variable<ISimpleDomain, ISimpleElement>("x") }),
                    new Constant<ISimpleDomain, ISimpleElement>(constant1))));
        }

        [Fact]
        public void Implication_AlsoPredicate()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => If(d.GroundPredicate1, d.GroundPredicate2), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Implication<ISimpleDomain, ISimpleElement>(
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate1, emptyArgList),
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate2, emptyArgList)));
        }

        [Fact]
        public void Negation_AlsoPredicate()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => !d.GroundPredicate1, out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new Negation<ISimpleDomain, ISimpleElement>(
                new Predicate<ISimpleDomain, ISimpleElement>(groundPredicate1, emptyArgList)));
        }

        [Fact]
        public void UniversalQuantification_AlsoEqualityFunctionVariableAndConstant()
        {
            Sentence.TryCreate<ISimpleDomain, ISimpleElement>(d => d.All(x => x.Parent == d.Constant1), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new UniversalQuantification<ISimpleDomain, ISimpleElement>(
                new Variable<ISimpleDomain, ISimpleElement>("x"),
                new Equality<ISimpleDomain, ISimpleElement>(
                    new Function<ISimpleDomain, ISimpleElement>(parent, new[] { new Variable<ISimpleDomain, ISimpleElement>("x") }),
                    new Constant<ISimpleDomain, ISimpleElement>(constant1))));
        }

        private interface ISimpleDomain : IEnumerable<ISimpleElement>
        {
            bool GroundPredicate1 { get; }

            bool GroundPredicate2 { get; }

            ISimpleElement Constant1 { get; }
        }

        private interface ISimpleElement
        {
            ISimpleElement Parent { get; }

            bool Predicate { get; }
        }
    }
}
