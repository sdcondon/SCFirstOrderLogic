using FluentAssertions;
using FlUnit;
using System;

namespace SCFirstOrderLogic.SentenceCreation;

public static class SentenceParserTests
{
    public static Test Parse_PositiveTestCases => TestThat
        .GivenEachOf(() => new ParseTestCase[]
        {
            new(
                Sentence: "P()",
                ExpectedResult: new Predicate("P")),

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
                ExpectedResult: new Conjunction(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

            new(
                Sentence: "P(x) ∨ ¬Q(y)",
                ExpectedResult: new Disjunction(new Predicate("P", new Constant("x")), new Negation(new Predicate("Q", new Constant("y"))))),

            new(
                Sentence: "P() ∨ ¬[Q() ∧ R()]",
                ExpectedResult: new Disjunction(new Predicate("P"), new Negation(new Conjunction(new Predicate("Q"), new Predicate("R"))))),

            new(
                Sentence: "P(x) ⇒ Q(y)",
                ExpectedResult: new Implication(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

            new(
                Sentence: "P(x) ⇔ Q(y)",
                ExpectedResult: new Equivalence(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

            new(
                Sentence: "F1() = F2(x, y)",
                ExpectedResult: new Predicate(EqualityIdentifier.Instance, new Function("F1"), new Function("F2", new Constant("x"), new Constant("y")))),
        })
        .When(tc => SentenceParser.BasicParser.Parse(tc.Sentence))
        .ThenReturns()
        .And((ParseTestCase tc, Sentence rv) => rv.Should().Be(tc.ExpectedResult));

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
        .When((ctx, tc) => SentenceParser.BasicParser.Parse(tc))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    public static Test Parse_WithCustomIdentifiers => TestThat
        .Given(() => new SentenceParser(s => $"p.{s}", s => $"f.{s}", s => $"vc.{s}"))
        .When(p => p.Parse("forall x, P(F(x, C))"))
        .ThenReturns()
        .And((_, rv) => rv.Should().Be(new UniversalQuantification(new("vc.x"), new Predicate("p.P", new Function("f.F", new VariableReference("vc.x"), new Constant("vc.C"))))));

    public static Test ParseList_PositiveTestCases => TestThat
        .GivenEachOf(() => new ParseListTestCase[]
        {
            new(
                Sentences: string.Empty,
                Expectation: Array.Empty<Sentence>()),

            new(
                Sentences: "   ",
                Expectation: Array.Empty<Sentence>()),

            new(
                Sentences: "P()",
                Expectation: new[] { new Predicate("P") }),

            new(
                Sentences: "P()Q()",
                Expectation: new[] { new Predicate("P"), new Predicate("Q") }),

            new(
                Sentences: "P() Q()",
                Expectation: new[] { new Predicate("P"), new Predicate("Q") }),

            new(
                Sentences: "P()\r\nQ()\n",
                Expectation: new[] { new Predicate("P"), new Predicate("Q") }),

            new(
                Sentences: "P(); Q()",
                Expectation: new[] { new Predicate("P"), new Predicate("Q") }),

            new(
                Sentences: " P() ; Q() ; ",
                Expectation: new[] { new Predicate("P"), new Predicate("Q") }),
        })
        .When(tc => SentenceParser.BasicParser.ParseList(tc.Sentences))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Equal(tc.Expectation));

    public static Test ParseList_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf(() => new[]
        {
            "P() Q()aaa",
            "P() Q();aaa",
            "P(); ; Q()",
        })
        .When((ctx, tc) => SentenceParser.BasicParser.ParseList(tc))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    private record ParseTestCase(string Sentence, Sentence ExpectedResult);

    private record ParseListTestCase(string Sentences, Sentence[] Expectation);
}