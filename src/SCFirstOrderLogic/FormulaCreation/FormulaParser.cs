// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using Antlr4.Runtime;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SCFirstOrderLogic.FormulaCreation;

/// <summary>
/// Parser for first-order logic formulas.
/// </summary>
public class FormulaParser
{
    private readonly AntlrFacade antlrFacade;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormulaParser"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the parser.</param>
    public FormulaParser(FormulaParserOptions options) => antlrFacade = new(options);

    /// <summary>
    /// Gets an instance of the <see cref="FormulaParser"/> class that uses <see cref="FormulaParserOptions.Default"/>.
    /// </summary>
    public static FormulaParser Default { get; } = new(FormulaParserOptions.Default);

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The string to parse.</param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(string formula) => antlrFacade.ParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a string containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(string formula, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormula(new AntlrInputStream(formula), extraVariables);

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The stream to parse.</param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(Stream formula) => antlrFacade.ParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a stream containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(Stream formula, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormula(new AntlrInputStream(formula), extraVariables);

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The text reader to parse.</param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(TextReader formula) => antlrFacade.ParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a text reader containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>The parsed <see cref="Formula"/>.</returns>
    public Formula Parse(TextReader formula, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormula(new AntlrInputStream(formula), extraVariables);

    /// <summary>
    /// Parses a string containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The string to parse.</param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(string formulas) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a string containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themsselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(string formulas, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), extraVariables);

    /// <summary>
    /// Parses a stream containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The stream to parse.</param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(Stream formulas) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a stream containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(Stream formulas, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), extraVariables);

    /// <summary>
    /// Parses a text reader containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The text reader to parse.</param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(TextReader formulas) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>());

    /// <summary>
    /// Parses a text reader containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <returns>A new array of formulas.</returns>
    public Formula[] ParseList(TextReader formulas, IEnumerable<VariableDeclaration> extraVariables) => antlrFacade.ParseFormulaList(new AntlrInputStream(formulas), extraVariables);

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
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The string to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        string formula,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        string formula,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The stream to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        Stream formula,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        Stream formula,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The text reader to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        TextReader formula,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing first-order logic syntax into a <see cref="Formula"/> object.
    /// </summary>
    /// <param name="formula">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formula itself) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formula. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if a formula was successfully parsed, otherwise false.</returns>
    public bool TryParse(
        TextReader formula,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormula(new AntlrInputStream(formula), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The string to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        string formulas,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a string containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The string to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        string formulas,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The stream to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        Stream formulas,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a stream containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The stream to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        Stream formulas,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), extraVariables, out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The text reader to parse.</param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        TextReader formulas,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), Enumerable.Empty<VariableDeclaration>(), out result, out errors);
    }

    /// <summary>
    /// Attempts to parse a text reader containing zero or more formulas into a <see cref="Formula"/> array.
    /// Formulas can be separated by a semi-colon and/or whitespace, but it is not required.
    /// </summary>
    /// <param name="formulas">The text reader to parse.</param>
    /// <param name="extraVariables">
    /// <para>
    /// Any extra variables (beyond those quantified in the formulas themselves) that should be considered in scope while parsing.
    /// </para>
    /// <para>
    /// Affects whether identifiers without trailing brackets encountered when a term is expected are interpreted as variables or zero-arity functions.
    /// </para>
    /// </param>
    /// <param name="result">If parsing succeeds, will be the parsed formulas. If parsing fails, will be null.</param>
    /// <param name="errors">If parsing fails, will be the details of all errors that were detected. If parsing succeeds, will be null.</param>
    /// <returns>True if all formulas were successfully parsed, otherwise false.</returns>
    public bool TryParseList(
        TextReader formulas,
        IEnumerable<VariableDeclaration> extraVariables,
        [MaybeNullWhen(false)] out Formula[] result,
        [MaybeNullWhen(true)] out SyntaxError[] errors)
    {
        return antlrFacade.TryParseFormulaList(new AntlrInputStream(formulas), extraVariables, out result, out errors);
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