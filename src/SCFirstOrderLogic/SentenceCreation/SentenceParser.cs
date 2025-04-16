// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using Antlr4.Runtime;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Parser for first-order logic sentences.
/// </summary>
public class SentenceParser
{
    private readonly SentenceParserOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SentenceParser"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the parser.</param>
    public SentenceParser(SentenceParserOptions options) => this.options = options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SentenceParser"/> class.
    /// </summary>
    /// <param name="getPredicateIdentifier">A delegate to retrieve the identifier for a predicate, given its symbol text.</param>
    /// <param name="getFunctionIdentifier">A delegate to retrieve the identifier for a function, given its symbol text.</param>
    /// <param name="getVariableOrConstantIdentifier">
    /// <para>
    /// A delegate to retrieve the identifier for a variable reference or a constant, given its symbol text.
    /// </para>
    /// <para>
    /// These are combined because we check whether the returned value is among the identifiers of variables
    /// that are in scope in order to determine whether something is a variable reference or a zero arity function
    /// without parentheses. If they were separate, you'd end up in the awkward situation where you'd have to bear
    /// in mind that "getVariableIdentifier" actually gets called for things that are zero arity functions, and for
    /// zero arity functions two "gets" would end up being performed. Or of course we could offer the ability to 
    /// customise this determination logic - which feels overcomplicated.
    /// </para>
    /// </param>
    [Obsolete("This constructor will be removed - use `new SentenceParser(SentenceParserOptions)` instead.")]
    public SentenceParser(
        Func<string, object> getPredicateIdentifier,
        Func<string, object> getFunctionIdentifier,
        Func<string, object> getVariableOrConstantIdentifier)
    {
        options = new SentenceParserOptions(getPredicateIdentifier, getFunctionIdentifier, getVariableOrConstantIdentifier);
    }

    /// <summary>
    /// <para>
    /// Retrieves an instance of a parser that just uses the symbol text as the identifier for returned predicates, functions, and variables.
    /// </para>
    /// <para>
    /// NB: This means that the identifiers for the zero arity functions declared as `f` and as `f()` are identical.
    /// </para>
    /// </summary>
    [Obsolete("This property will be removed - use `SentenceParser.Default` instead.")]
    public static SentenceParser BasicParser => Default;

    /// <summary>
    /// Gets an instance of the <see cref="SentenceParser"/> class that uses <see cref="SentenceParserOptions.Default"/>.
    /// </summary>
    public static SentenceParser Default { get; } = new(SentenceParserOptions.Default);

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The string to parse.</param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(string sentence) => Parse(new AntlrInputStream(sentence));

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The stream to parse.</param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(Stream sentence) => Parse(new AntlrInputStream(sentence));

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The text reader to parse.</param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(TextReader sentence) => Parse(new AntlrInputStream(sentence));

    /// <summary>
    /// Parses a string containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The string to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(string sentences) => ParseList(new AntlrInputStream(sentences));

    /// <summary>
    /// Parses a stream containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The stream to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(Stream sentences) => ParseList(new AntlrInputStream(sentences));

    /// <summary>
    /// Parses a text reader containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The text reader to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(TextReader sentences) => ParseList(new AntlrInputStream(sentences));








    /// <summary>
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The string to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        string sentence,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(sentence), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The stream to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        Stream sentence,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(sentence), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The text reader to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        TextReader sentence,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(sentence), out result, out errors);
    } 

    /// <summary>
    /// Attempts to parse a string containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The string to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        string sentences,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors) 
    {
        return TryParseList(new AntlrInputStream(sentences), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The stream to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        Stream sentences,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParseList(new AntlrInputStream(sentences), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The text reader to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        TextReader sentences,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParseList(new AntlrInputStream(sentences), out result, out errors);
    }

    private Sentence Parse(AntlrInputStream inputStream)
    {
        if (!TryParse(inputStream, out var sentence, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentence;
    }

    private Sentence[] ParseList(AntlrInputStream inputStream)
    {
        if (!TryParseList(inputStream, out var sentences, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentences;
    }

    private bool TryParse(
        AntlrInputStream inputStream,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = new SentenceTransformation(options, Enumerable.Empty<VariableDeclaration>())
            .Visit(AntlrParserFactory.MakeParser(inputStream, errorListener).singleSentence().sentence());

        if (errorListener.Errors.Any())
        {
            errors = errorListener.Errors.ToArray();
            return false;
        }

        errors = null;
        return true;
    }

    private bool TryParseList(
        AntlrInputStream inputStream,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = AntlrParserFactory.MakeParser(inputStream, errorListener).sentenceList()._sentences
            .Select(s => new SentenceTransformation(options, Enumerable.Empty<VariableDeclaration>()).Visit(s))
            .ToArray();

        if (errorListener.Errors.Any())
        {
            errors = errorListener.Errors.ToArray();
            return false;
        }

        errors = null;
        return true;
    }
}