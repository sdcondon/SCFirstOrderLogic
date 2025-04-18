﻿using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

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
            "A+()",
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

    public static Test ParseTerm_PositiveTestCases => TestThat
        .GivenEachOf<ParseTermTestCase>(() =>
        [
            new(
                Text: "F()",
                Variables: [new("F")],
                Expected: new Function("F")),

            new(
                Text: " F () ",
                Variables: [new("F")],
                Expected: new Function("F")),

            new(
                Text: "F",
                Variables: [new("F")],
                Expected: new VariableReference("F")),

            new(
                Text: "F(G(), X)",
                Variables: [new("X")],
                Expected: new Function("F", new Function("G"), new VariableReference("X"))),
        ])
        .When(tc => SentenceParser.Default.ParseTerm(tc.Text, tc.Variables))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test ParseTerm_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf(() => new[]
        {
            string.Empty,
            "F(x,y,)",
            "F(,x,y)",
            "F()aaa",
        })
        .When((ctx, tc) => SentenceParser.Default.ParseTerm(tc, []))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    public static Test ParseTerm_WithCustomIdentifiers => TestThat
        .Given(() => new SentenceParser(new SentenceParserOptions(s => $"p:{s}", s => $"f:{s}", s => $"vc:{s}")))
        .When(p => p.ParseTerm("F(x, C)", [new("vc:x")]))
        .ThenReturns()
        .And((_, rv) => rv.Should().Be(new Function("f:F", new VariableReference("vc:x"), new Function("vc:C"))));

    public static Test ParseTermList_PositiveTestCases => TestThat
        .GivenEachOf<ParseTermListTestCase>(() =>
        [
            new(
                Text: string.Empty,
                Variables: [],
                Expected: []),

            new(
                Text: "   ",
                Variables: [],
                Expected: []),

            new(
                Text: "F()",
                Variables: [],
                Expected: [new Function("F")]),

            new(
                Text: "F()G()",
                Variables: [],
                Expected: [new Function("F"), new Function("G")]),

            new(
                Text: "F() G()",
                Variables: [],
                Expected: [new Function("F"), new Function("G")]),

            new(
                Text: "F()\r\nG()\n",
                Variables: [],
                Expected: [new Function("F"), new Function("G")]),

            new(
                Text: "F(); G()",
                Variables: [],
                Expected: [new Function("F"), new Function("G")]),

            new(
                Text: " F() ; G() ; ",
                Variables: [],
                Expected: [new Function("F"), new Function("G")]),

            new(
                Text: "F() G()aaa",
                Variables: [],
                Expected: [new Function("F"), new Function("G"), new Function("aaa")]),

            new(
                Text: "F() G();aaa",
                Variables: [],
                Expected: [new Function("F"), new Function("G"), new Function("aaa")]),
        ])
        .When(tc => SentenceParser.Default.ParseTermList(tc.Text, []))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Equal(tc.Expected));

    public static Test ParseTermList_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf<string>(() =>
        [
            "F(); ; G()",
        ])
        .When((ctx, tc) => SentenceParser.Default.ParseTermList(tc, []))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    private record ParseTestCase(string Sentence, Sentence ExpectedResult);

    private record ParseListTestCase(string Sentences, Sentence[] Expectation);

    private record ParseTermTestCase(string Text, IEnumerable<VariableDeclaration> Variables, Term Expected);

    private record ParseTermListTestCase(string Text, IEnumerable<VariableDeclaration> Variables, Term[] Expected);
}