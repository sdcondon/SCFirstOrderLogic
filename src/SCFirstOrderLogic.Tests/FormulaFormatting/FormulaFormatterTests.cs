using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.FormulaCreation;

namespace SCFirstOrderLogic.FormulaFormatting;

public static class FormulaFormatterTests
{
    public static Test FormatBehaviour => TestThat
        .GivenEachOf<FormatTestCase>(() =>
        [
            new(Formula: "P(C())",            Expected: "P(C())"),
            new(Formula: "P() & Q() & R()",   Expected: "P() ∧ Q() ∧ R()"),
            new(Formula: "[P() | Q()] & R()", Expected: "[P() ∨ Q()] ∧ R()"),
            new(Formula: "P() | [Q() & R()]", Expected: "P() ∨ [Q() ∧ R()]"),
            new(Formula: "![P() | Q()]",      Expected: "¬[P() ∨ Q()]"),
            new(Formula: "![P()]",            Expected: "¬P()"),

            //∃∀
            new(Formula: "∃ x, P(x)",        Expected: "∃ x, P(x)"),
            new(Formula: "∃ x, P(x) | Q(x)", Expected: "∃ x, P(x) ∨ Q(x)"),
            new(Formula: "∃ x, P(x) => Q(x)", Expected: "∃ x, P(x) ⇒ Q(x)"),

            new(Formula: "∃ x, ∀ y, P(x, y)",        Expected: "∃ x, ∀ y, P(x, y)"),

            new(Formula: "∀ x, P(x) => Q(x) => R(x)", Expected: "∀ x, [P(x) ⇒ Q(x)] ⇒ R(x)"),

            new(Formula: "∀ x, ∀ y, P(x, y)", Expected: "∀ x, y, P(x, y)"),
            new(Formula: "∃ x, ∃ y, P(x, y)", Expected: "∃ x, y, P(x, y)"),
        ])
        .When(tc => new FormulaFormatter().Format(FormulaParser.Default.Parse(tc.Formula)))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    private record FormatTestCase(string Formula, string Expected);
}
