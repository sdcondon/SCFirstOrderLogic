using FluentAssertions;
using FlUnit;
using System;
using static SCFirstOrderLogic.SentenceManipulation.OperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation
{
    public class OperableSentenceFactoryTests
    {
        private record TestCase(OperableSentence SentenceSurrogate, Sentence ExpectedSentence);

        private static OperableConstant Constant1 => new Constant(nameof(Constant1));
        private static OperableConstant Constant2 => new Constant(nameof(Constant2));
        private static OperablePredicate GroundPredicate1 => new Predicate(nameof(GroundPredicate1));
        private static OperablePredicate GroundPredicate2 => new Predicate(nameof(GroundPredicate2));
        private static OperablePredicate UnaryPredicate(OperableTerm term) => new Predicate(nameof(UnaryPredicate), term);
        private static OperableFunction UnaryFunction(OperableTerm term) => new Function(nameof(UnaryFunction), term);

        public static Test CreationAndCasting => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    SentenceSurrogate: GroundPredicate1 & UnaryPredicate(Constant1),
                    ExpectedSentence: new Conjunction(
                        new Predicate(nameof(GroundPredicate1), Array.Empty<Term>()),
                        new Predicate(nameof(UnaryPredicate), new Constant(nameof(Constant1))))),

                new TestCase(
                    SentenceSurrogate: GroundPredicate1 | GroundPredicate2,
                    ExpectedSentence: new Disjunction(
                        new Predicate(nameof(GroundPredicate1), Array.Empty<Term>()),
                        new Predicate(nameof(GroundPredicate2), Array.Empty<Term>()))),

                new TestCase(
                    SentenceSurrogate: Constant1 == Constant2,
                    ExpectedSentence: new Predicate(
                        EqualitySymbol.Instance,
                        new Constant(nameof(Constant1)),
                        new Constant(nameof(Constant2)))),

                new TestCase(
                    SentenceSurrogate: Iff(GroundPredicate1, GroundPredicate2),
                    ExpectedSentence: new Equivalence(
                        new Predicate(nameof(GroundPredicate1), Array.Empty<Term>()),
                        new Predicate(nameof(GroundPredicate2), Array.Empty<Term>()))),

                new TestCase(
                    SentenceSurrogate: ThereExists(X, UnaryFunction(X) == Constant1),
                    ExpectedSentence: new ExistentialQuantification(
                        new VariableDeclaration("X"),
                        new Predicate(
                            EqualitySymbol.Instance,
                            new Function(nameof(UnaryFunction), new[] { new VariableReference(new VariableDeclaration("X")) }),
                            new Constant(nameof(Constant1))))),

                new TestCase(
                    SentenceSurrogate: If(GroundPredicate1, GroundPredicate2),
                    ExpectedSentence: new Implication(
                        new Predicate(nameof(GroundPredicate1), Array.Empty<Term>()),
                        new Predicate(nameof(GroundPredicate2), Array.Empty<Term>()))),

                new TestCase(
                    SentenceSurrogate: !GroundPredicate1,
                    ExpectedSentence: new Negation(
                        new Predicate(nameof(GroundPredicate1), Array.Empty<Term>()))),

                new TestCase(
                    SentenceSurrogate: ForAll(X, UnaryFunction(X) == Constant1),
                    ExpectedSentence: new UniversalQuantification(
                        new VariableDeclaration("X"),
                        new Predicate(
                            EqualitySymbol.Instance,
                            new Function(nameof(UnaryFunction), new[] { new VariableReference(new VariableDeclaration("X")) }),
                            new Constant(nameof(Constant1))))),
            })
            .When(tc => (Sentence)tc.SentenceSurrogate)
            .ThenReturns((tc, sentence) =>
            {
                sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
            });
    }
}
