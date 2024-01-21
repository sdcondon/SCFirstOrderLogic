// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Linq;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// This class provides functionality for rendering <see cref="Sentence"/> instances (and <see cref="CNFSentence"/> instances) in the standard first-order logic syntax.
    /// Using a single <see cref="SentenceFormatter"/> instance allows for unique (and customisable) labelling of standardised variables and Skolem functions for all sentences formatted with the instance.
    /// </summary>
    // TODO-FEATURE: Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed). 
    public class SentenceFormatter
    {
        private const char PrecedenceBracketL = '[';
        private const char PrecedenceBracketR = ']';

        private readonly ILabellingScope<StandardisedVariableIdentifier> standardisedVariableLabellingScope;
        private readonly ILabellingScope<SkolemFunctionIdentifier> skolemFunctionLabellingScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceFormatter"/> class that uses the <see cref="DefaultStandardisedVariableLabeller"/>
        /// and <see cref="DefaultSkolemFunctionLabeller"/>.
        /// </summary>
        public SentenceFormatter()
            : this(DefaultStandardisedVariableLabeller, DefaultSkolemFunctionLabeller)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceFormatter"/> class.
        /// </summary>
        /// <param name="standardisedVariableLabeller">The labeller to use for standardised variables.</param>
        /// <param name="skolemFunctionLabeller">The labeller to use for Skolem functions.</param>
        public SentenceFormatter(ILabeller<StandardisedVariableIdentifier> standardisedVariableLabeller, ILabeller<SkolemFunctionIdentifier> skolemFunctionLabeller)
        {
            this.standardisedVariableLabellingScope = standardisedVariableLabeller.MakeLabellingScope();
            this.skolemFunctionLabellingScope = skolemFunctionLabeller.MakeLabellingScope();
        }

        /// <summary>
        /// Gets or sets the default labeller to use for standardised variables. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to a <see cref="SubscriptSuffixLabeller"/>.
        /// </summary>
        public static ILabeller<StandardisedVariableIdentifier> DefaultStandardisedVariableLabeller { get; set; } =
            new SubscriptSuffixLabeller();

        /// <summary>
        /// Gets or sets the default labeller to use for Skolem functions. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to a <see cref="LabelSetLabeller{T}"/> using the (upper-case) modern Latin alphabet;
        /// </summary>
        public static ILabeller<SkolemFunctionIdentifier> DefaultSkolemFunctionLabeller { get; set; } =
            new LabelSetLabeller<SkolemFunctionIdentifier>(LabelSets.UpperModernLatinAlphabet);

        /// <summary>
        /// Returns a string representation of a given <see cref="CNFClause"/> instance.
        /// </summary>
        /// <param name="clause">The clause to be formatted.</param>
        /// <returns>A string representation of the given clause.</returns>
        public string Format(CNFClause clause) => string.Join(" ∨ ", clause.Literals.Select(l => Format(l)));

        /// <summary>
        /// Returns a string representation of a given <see cref="CNFDefiniteClause"/> instance.
        /// </summary>
        /// <param name="definiteClause">The clause to be formatted.</param>
        /// <returns>A string representation of the given clause.</returns>
        public string Format(CNFDefiniteClause definiteClause)
        {
            if (definiteClause.IsUnitClause)
            {
                return Format(definiteClause.Consequent);
            }
            else
            {
                return $"{ string.Join(" ∧ ", definiteClause.Conjuncts.Select(c => Format(c))) } ⇒ { Format(definiteClause.Consequent) }";
            }
        }

        /// <summary>
        /// Returns a string representation of a given <see cref="CNFSentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to be formatted.</param>
        /// <returns>A string representation of the given sentence.</returns>
        public string Format(CNFSentence sentence) => string.Join(" ∧ ", sentence.Clauses.Select(c => $"{PrecedenceBracketL}{Format(c)}{PrecedenceBracketR}"));

        /// <summary>
        /// Returns a string representation of a given <see cref="Literal"/> instance.
        /// </summary>
        /// <param name="literal">The literal to be formatted.</param>
        /// <returns>A string representation of the given literal.</returns>
        public string Format(Literal literal) => $"{(literal.IsNegated ? "¬" : "")}{Format(literal.Predicate)}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Sentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to be formatted.</param>
        /// <returns>A string representation of the given sentence.</returns>
        public string Format(Sentence sentence) => sentence switch
        {
            Conjunction conjunction => Format(conjunction),
            Disjunction disjunction => Format(disjunction),
            Equivalence equivalence => Format(equivalence),
            ExistentialQuantification existentialQuantification => Format(existentialQuantification),
            Implication implication => Format(implication),
            Negation negation => Format(negation),
            Predicate predicate => Format(predicate),
            UniversalQuantification universalQuantification => Format(universalQuantification),
            _ => throw new ArgumentException("Unsupported sentence type")
        };

        /// <summary>
        /// Returns a string representation of a given <see cref="Conjunction"/> instance.
        /// </summary>
        /// <param name="conjunction">The conjunction to be formatted.</param>
        /// <returns>A string representation of the given conjunction.</returns>
        public string Format(Conjunction conjunction) =>
            $"{PrecedenceBracketL}{Format(conjunction.Left)} ∧ {Format(conjunction.Right)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Disjunction"/> instance.
        /// </summary>
        /// <param name="disjunction">The disjunction to be formatted.</param>
        /// <returns>A string representation of the given disjunction.</returns>
        public string Format(Disjunction disjunction) =>
            $"{PrecedenceBracketL}{Format(disjunction.Left)} ∨ {Format(disjunction.Right)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Equivalence"/> instance.
        /// </summary>
        /// <param name="equivalence">The equivalence to be formatted.</param>
        /// <returns>A string representation of the given equivalence.</returns>
        public string Format(Equivalence equivalence) =>
            $"{PrecedenceBracketL}{Format(equivalence.Left)} ⇔ {Format(equivalence.Right)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="ExistentialQuantification"/> instance.
        /// </summary>
        /// <param name="existentialQuantification">The existential quantification to be formatted.</param>
        /// <returns>A string representation of the given existential quantification.</returns>
        public string Format(ExistentialQuantification existentialQuantification) =>
            $"{PrecedenceBracketL}∃ {Format(existentialQuantification.Variable)}, {Format(existentialQuantification.Sentence)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Implication"/> instance.
        /// </summary>
        /// <param name="implication">The implication to be formatted.</param>
        /// <returns>A string representation of the given implication.</returns>
        public string Format(Implication implication) =>
            $"{PrecedenceBracketL}{Format(implication.Antecedent)} ⇒ {Format(implication.Consequent)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Negation"/> instance.
        /// </summary>
        /// <param name="negation">The negation to be formatted.</param>
        /// <returns>A string representation of the given negation.</returns>
        public string Format(Negation negation) =>
            $"¬{Format(negation.Sentence)}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Predicate"/> instance.
        /// </summary>
        /// <param name="predicate">The predicate to be formatted.</param>
        /// <returns>A string representation of the given predicate.</returns>
        public string Format(Predicate predicate) =>
            $"{predicate.Identifier}({string.Join(", ", predicate.Arguments.Select(a => Format(a)))})";

        /// <summary>
        /// Returns a string representation of a given <see cref="UniversalQuantification"/> instance.
        /// </summary>
        /// <param name="universalQuantification">The universal quantification to be formatted.</param>
        /// <returns>A string representation of the given universal quantification.</returns>
        public string Format(UniversalQuantification universalQuantification) =>
            $"{PrecedenceBracketL}∀ {Format(universalQuantification.Variable)}, {Format(universalQuantification.Sentence)}{PrecedenceBracketR}";

        /// <summary>
        /// Returns a string representation of a given <see cref="Term"/> instance.
        /// </summary>
        /// <param name="term">The term to be formatted.</param>
        /// <returns>A string representation of the given term.</returns>
        public string Format(Term term) => term switch
        {
            Constant constant => Format(constant),
            VariableReference variable => Format(variable),
            Function function => Format(function),
            _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'")
        };

        /// <summary>
        /// Returns a string representation of a given <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="constant">The constant to be formatted.</param>
        /// <returns>A string representation of the given constant.</returns>
        public string Format(Constant constant) =>
            constant.Identifier.ToString() ?? throw new ArgumentException("Cannot format constant because ToString of its identifier returned null", nameof(constant));

        /// <summary>
        /// Returns a string representation of a given <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="variableReference">The variable reference to be formatted.</param>
        /// <returns>A string representation of the given variable reference.</returns>
        public string Format(VariableReference variableReference) =>
            Format(variableReference.Declaration);

        /// <summary>
        /// Returns a string representation of a given <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to be formatted.</param>
        /// <returns>A string representation of the given function.</returns>
        public string Format(Function function)
        {
            var label = function.Identifier is SkolemFunctionIdentifier skm ? Format(skm) : function.Identifier.ToString();
            return $"{label}({string.Join(", ", function.Arguments.Select(a => Format(a)))})";
        }

        /// <summary>
        /// Returns a string representation of a given <see cref="SkolemFunctionIdentifier"/> instance.
        /// </summary>
        /// <param name="identifier">The identifier to be formatted.</param>
        /// <returns>A string representation of the given identifier.</returns>
        public string Format(SkolemFunctionIdentifier identifier) => skolemFunctionLabellingScope.GetLabel(identifier);

        /// <summary>
        /// Returns a string representation of a given <see cref="VariableDeclaration"/> instance.
        /// </summary>
        /// <param name="variableDeclaration">The variable declaration to be formatted.</param>
        /// <returns>A string representation of the given variable declaration.</returns>
        public string Format(VariableDeclaration variableDeclaration)
        {
            return variableDeclaration.Identifier switch
            {
                StandardisedVariableIdentifier std => Format(std),
                _ => variableDeclaration.Identifier.ToString() ?? throw new ArgumentException("Cannot format variable declaration because ToString of its identifier returned null", nameof(variableDeclaration))
            };
        }

        /// <summary>
        /// Returns a string representation of a given <see cref="StandardisedVariableIdentifier"/> instance.
        /// </summary>
        /// <param name="identifier">The identifier to be formatted.</param>
        /// <returns>A string representation of the given standardised variable identifier.</returns>
        public string Format(StandardisedVariableIdentifier identifier) => standardisedVariableLabellingScope.GetLabel(identifier);
    }
}
