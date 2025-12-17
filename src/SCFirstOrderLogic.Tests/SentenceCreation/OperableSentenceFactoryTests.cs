using FluentAssertions;
using FlUnit;
using System;
using static SCFirstOrderLogic.FormulaCreation.OperableFormulaFactory;

namespace SCFirstOrderLogic.FormulaCreation;

public class OperableSentenceFactoryTests
{
    private record TestCase(OperableFormula SentenceSurrogate, Formula ExpectedSentence);

    private static OperableFunction Constant1 => new Function(nameof(Constant1));
    private static OperableFunction Constant2 => new Function(nameof(Constant2));
    private static OperablePredicate GroundPredicate1 => new Predicate(nameof(GroundPredicate1));
    private static OperablePredicate GroundPredicate2 => new Predicate(nameof(GroundPredicate2));
    private static OperablePredicate UnaryPredicate(OperableTerm term) => new Predicate(nameof(UnaryPredicate), term);
    private static OperableFunction UnaryFunction(OperableTerm term) => new Function(nameof(UnaryFunction), term);

    public static Test CreationAndCasting => TestThat
        .GivenEachOf<TestCase>(() =>
        [
            new(
                SentenceSurrogate: GroundPredicate1 & UnaryPredicate(Constant1),
                ExpectedSentence: new Conjunction(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(UnaryPredicate), new Function(nameof(Constant1))))),

            new(
                SentenceSurrogate: GroundPredicate1 | GroundPredicate2,
                ExpectedSentence: new Disjunction(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                SentenceSurrogate: Constant1 == Constant2,
                ExpectedSentence: new Predicate(
                    EqualityIdentifier.Instance,
                    new Function(nameof(Constant1)),
                    new Function(nameof(Constant2)))),

            new(
                SentenceSurrogate: Iff(GroundPredicate1, GroundPredicate2),
                ExpectedSentence: new Equivalence(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                SentenceSurrogate: ThereExists(X, UnaryFunction(X) == Constant1),
                ExpectedSentence: new ExistentialQuantification(
                    new VariableDeclaration("X"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(nameof(UnaryFunction), [ new VariableReference(new VariableDeclaration("X")) ]),
                        new Function(nameof(Constant1))))),

            new(
                SentenceSurrogate: If(GroundPredicate1, GroundPredicate2),
                ExpectedSentence: new Implication(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                SentenceSurrogate: !GroundPredicate1,
                ExpectedSentence: new Negation(
                    new Predicate(nameof(GroundPredicate1), []))),

            new(
                SentenceSurrogate: ForAll(X, UnaryFunction(X) == Constant1),
                ExpectedSentence: new UniversalQuantification(
                    new VariableDeclaration("X"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(nameof(UnaryFunction), [ new VariableReference(new VariableDeclaration("X")) ]),
                        new Function(nameof(Constant1))))),
        ])
        .When(tc => (Formula)tc.SentenceSurrogate)
        .ThenReturns((tc, sentence) =>
        {
            sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
        });
}
