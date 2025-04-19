﻿using SCFirstOrderLogic.SentenceCreation.Antlr;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Visitor that transforms from a syntax tree generated by ANTLR to a <see cref="Sentence"/> instance.
/// </summary>
internal class SentenceTransformation : FirstOrderLogicBaseVisitor<Sentence>
{
    private readonly SentenceParserOptions options;
    private readonly IEnumerable<VariableDeclaration> variablesInScope;
    private readonly TermTransformation termTransformation;

    public SentenceTransformation(
        SentenceParserOptions options,
        IEnumerable<VariableDeclaration> variablesInScope)
    {
        this.options = options;
        this.variablesInScope = variablesInScope;
        termTransformation = new TermTransformation(options, variablesInScope);
    }

    public override Sentence VisitPredicate([NotNull] FirstOrderLogicParser.PredicateContext context)
    {
        return new Predicate(
            options.GetPredicateIdentifier(context.IDENTIFIER().Symbol.Text),
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
        var newVariables = context.declarationList()._elements.Select(e => new VariableDeclaration(options.GetVariableOrConstantIdentifier(e.Text)));

        Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
        {
            if (!remainingVariables.Any())
            {
                return new SentenceTransformation(options, variablesInScope.Concat(newVariables)).Visit(context.sentence());
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
        var newVariables = context.declarationList()._elements.Select(e => new VariableDeclaration(options.GetVariableOrConstantIdentifier(e.Text)));

        Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
        {
            if (!remainingVariables.Any())
            {
                return new SentenceTransformation(options, variablesInScope.Concat(newVariables)).Visit(context.sentence());
            }
            else
            {
                return new UniversalQuantification(remainingVariables.First(), MakeSentence(remainingVariables.Skip(1)));
            }
        }

        return MakeSentence(newVariables);
    }
}

