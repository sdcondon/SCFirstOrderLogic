﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using SCFirstOrderLogic.SentenceCreation.Antlr;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SCFirstOrderLogic.SentenceCreation
{
    /// <summary>
    /// Parser for first-order logic sentences.
    /// </summary>
    // TODO-FEATURE: All identifiers are just strings (i.e. the token text) for the mo.
    // Might be useful to allow for callbacks (Func<string, object> getPredicateSymbol etc) in case the caller needs richer symbols.
    // ..although, now that we have an actual parser, its becoming clearer that our Symbol props should really be called Identifier.
    // Something for v5.
    public static class SentenceParser
    {
        /// <summary>
        /// Parses a string containing first-order logic syntax into a <see cref="Sentence"/> object.
        /// </summary>
        /// <param name="sentence">The string to parse.</param>
        /// <returns>The parsed <see cref="Sentence"/>.</returns>
        public static Sentence Parse(string sentence)
        {
            AntlrInputStream input = new(sentence);
            FirstOrderLogicLexer lexer = new(input);
            CommonTokenStream tokens = new(lexer);
            FirstOrderLogicParser parser = new(tokens);
            return new SentenceTransformation(Enumerable.Empty<VariableDeclaration>()).Visit(parser.sentence());
        }

        // Visitor that tranforms from a syntax tree generated by ANTLR to a Sentence instance
        private class SentenceTransformation : FirstOrderLogicBaseVisitor<Sentence>
        {
            // variable scoping - could going listener route would avoid need for sub-instances,
            // but visitor is otherwise a great fit, so this'll do for now at least.
            private readonly TermTransformation termTransformation;
            private readonly IEnumerable<VariableDeclaration> variablesInScope;

            public SentenceTransformation(IEnumerable<VariableDeclaration> variablesInScope)
            {
                this.variablesInScope = variablesInScope;
                termTransformation = new TermTransformation(variablesInScope);
            }

            public override Sentence VisitPredicate([NotNull] FirstOrderLogicParser.PredicateContext context)
            {
                return new Predicate(
                    context.ID().Symbol.Text,
                    context.termList()._elements.Select(e => termTransformation.Visit(e)));
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
                var newVariables = context.identifierList()._elements.Select(e => new VariableDeclaration(e.Text));

                Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
                {
                    if (!remainingVariables.Any())
                    {
                        return new SentenceTransformation(variablesInScope.Concat(newVariables)).Visit(context.sentence());
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
                    EqualitySymbol.Instance,
                    new[] { termTransformation.Visit(arguments[0]), termTransformation.Visit(arguments[1]) });
            }

            public override Sentence VisitUniversalQuantification([NotNull] FirstOrderLogicParser.UniversalQuantificationContext context)
            {
                var newVariables = context.identifierList()._elements.Select(e => new VariableDeclaration(e.Text));

                Sentence MakeSentence(IEnumerable<VariableDeclaration> remainingVariables)
                {
                    if (!remainingVariables.Any())
                    {
                        return new SentenceTransformation(variablesInScope.Concat(newVariables)).Visit(context.sentence());
                    }
                    else
                    {
                        return new UniversalQuantification(remainingVariables.First(), MakeSentence(remainingVariables.Skip(1)));
                    }
                }

                return MakeSentence(newVariables);
            }
        }

        // Visitor that tranforms from a syntax tree generated by ANTLR to a Term instance
        private class TermTransformation : FirstOrderLogicBaseVisitor<Term>
        {
            private readonly IEnumerable<VariableDeclaration> variablesInScope;

            public TermTransformation(IEnumerable<VariableDeclaration> variablesInScope)
            {
                this.variablesInScope = variablesInScope;
            }

            public override Term VisitVariableOrConstant([NotNull] FirstOrderLogicParser.VariableOrConstantContext context)
            {
                var symbolText = context.ID().Symbol.Text;
                var variableDeclaration = variablesInScope.SingleOrDefault(v => v.Symbol.Equals(symbolText));
                if (variableDeclaration != null)
                {
                    // symbol matches a variable that's in scope - assume its a reference to it
                    return new VariableReference(variableDeclaration);
                }
                else
                {
                    // symbol doesn't match any variable in scope - assume its a constant
                    return new Constant(symbolText);
                }
            }

            public override Term VisitFunction([NotNull] FirstOrderLogicParser.FunctionContext context)
            {
                return new Function(
                    context.ID().Symbol.Text,
                    context.termList()._elements.Select(e => Visit(e)));
            }
        }
    }
}