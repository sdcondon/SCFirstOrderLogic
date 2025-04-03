// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using SCFirstOrderLogic.SentenceCreation.Antlr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Parser for first-order logic sentences.
/// </summary>
public class SentenceParser
{
    private readonly IdentifierGetters identifierGetters;

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
    public SentenceParser(
        Func<string, object> getPredicateIdentifier,
        Func<string, object> getFunctionIdentifier,
        Func<string, object> getVariableOrConstantIdentifier)
    {
        identifierGetters = new(
            getPredicateIdentifier,
            getFunctionIdentifier,
            getVariableOrConstantIdentifier);
    }

    /// <summary>
    /// <para>
    /// Retrieves an instance of a parser that just uses the symbol text as the identifier for returned predicates, functions, and variables.
    /// </para>
    /// <para>
    /// NB: This means that the identifiers for the zero arity functions declared as `f` and as `f()` are identical.
    /// </para>
    /// </summary>
    public static SentenceParser BasicParser { get; } = new SentenceParser(s => s, s => s, s => s);

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

    private Sentence Parse(AntlrInputStream inputStream)
    {
        return new SentenceTransformation(identifierGetters, Enumerable.Empty<VariableDeclaration>())
            .Visit(MakeParser(inputStream).singleSentence().sentence());
    }

    private Sentence[] ParseList(AntlrInputStream inputStream)
    {
        return MakeParser(inputStream).sentenceList()._sentences
            .Select(s => new SentenceTransformation(identifierGetters, Enumerable.Empty<VariableDeclaration>()).Visit(s))
            .ToArray();
    }

    private static FirstOrderLogicParser MakeParser(AntlrInputStream inputStream)
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
        parser.AddErrorListener(ThrowingErrorListener.Instance);

        return parser;
    }

    private class ThrowingErrorListener : BaseErrorListener
    {
        public static ThrowingErrorListener Instance = new();

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException("line " + line + ":" + charPositionInLine + " " + msg, "sentence");
        }
    }

    private record IdentifierGetters(
        Func<string, object> GetPredicateIdentifier,
        Func<string, object> GetFunctionIdentifier,
        Func<string, object> GetVariableOrConstantIdentifier);

    // Visitor that transforms from a syntax tree generated by ANTLR to a Sentence instance
    private class SentenceTransformation : FirstOrderLogicBaseVisitor<Sentence>
    {
        private readonly IdentifierGetters identifierGetters;
        private readonly IEnumerable<VariableDeclaration> variablesInScope;
        private readonly TermTransformation termTransformation;

        public SentenceTransformation(
            IdentifierGetters identifierGetters,
            IEnumerable<VariableDeclaration> variablesInScope)
        {
            this.identifierGetters = identifierGetters;
            this.variablesInScope = variablesInScope;
            termTransformation = new TermTransformation(identifierGetters, variablesInScope);
        }

        public override Sentence VisitPredicate([NotNull] FirstOrderLogicParser.PredicateContext context)
        {
            return new Predicate(
                identifierGetters.GetPredicateIdentifier(context.ID().Symbol.Text),
                context.argumentList()._elements.Select(e => termTransformation.Visit(e)));
        }

        public override Sentence VisitNegation([NotNull] FirstOrderLogicParser.NegationContext context)
        {
            return new Negation(Visit(context.sentence()));
        }

        public override Sentence VisitEquivalence([NotNull] FirstOrderLogicParser.EquivalenceContext context)
        {
            var subSentences = context.sentence();
            return new Equivalence(Visit(subSentences[0]), Visit(subSentences[1]));
        }

        public override Sentence VisitConjunction([NotNull] FirstOrderLogicParser.ConjunctionContext context)
        {
            var subSentences = context.sentence();
            return new Conjunction(Visit(subSentences[0]), Visit(subSentences[1]));
        }

        public override Sentence VisitExistentialQuantification([NotNull] FirstOrderLogicParser.ExistentialQuantificationContext context)
        {
            var newVariables = context.declarationList()._elements.Select(e => new VariableDeclaration(identifierGetters.GetVariableOrConstantIdentifier(e.Text)));

            Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
            {
                if (!remainingVariables.Any())
                {
                    return new SentenceTransformation(identifierGetters, variablesInScope.Concat(newVariables)).Visit(context.sentence());
                }
                else
                {
                    return new ExistentialQuantification(remainingVariables.First(), MakeSentence(remainingVariables.Skip(1)));
                }
            }

            return MakeSentence(newVariables);
        }

        public override Sentence VisitDisjunction([NotNull] FirstOrderLogicParser.DisjunctionContext context)
        {
            var subSentences = context.sentence();
            return new Disjunction(Visit(subSentences[0]), Visit(subSentences[1]));
        }

        public override Sentence VisitImplication([NotNull] FirstOrderLogicParser.ImplicationContext context)
        {
            var subSentences = context.sentence();
            return new Implication(Visit(subSentences[0]), Visit(subSentences[1]));
        }

        public override Sentence VisitBracketedSentence([NotNull] FirstOrderLogicParser.BracketedSentenceContext context)
        {
            return Visit(context.sentence());
        }

        public override Sentence VisitPredicateEquality([NotNull] FirstOrderLogicParser.PredicateEqualityContext context)
        {
            var arguments = context.term();
            return new Predicate(
                EqualityIdentifier.Instance,
                new[] { termTransformation.Visit(arguments[0]), termTransformation.Visit(arguments[1]) });
        }

        public override Sentence VisitUniversalQuantification([NotNull] FirstOrderLogicParser.UniversalQuantificationContext context)
        {
            var newVariables = context.declarationList()._elements.Select(e => new VariableDeclaration(identifierGetters.GetVariableOrConstantIdentifier(e.Text)));

            Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
            {
                if (!remainingVariables.Any())
                {
                    return new SentenceTransformation(identifierGetters, variablesInScope.Concat(newVariables)).Visit(context.sentence());
                }
                else
                {
                    return new UniversalQuantification(remainingVariables.First(), MakeSentence(remainingVariables.Skip(1)));
                }
            }

            return MakeSentence(newVariables);
        }
    }

    // Visitor that transforms from a syntax tree generated by ANTLR to a Term instance
    private class TermTransformation : FirstOrderLogicBaseVisitor<Term>
    {
        private readonly IdentifierGetters identifierGetters;
        private readonly IEnumerable<VariableDeclaration> variablesInScope;

        public TermTransformation(IdentifierGetters identifierGetters, IEnumerable<VariableDeclaration> variablesInScope)
        {
            this.identifierGetters = identifierGetters;
            this.variablesInScope = variablesInScope;
        }

        public override Term VisitVariableOrConstant([NotNull] FirstOrderLogicParser.VariableOrConstantContext context)
        {
            var identifier = identifierGetters.GetVariableOrConstantIdentifier(context.ID().Symbol.Text);
            var matchingVariableDeclaration = variablesInScope.SingleOrDefault(v => v.Identifier.Equals(identifier));
            if (matchingVariableDeclaration != null)
            {
                // identifier matches a variable that's in scope - interpret as a reference to it
                return new VariableReference(matchingVariableDeclaration);
            }
            else
            {
                // identifier doesn't match any variable in scope - interpret as a zero arity function
                return new Function(identifier);
            }
        }

        public override Term VisitFunction([NotNull] FirstOrderLogicParser.FunctionContext context)
        {
            return new Function(
                identifierGetters.GetFunctionIdentifier(context.ID().Symbol.Text),
                context.argumentList()._elements.Select(e => Visit(e)));
        }
    }
}