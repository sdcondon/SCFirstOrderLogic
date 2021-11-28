using FluentAssertions;
using FlUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    public class SentenceTests
    {
        private interface IDomain : IEnumerable<IElement>
        {
            bool GroundPredicate1 { get; }
            bool GroundPredicate2 { get; }
            IElement Constant1 { get; }
        }

        private interface IElement
        {
            IElement Parent { get; }
        }

        private static readonly MemberInfo groundPredicate1 = typeof(IDomain).GetProperty(nameof(IDomain.GroundPredicate1));
        private static readonly MemberInfo groundPredicate2 = typeof(IDomain).GetProperty(nameof(IDomain.GroundPredicate2));
        private static readonly MemberInfo constant1 = typeof(IDomain).GetProperty(nameof(IDomain.Constant1));
        private static readonly MemberInfo parent = typeof(IElement).GetProperty(nameof(IElement.Parent));
        private static readonly IList<Term<IDomain, IElement>> emptyArgList = Array.Empty<Term<IDomain, IElement>>();

        private record TestCase(Expression<Predicate<IDomain>> Expression, Sentence<IDomain, IElement> ExpectedSentence);

        public static Test Creation => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Expression: d => d.GroundPredicate1 && d.GroundPredicate2,
                    ExpectedSentence: new Conjunction<IDomain, IElement>(
                        new Predicate<IDomain, IElement>(groundPredicate1, emptyArgList),
                        new Predicate<IDomain, IElement>(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.GroundPredicate1 || d.GroundPredicate2,
                    ExpectedSentence: new Disjunction<IDomain, IElement>(
                        new Predicate<IDomain, IElement>(groundPredicate1, emptyArgList),
                        new Predicate<IDomain, IElement>(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.Constant1.Parent == d.Constant1,
                    ExpectedSentence: new Equality<IDomain, IElement>(
                        new DomainFunction<IDomain, IElement>(parent, new[] { new Constant<IDomain, IElement>(constant1) }),
                        new Constant<IDomain, IElement>(constant1))),

                new TestCase(
                    Expression: d => Iff(d.GroundPredicate1, d.GroundPredicate2),
                    ExpectedSentence: new Equivalence<IDomain, IElement>(
                        new Predicate<IDomain, IElement>(groundPredicate1, emptyArgList),
                        new Predicate<IDomain, IElement>(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.Any(x => x.Parent == d.Constant1),
                    ExpectedSentence: new ExistentialQuantification<IDomain, IElement>(
                        new VariableDeclaration<IDomain, IElement>("x"),
                        new Equality<IDomain, IElement>(
                            new DomainFunction<IDomain, IElement>(parent, new[] { new Variable<IDomain, IElement>(new VariableDeclaration<IDomain, IElement>("x")) }),
                            new Constant<IDomain, IElement>(constant1)))),

                new TestCase(
                    Expression: d => If(d.GroundPredicate1, d.GroundPredicate2),
                    ExpectedSentence: new Implication<IDomain, IElement>(
                        new Predicate<IDomain, IElement>(groundPredicate1, emptyArgList),
                        new Predicate<IDomain, IElement>(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => !d.GroundPredicate1,
                    ExpectedSentence: new Negation<IDomain, IElement>(
                        new Predicate<IDomain, IElement>(groundPredicate1, emptyArgList))),

                new TestCase(
                    Expression: d => d.All(x => x.Parent == d.Constant1),
                    ExpectedSentence: new UniversalQuantification<IDomain, IElement>(
                        new VariableDeclaration<IDomain, IElement>("x"),
                        new Equality<IDomain, IElement>(
                            new DomainFunction<IDomain, IElement>(parent, new[] { new Variable<IDomain, IElement>(new VariableDeclaration<IDomain, IElement>("x")) }),
                            new Constant<IDomain, IElement>(constant1)))),
            })
            .When(tc => Sentence.Create<IDomain, IElement>(tc.Expression))
            .ThenReturns((tc, sentence) =>
            {
                sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
            });
    }
}
