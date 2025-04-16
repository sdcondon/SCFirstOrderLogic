using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Parser for first-order logic terms.
/// </summary>
public class TermParser
{
    private readonly TermParserOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TermParser"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the parser.</param>
    public TermParser(TermParserOptions options) => this.options = options;

    /// <summary>
    /// Gets an instance of the <see cref="TermParser"/> class that uses <see cref="TermParserOptions.Default"/>.
    /// </summary>
    public static TermParser Default { get; } = new(TermParserOptions.Default);

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The string to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term Parse(string term, IEnumerable<VariableDeclaration> variablesInScope) => Parse(new AntlrInputStream(term), variablesInScope);

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The stream to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term Parse(Stream term, IEnumerable<VariableDeclaration> variablesInScope) => Parse(new AntlrInputStream(term), variablesInScope);

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The text reader to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term Parse(TextReader term, IEnumerable<VariableDeclaration> variablesInScope) => Parse(new AntlrInputStream(term), variablesInScope);

    /// <summary>
    /// Parses a string containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The string to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseList(string terms, IEnumerable<VariableDeclaration> variablesInScope) => ParseList(new AntlrInputStream(terms), variablesInScope);

    /// <summary>
    /// Parses a stream containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The stream to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseList(Stream terms, IEnumerable<VariableDeclaration> variablesInScope) => ParseList(new AntlrInputStream(terms), variablesInScope);

    /// <summary>
    /// Parses a text reader containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The text reader to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseList(TextReader terms, IEnumerable<VariableDeclaration> variablesInScope) => ParseList(new AntlrInputStream(terms), variablesInScope);

    /// <summary>
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The string to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed term. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a term was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        string term,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(term), variablesInScope, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The stream to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed term. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a term was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        Stream term,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(term), variablesInScope, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The text reader to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed term. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a term was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        TextReader term,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParse(new AntlrInputStream(term), variablesInScope, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The string to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed terms. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all terms were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        string terms,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParseList(new AntlrInputStream(terms), variablesInScope, out result, out errors);
    }

    /// <summary>
    /// Attempts to parses a stream containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The stream to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed terms. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all terms were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        Stream terms,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParseList(new AntlrInputStream(terms), variablesInScope, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The text reader to parse.</param>
    /// <param name="variablesInScope">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed terms. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all terms were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        TextReader terms,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return TryParseList(new AntlrInputStream(terms), variablesInScope, out result, out errors);
    }

    private Term Parse(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variablesInScope)
    {
        if (!TryParse(inputStream, variablesInScope, out var sentence, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentence;
    }

    private Term[] ParseList(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variablesInScope)
    {
        if (!TryParseList(inputStream, variablesInScope, out var sentences, out var errors))
        {
            throw new SyntaxErrorsException(errors);
        }

        return sentences;
    }

    private bool TryParse(
        AntlrInputStream inputStream,
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = new TermTransformation(options, variablesInScope)
            .Visit(AntlrParserFactory.MakeParser(inputStream, errorListener).singleTerm().term());

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
        IEnumerable<VariableDeclaration> variablesInScope,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        var errorListener = new SyntaxErrorListener();

        result = AntlrParserFactory.MakeParser(inputStream, errorListener).termList()._terms
            .Select(s => new TermTransformation(options, variablesInScope).Visit(s))
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
