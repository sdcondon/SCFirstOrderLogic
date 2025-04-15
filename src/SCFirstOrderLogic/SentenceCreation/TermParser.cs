using Antlr4.Runtime;
using System;
using System.Collections.Generic;
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
    /// Initializes a new instance of the <see cref="TermParser"/> class that uses <see cref="TermParserOptions.Default"/>.
    /// </summary>
    public TermParser() => this.options = TermParserOptions.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="TermParser"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the parser.</param>
    public TermParser(TermParserOptions options) => this.options = options;

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
    /// Parse a string containing zero or more comma-separated variable declarations into an array of <see cref="VariableDeclaration"/> instances -
    /// including invocation of identifier getter from the parser options.
    /// </summary>
    /// <param name="declarations">The string to parse.</param>
    /// <returns>A new array of variable declarations.</returns>
    public VariableDeclaration[] ParseVariableDeclarationList(string declarations)
    {
        if (string.IsNullOrWhiteSpace(declarations))
        {
            return Array.Empty<VariableDeclaration>();
        }

        return AntlrParserFactory.MakeParser(new AntlrInputStream(declarations)).declarationList()._elements
            .Select(s => new VariableDeclaration(options.GetVariableOrConstantIdentifier(s.Text)))
            .ToArray();
    }

    private Term Parse(AntlrInputStream inputStream, IEnumerable<VariableDeclaration> variablesInScope)
    {
        return new TermTransformation(options, variablesInScope)
            .Visit(AntlrParserFactory.MakeParser(inputStream).singleTerm().term());
    }

    private Term[] ParseList(AntlrInputStream inputStream, IEnumerable<VariableDeclaration> variablesInScope)
    {
        return AntlrParserFactory.MakeParser(inputStream).termList()._terms
            .Select(s => new TermTransformation(options, variablesInScope).Visit(s))
            .ToArray();
    }
}
