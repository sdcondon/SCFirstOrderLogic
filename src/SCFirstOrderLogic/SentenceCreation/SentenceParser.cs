// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Parser for first-order logic sentences.
/// </summary>
public class SentenceParser
{
    private readonly AntlrFacade antlrFacade;

    /// <summary>
    /// Initializes a new instance of the <see cref="SentenceParser"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the parser.</param>
    public SentenceParser(SentenceParserOptions options) => antlrFacade = new(options);

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
    // TODO-BREAKING: remove me
    [Obsolete("This constructor will be removed - use `new SentenceParser(SentenceParserOptions)` instead.")]
    public SentenceParser(
        Func<string, object> getPredicateIdentifier,
        Func<string, object> getFunctionIdentifier,
        Func<string, object> getVariableOrConstantIdentifier)
    {
        antlrFacade = new(new(getPredicateIdentifier, getFunctionIdentifier, getVariableOrConstantIdentifier));
    }

    /// <summary>
    /// <para>
    /// Retrieves an instance of a parser that just uses the symbol text as the identifier for returned predicates, functions, and variables.
    /// </para>
    /// <para>
    /// NB: This means that the identifiers for the zero arity functions declared as `f` and as `f()` are identical.
    /// </para>
    /// </summary>
    // TODO-BREAKING: remove me
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
    public Sentence Parse(string sentence) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(string sentence, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), extraVariables);

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The stream to parse.</param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(Stream sentence) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(Stream sentence, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), extraVariables);

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The text reader to parse.</param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(TextReader sentence) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Sentence"/>.</returns>
    public Sentence Parse(TextReader sentence, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentence(new AntlrInputStream(sentence), extraVariables);

    /// <summary>
    /// Parses a string containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The string to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(string sentences) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a string containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themsselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(string sentences, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), extraVariables);

    /// <summary>
    /// Parses a stream containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The stream to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(Stream sentences) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a stream containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(Stream sentences, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), extraVariables);

    /// <summary>
    /// Parses a text reader containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The text reader to parse.</param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(TextReader sentences) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a text reader containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of sentences.</returns>
    public Sentence[] ParseList(TextReader sentences, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseSentenceList(new AntlrInputStream(sentences), extraVariables);

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The string to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term ParseTerm(string term, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTerm(new AntlrInputStream(term), variables);

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The stream to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term ParseTerm(Stream term, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTerm(new AntlrInputStream(term), variables);

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The text reader to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Term"/>.</returns>
    public Term ParseTerm(TextReader term, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTerm(new AntlrInputStream(term), variables);

    /// <summary>
    /// Parses a string containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The string to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseTermList(string terms, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTermList(new AntlrInputStream(terms), variables);

    /// <summary>
    /// Parses a stream containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The stream to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseTermList(Stream terms, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTermList(new AntlrInputStream(terms), variables);

    /// <summary>
    /// Parses a text reader containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The text reader to parse.</param>
    /// <param name="variables">
    /// <para>
    /// The variables that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of terms.</returns>
    public Term[] ParseTermList(TextReader terms, IEnumerable<VariableDeclaration> variables) => antlrFacade.ParseTermList(new AntlrInputStream(terms), variables);

    /// <summary>
    /// Parses a string containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The string to parse.</param>
    /// <returns>A new array of variable declarations.</returns>
    public VariableDeclaration[] ParseDeclarationList(string declarations) => antlrFacade.ParseDeclarationList(new AntlrInputStream(declarations));

    /// <summary>
    /// Parses a stream containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The stream to parse.</param>
    /// <returns>A new array of variable declarations.</returns>
    public VariableDeclaration[] ParseDeclarationList(Stream declarations) => antlrFacade.ParseDeclarationList(new AntlrInputStream(declarations));

    /// <summary>
    /// Parses a text reader containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The text reader to parse.</param>
    /// <returns>A new array of variable declarations.</returns>
    public VariableDeclaration[] ParseDeclarationList(TextReader declarations) => antlrFacade.ParseDeclarationList(new AntlrInputStream(declarations));

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
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        string sentence,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), extraVariables, out result, out errors);
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
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        Stream sentence,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), extraVariables, out result, out errors);
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
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Sentence"/> object.
    /// </summary>
    /// <param name="sentence">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentence itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentence. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a sentence was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        TextReader sentence,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentence(new AntlrInputStream(sentence), extraVariables, out result, out errors);
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
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        string sentences,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), extraVariables, out result, out errors);
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
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        Stream sentences,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), extraVariables, out result, out errors);
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
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more sentences into a <see cref="Sentence"/> array.
    /// Sentences can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="sentences">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the sentences themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed sentences. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all sentences were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        TextReader sentences,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Sentence[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseSentenceList(new AntlrInputStream(sentences), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The string to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTerm(
        string term,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTerm(new AntlrInputStream(term), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The stream to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTerm(
        Stream term,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTerm(new AntlrInputStream(term), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Term"/> object.
    /// </summary>
    /// <param name="term">The text reader to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTerm(
        TextReader term,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTerm(new AntlrInputStream(term), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The string to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTermList(
        string terms,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTermList(new AntlrInputStream(terms), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parses a stream containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The stream to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTermList(
        Stream terms,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTermList(new AntlrInputStream(terms), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more terms into a <see cref="Term"/> array.
    /// Terms can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="terms">The text reader to parse.</param>
    /// <param name="variables">
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
    public bool TryParseTermList(
        TextReader terms,
        IEnumerable<VariableDeclaration> variables,
        [MaybeNullWhen(false)] out Term[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseTermList(new AntlrInputStream(terms), variables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The string to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed variable declarations. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all declarations were successfully parsed, otherwise false.</returns>
    public bool TryParseDeclarationList(
        string declarations,
        [MaybeNullWhen(false)] out VariableDeclaration[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseDeclarationList(new AntlrInputStream(declarations), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The stream to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed variable declarations. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all declarations were successfully parsed, otherwise false.</returns>
    public bool TryParseDeclarationList(
        Stream declarations,
        [MaybeNullWhen(false)] out VariableDeclaration[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseDeclarationList(new AntlrInputStream(declarations), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing one or more (comma-separated) variable declarations into a <see cref="VariableDeclaration"/> array.
    /// </summary>
    /// <param name="declarations">The text reader to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed variable declarations. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all declarations were successfully parsed, otherwise false.</returns>
    public bool TryParseDeclarationList(
        TextReader declarations,
        [MaybeNullWhen(false)] out VariableDeclaration[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseDeclarationList(new AntlrInputStream(declarations), out result, out errors);
    }
}