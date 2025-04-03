using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceCreation;

namespace SCFirstOrderLogic.SentenceFormatting;

public static class SentenceFormatterTests
{
    public static Test FormatBehaviour => TestThat
        .GivenEachOf<FormatTestCase>(() =>
        [
            new(Sentence: "P(C())",            Expected: "P(C())"),
            new(Sentence: "P() & Q() & R()",   Expected: "P() ∧ Q() ∧ R()"),
            new(Sentence: "[P() | Q()] & R()", Expected: "[P() ∨ Q()] ∧ R()"),
            new(Sentence: "P() | [Q() & R()]", Expected: "P() ∨ [Q() ∧ R()]"),
            new(Sentence: "![P() | Q()]",      Expected: "¬[P() ∨ Q()]"),
            new(Sentence: "![P()]",            Expected: "¬P()"),

            //∃∀
            new(Sentence: "∃ x, P(x)",        Expected: "∃ x, P(x)"),
            new(Sentence: "∃ x, P(x) | Q(x)", Expected: "∃ x, P(x) ∨ Q(x)"),
            new(Sentence: "∃ x, P(x) => Q(x)", Expected: "∃ x, P(x) ⇒ Q(x)"),

            new(Sentence: "∃ x, ∀ y, P(x, y)",        Expected: "∃ x, ∀ y, P(x, y)"),

            new(Sentence: "∀ x, P(x) => Q(x) => R(x)", Expected: "∀ x, [P(x) ⇒ Q(x)] ⇒ R(x)"),
        ])
        .When(tc => new SentenceFormatter().Format(SentenceParser.BasicParser.Parse(tc.Sentence)))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    private record FormatTestCase(string Sentence, string Expected);
}
