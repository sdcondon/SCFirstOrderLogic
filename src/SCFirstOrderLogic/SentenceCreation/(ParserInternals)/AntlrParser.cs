using Antlr4.Runtime;
using SCFirstOrderLogic.SentenceCreation.Antlr;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

internal class AntlrParser
{
    private readonly SentenceParserOptions options;

    public AntlrParser(
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

        return HasErrors(errorListener, out errors);
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

        return HasErrors(errorListener, out errors);
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

        return HasErrors(errorListener, out errors);
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

        return HasErrors(errorListener, out errors);
    }

    public bool TryParseDeclarationList(
        AntlrInputStream inputStream,
        [MaybeNullWhen(false)] out VariableDeclaration[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = MakeParser(inputStream, errorListener).singleDeclarationList().declarationList()._elements
            .Select(e => new VariableDeclaration(options.GetVariableOrConstantIdentifier(e.Text)))
            .ToArray();

        return HasErrors(errorListener, out errors);
    }

    private static FirstOrderLogicParser MakeParser(
        AntlrInputStream inputStream,
        IAntlrErrorListener<IToken> errorListener)
    {
        // NB: ANTLR apparently adds a listener by default that writes to the console.
        // Which is crazy default behaviour if you ask me, but never mind.
        // We remove it so that consumers of this lib don't get random messages turning up on their console.
        FirstOrderLogicLexer lexer = new(inputStream);
        lexer.RemoveErrorListeners();
        CommonTokenStream tokens = new(lexer);

        // NB: In the parser, we add our own error listener that throws an exception.
        // Otherwise errors would just be ignored and the method would just return null, which is obviously bad behaviour.
        FirstOrderLogicParser parser = new(tokens);
        parser.RemoveErrorListeners();
        parser.AddErrorListener(errorListener);

        return parser;
    }

    private bool HasErrors(
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
