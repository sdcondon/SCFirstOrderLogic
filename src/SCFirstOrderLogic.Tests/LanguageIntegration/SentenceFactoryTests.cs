using FluentAssertions;
using FlUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.LanguageIntegration;

public class SentenceFactoryTests
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

    private static readonly MemberInfo groundPredicate1 = typeof(IDomain).GetProperty(nameof(IDomain.GroundPredicate1))!;
    private static readonly MemberInfo groundPredicate2 = typeof(IDomain).GetProperty(nameof(IDomain.GroundPredicate2))!;
    private static readonly MemberInfo constant1 = typeof(IDomain).GetProperty(nameof(IDomain.Constant1))!;
    private static readonly MemberInfo parent = typeof(IElement).GetProperty(nameof(IElement.Parent))!;
    private static readonly IList<Term> emptyArgList = Array.Empty<Term>();

    private record TestCase(Expression<Predicate<IDomain>> Expression, Sentence ExpectedSentence);

    public static Test Creation => TestThat
        .GivenEachOf(() => new[]
        {
            new TestCase(
                Expression: d => d.GroundPredicate1 && d.GroundPredicate2,
                ExpectedSentence: new Conjunction(
                    new Predicate(new MemberPredicateIdentifier(groundPredicate1), emptyArgList),
                    new Predicate(new MemberPredicateIdentifier(groundPredicate2), emptyArgList))),

            new TestCase(
                Expression: d => d.GroundPredicate1 || d.GroundPredicate2,
                ExpectedSentence: new Disjunction(
                    new Predicate(new MemberPredicateIdentifier(groundPredicate1), emptyArgList),
                    new Predicate(new MemberPredicateIdentifier(groundPredicate2), emptyArgList))),

            new TestCase(
                Expression: d => d.Constant1.Parent == d.Constant1,
                ExpectedSentence: new Predicate(
                    EqualityIdentifier.Instance,
                    new Function(new MemberFunctionIdentifier(parent), new[] { new Function(new MemberConstantIdentifier(constant1)) }),
                    new Function(new MemberConstantIdentifier(constant1)))),

            new TestCase(
                Expression: d => Iff(d.GroundPredicate1, d.GroundPredicate2),
                ExpectedSentence: new Equivalence(
                    new Predicate(new MemberPredicateIdentifier(groundPredicate1), emptyArgList),
                    new Predicate(new MemberPredicateIdentifier(groundPredicate2), emptyArgList))),

            new TestCase(
                Expression: d => d.Any(x => x.Parent == d.Constant1),
                ExpectedSentence: new ExistentialQuantification(
                    new VariableDeclaration("x"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(new MemberFunctionIdentifier(parent), new[] { new VariableReference(new VariableDeclaration("x")) }),
                        new Function(new MemberConstantIdentifier(constant1))))),

            new TestCase(
                Expression: d => If(d.GroundPredicate1, d.GroundPredicate2),
                ExpectedSentence: new Implication(
                    new Predicate(new MemberPredicateIdentifier(groundPredicate1), emptyArgList),
                    new Predicate(new MemberPredicateIdentifier(groundPredicate2), emptyArgList))),

            new TestCase(
                Expression: d => !d.GroundPredicate1,
                ExpectedSentence: new Negation(
                    new Predicate(new MemberPredicateIdentifier(groundPredicate1), emptyArgList))),

            new TestCase(
                Expression: d => d.All(x => x.Parent == d.Constant1),
                ExpectedSentence: new UniversalQuantification(
                    new VariableDeclaration("x"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(new MemberFunctionIdentifier(parent), new[] { new VariableReference(new VariableDeclaration("x")) }),
                        new Function(new MemberConstantIdentifier(constant1))))),
        })
        .When(tc => SentenceFactory.Create<IDomain, IElement>(tc.Expression))
        .ThenReturns((tc, sentence) =>
        {
            sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
        });
}
