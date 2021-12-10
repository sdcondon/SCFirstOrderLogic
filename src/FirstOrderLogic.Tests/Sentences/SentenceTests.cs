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
        private static readonly IList<Term> emptyArgList = Array.Empty<Term>();

        private record TestCase(Expression<Predicate<IDomain>> Expression, Sentence ExpectedSentence);

        public static Test Creation => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Expression: d => d.GroundPredicate1 && d.GroundPredicate2,
                    ExpectedSentence: new Conjunction(
                        new MemberPredicate(groundPredicate1, emptyArgList),
                        new MemberPredicate(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.GroundPredicate1 || d.GroundPredicate2,
                    ExpectedSentence: new Disjunction(
                        new MemberPredicate(groundPredicate1, emptyArgList),
                        new MemberPredicate(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.Constant1.Parent == d.Constant1,
                    ExpectedSentence: new Equality(
                        new MemberFunction(parent, new[] { new MemberConstant(constant1) }),
                        new MemberConstant(constant1))),

                new TestCase(
                    Expression: d => Iff(d.GroundPredicate1, d.GroundPredicate2),
                    ExpectedSentence: new Equivalence(
                        new MemberPredicate(groundPredicate1, emptyArgList),
                        new MemberPredicate(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => d.Any(x => x.Parent == d.Constant1),
                    ExpectedSentence: new ExistentialQuantification(
                        new VariableDeclaration("x"),
                        new Equality(
                            new MemberFunction(parent, new[] { new Variable(new VariableDeclaration("x")) }),
                            new MemberConstant(constant1)))),

                new TestCase(
                    Expression: d => If(d.GroundPredicate1, d.GroundPredicate2),
                    ExpectedSentence: new Implication(
                        new MemberPredicate(groundPredicate1, emptyArgList),
                        new MemberPredicate(groundPredicate2, emptyArgList))),

                new TestCase(
                    Expression: d => !d.GroundPredicate1,
                    ExpectedSentence: new Negation(
                        new MemberPredicate(groundPredicate1, emptyArgList))),

                new TestCase(
                    Expression: d => d.All(x => x.Parent == d.Constant1),
                    ExpectedSentence: new UniversalQuantification(
                        new VariableDeclaration("x"),
                        new Equality(
                            new MemberFunction(parent, new[] { new Variable(new VariableDeclaration("x")) }),
                            new MemberConstant(constant1)))),
            })
            .When(tc => Sentence.Create<IDomain, IElement>(tc.Expression))
            .ThenReturns((tc, sentence) =>
            {
                sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
            });
    }
}
