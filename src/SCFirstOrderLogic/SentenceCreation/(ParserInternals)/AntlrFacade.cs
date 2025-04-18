﻿using Antlr4.Runtime;
using SCFirstOrderLogic.SentenceCreation.Antlr;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

internal class AntlrFacade
{
    private readonly SentenceParserOptions options;

    public AntlrFacade(
        SentenceParserOptions options)
    {
        this.options = options;
    }

    public Sentence ParseSentence(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables)
    {
        if (!TryParseSentence(inputStream, variables, out var sentence, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentence;
    }

    public Sentence[] ParseSentenceList(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables)
    {
        if (!TryParseSentenceList(inputStream, variables, out var sentences, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentences;
    }

    public Term ParseTerm(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables)
    {
        if (!TryParseTerm(inputStream, variables, out var sentence, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentence;
    }

    public Term[] ParseTermList(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables)
    {
        if (!TryParseTermList(inputStream, variables, out var sentences, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentences;
    }

    public VariableDeclaration[] ParseDeclarationList(
       AntlrInputStream inputStream)
    {
        if (!TryParseDeclarationList(inputStream, out var declarations, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return declarations;
    }

    public bool TryParseSentence(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = new SentenceTransformation(options, variables)
            .Visit(MakeParser(inputStream, errorListener).singleSentence().sentence());

        return HasNoErrors(errorListener, out errors);
    }

    public bool TryParseSentenceList(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = MakeParser(inputStream, errorListener).sentenceList()._sentences
            .Select(s => new SentenceTransformation(options, variables).Visit(s))
            .ToArray();

        return HasNoErrors(errorListener, out errors);
    }

    public bool TryParseTerm(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = new TermTransformation(options, variables)
            .Visit(MakeParser(inputStream, errorListener).singleTerm().term());

        return HasNoErrors(errorListener, out errors);
    }

    public bool TryParseTermList(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = MakeParser(inputStream, errorListener).termList()._terms
            .Select(s => new TermTransformation(options, variables).Visit(s))
            .ToArray();

        return HasNoErrors(errorListener, out errors);
    }

    public bool TryParseDeclarationList(
        AntlrInputStream inputStream,
        [MaybeNullWhen(false)] out VariableDeclaration[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = MakeParser(inputStream, errorListener).singleDeclarationList()._elements
            .Select(e => new VariableDeclaration(options.GetVariableOrConstantIdentifier(e.Text)))
            .ToArray();

        return HasNoErrors(errorListener, out errors);
    }

    private static FirstOrderLogicParser MakeParser(
        AntlrInputStream inputStream,
        SyntaxErrorListener errorListener)
    {
        // NB: ANTLR apparently adds a listener by default that writes to the console.
        // Which is crazy default behaviour if you ask me, but never mind.
        // We remove it so that consumers of this lib don't get random messages turning up on their console.
        // We add our own listener instead, that keeps track of errors so that we can check at the end.
        FirstOrderLogicLexer lexer = new(inputStream);
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(errorListener);
        CommonTokenStream tokens = new(lexer);

        // ..and same for the parser..
        FirstOrderLogicParser parser = new(tokens);
        parser.RemoveErrorListeners();
        parser.AddErrorListener(errorListener);
        return parser;
    }

    private static bool HasNoErrors(
        SyntaxErrorListener syntaxErrorListener,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        if (syntaxErrorListener.Errors.Any())
        {
            errors = syntaxErrorListener.Errors.ToArray();
            return false;
        }

        errors = null;
        return true;
    }
}
