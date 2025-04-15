﻿using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceCreation;

public static class TermParserTests
{
    public static Test Parse_PositiveTestCases => TestThat
        .GivenEachOf<ParseTestCase>(() =>
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
        .When(tc => new TermParser().Parse(tc.Text, tc.Variables))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test Parse_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf(() => new[]
        {
            string.Empty,
            "F(x,y,)",
            "F(,x,y)",
            "F()aaa",
        })
        .When((ctx, tc) => new TermParser().Parse(tc, []))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    public static Test Parse_WithCustomIdentifiers => TestThat
        .Given(() => new TermParser(new TermParserOptions(s => $"f:{s}", s => $"vc:{s}")))
        .When(p => p.Parse("F(x, C)", [new("vc:x")]))
        .ThenReturns()
        .And((_, rv) => rv.Should().Be(new Function("f:F", new VariableReference("vc:x"), new Function("vc:C"))));

    public static Test ParseList_PositiveTestCases => TestThat
        .GivenEachOf<ParseListTestCase>(() =>
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
        .When(tc => new TermParser().ParseList(tc.Text, []))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Equal(tc.Expected));

    public static Test ParseList_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf<string>(() =>
        [
            "F(); ; G()",
        ])
        .When((ctx, tc) => new TermParser().ParseList(tc, []))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    public static Test ParseVariableDeclarationList_PositiveTestCases => TestThat
        .GivenEachOf<ParseDeclarationListTestCase>(() =>
        [
            new(
                Text: " ",
                Expected: []),

            new(
                Text: "x, y",
                Expected: [new VariableDeclaration("vc:x"), new VariableDeclaration("vc:y")]),
        ])
        .When(tc => new TermParser(new TermParserOptions(s => s, s => $"vc:{s}")).ParseVariableDeclarationList(tc.Text))
        .ThenReturns()
        .And((tc, rv) => rv.Should().Equal(tc.Expected));

    public static Test ParseVariableDeclarationList_NegativeTestCases => TestThat
        .GivenTestContext()
        .AndEachOf<string>(() =>
        [
            ",",
            "a,",
            "a()",
        ])
        .When((ctx, tc) => new TermParser().ParseVariableDeclarationList(tc))
        .ThenThrows((ctx, _, e) => ctx.WriteOutput(e.Message));

    private record ParseTestCase(string Text, IEnumerable<VariableDeclaration> Variables, Term Expected);

    private record ParseListTestCase(string Text, IEnumerable<VariableDeclaration> Variables, Term[] Expected);

    private record ParseDeclarationListTestCase(string Text, IEnumerable<VariableDeclaration> Expected);
}