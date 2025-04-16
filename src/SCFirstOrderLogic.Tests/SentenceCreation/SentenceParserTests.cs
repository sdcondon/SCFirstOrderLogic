using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic.SentenceCreation;

public static class SentenceParserTests
{
    public static Test Parse_PositiveTestCases => TestThat
        .GivenEachOf<ParseTestCase>(() =>
        [
            new(
                Sentence: "Aa_1()",
                ExpectedResult: new Predicate("Aa_1")),

            new(
                Sentence: " P () ",
                ExpectedResult: new Predicate("P")),

            new(
                Sentence: "∀x, P(x)",
                ExpectedResult: new UniversalQuantification(new("x"), new Predicate("P", new VariableReference("x")))),

            new(
                Sentence: "∀x, P(F(x))",
                ExpectedResult: new UniversalQuantification(new("x"), new Predicate("P", new Function("F", new VariableReference("x"))))),

            new(
                Sentence: "∃x, y, P(x, y)",
                ExpectedResult: new ExistentialQuantification(new("x"), new ExistentialQuantification(new("y"), new Predicate("P", new VariableReference("x"), new VariableReference("y"))))),

            new(
                Sentence: "∃x y, P(x, y)",
                ExpectedResult: new ExistentialQuantification(new("x"), new ExistentialQuantification(new("y"), new Predicate("P", new VariableReference("x"), new VariableReference("y"))))),

            new(
                Sentence: "P(x) ∧ Q(y)",
                ExpectedResult: new Conjunction(new Predicate("P", new Function("x")), new Predicate("Q", new Function("y")))),

            new(
                Sentence: "P(x) ∨ ¬Q(y)",
                ExpectedResult: new Disjunction(new Predicate("P", new Function("x")), new Negation(new Predicate("Q", new Function("y"))))),

            new(
                Sentence: "P() ∨ ¬[Q() ∧ R()]",
                ExpectedResult: new Disjunction(new Predicate("P"), new Negation(new Conjunction(new Predicate("Q"), new Predicate("R"))))),

            new(
                Sentence: "P(x) ⇒ Q(y)",
                ExpectedResult: new Implication(new Predicate("P", new Function("x")), new Predicate("Q", new Function("y")))),

            new(
                Sentence: "P(x) ⇔ Q(y)",
                ExpectedResult: new Equivalence(new Predicate("P", new Function("x")), new Predicate("Q", new Function("y")))),

            new(
                Sentence: "F1() = F2(x, y)",
                ExpectedResult: new Predicate(EqualityIdentifier.Instance, new Function("F1"), new Function("F2", new Function("x"), new Function("y")))),
        ])
        .When(tc => SentenceParser.Default.Parse(tc.Sentence))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedResult));

    public static Test Parse_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf(() => new[]
        {
            string.Empty,
            "couldBeATermButNotASentence",
            "P(x,y,)",
            "P(,x,y)",
            "∀ P(x)",
            "∃ P(x)",
            "P()aaa",
        })
        .When((ctx, tc) => SentenceParser.Default.Parse(tc))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.ToString()));

    public static Test Parse_WithCustomIdentifiers => TestThat
        .Given(() => new SentenceParser(new(s => $"p.{s}", s => $"f.{s}", s => $"vc.{s}")))
        .When(p => p.Parse("forall x, P(F(x, C))"))
        .ThenReturns()
        .And((_, rv) => rv.Should().Be(new UniversalQuantification(new("vc.x"), new Predicate("p.P", new Function("f.F", new VariableReference("vc.x"), new Function("vc.C"))))));

    public static Test ParseList_PositiveTestCases => TestThat
        .GivenEachOf<ParseListTestCase>(() =>
        [
            new(
                Sentences: string.Empty,
                Expectation: []),

            new(
                Sentences: "   ",
                Expectation: []),

            new(
                Sentences: "P()",
                Expectation: [new Predicate("P")]),

            new(
                Sentences: "P()Q()",
                Expectation: [new Predicate("P"), new Predicate("Q")]),

            new(
                Sentences: "P() Q()",
                Expectation: [new Predicate("P"), new Predicate("Q")]),

            new(
                Sentences: "P()\r\nQ()\n",
                Expectation: [new Predicate("P"), new Predicate("Q")]),

            new(
                Sentences: "P(); Q()",
                Expectation: [new Predicate("P"), new Predicate("Q")]),

            new(
                Sentences: " P() ; Q() ; ",
                Expectation: [new Predicate("P"), new Predicate("Q")]),
        ])
        .When(tc => SentenceParser.Default.ParseList(tc.Sentences))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Equal(tc.Expectation));

    public static Test ParseList_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf<string>(() =>
        [
            "P() Q()aaa",
            "P() Q();aaa",
            "P(); ; Q()",
            $"P()\r\nQ()aaa",
            $"P()\nQ()aaa",
        ])
        .When((ctx, tc) => SentenceParser.Default.ParseList(tc))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.ToString()));

    private record ParseTestCase(string Sentence, Sentence ExpectedResult);

    private record ParseListTestCase(string Sentences, Sentence[] Expectation);
}