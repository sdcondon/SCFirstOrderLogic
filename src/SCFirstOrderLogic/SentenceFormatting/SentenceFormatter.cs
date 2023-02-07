using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// <para>
    /// This class provides functionality for rendering <see cref="Sentence"/> instances (and <see cref="CNFSentence"/> instances) in the standard first-order logic syntax.
    /// Using a single <see cref="SentenceFormatter"/> instance guarantees unique (and customisable) labelling of standardised variables and Skolem functions for all sentences formatted with the instance.
    /// </para>
    /// <para>
    /// NB: fairly likely to change in future. This implementation will suffice while I figure out what I want (IFormatProvider and ICustomFormatter/s, probably).
    /// Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed), too.
    /// </para>
    /// </summary>
    public class SentenceFormatter
    {
        private readonly ILabellingScope<StandardisedVariableSymbol> standardisedVariableLabellingScope;
        private readonly ILabellingScope<SkolemFunctionSymbol> skolemFunctionLabellingScope;

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
        public SentenceFormatter(ILabeller<StandardisedVariableSymbol> standardisedVariableLabeller, ILabeller<SkolemFunctionSymbol> skolemFunctionLabeller)
        {
            this.standardisedVariableLabellingScope = standardisedVariableLabeller.MakeLabellingScope();
            this.skolemFunctionLabellingScope = skolemFunctionLabeller.MakeLabellingScope();
        }

        /// <summary>
        /// Gets or sets the default labeller to use for standardised variables. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to a <see cref="LabelSetLabeller{T}"/> using the (lower-case) Greek alphabet.
        /// </summary>
        public static ILabeller<StandardisedVariableSymbol> DefaultStandardisedVariableLabeller { get; set; } = 
            new LabelSetLabeller<StandardisedVariableSymbol>(LabelSets.LowerGreekAlphabet);

        /// <summary>
        /// Gets or sets the default labeller to use for Skolem functions. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to a <see cref="LabelSetLabeller{T}"/> using the (upper-case) modern Latin alphabet;
        /// </summary>
        public static ILabeller<SkolemFunctionSymbol> DefaultSkolemFunctionLabeller { get; set; } =
            new LabelSetLabeller<SkolemFunctionSymbol>(LabelSets.UpperModernLatinAlphabet);

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
            $"[{Format(conjunction.Left)} ∧ {Format(conjunction.Right)}]";

        /// <summary>
        /// Returns a string representation of a given <see cref="Disjunction"/> instance.
        /// </summary>
        /// <param name="disjunction">The disjunction to be formatted.</param>
        /// <returns>A string representation of the given disjunction.</returns>
        public string Format(Disjunction disjunction) =>
            $"[{Format(disjunction.Left)} ∨ {Format(disjunction.Right)}]";

        /// <summary>
        /// Returns a string representation of a given <see cref="Equivalence"/> instance.
        /// </summary>
        /// <param name="equivalence">The equivalence to be formatted.</param>
        /// <returns>A string representation of the given equivalence.</returns>
        public string Format(Equivalence equivalence) =>
            $"[{Format(equivalence.Left)} ⇔ {Format(equivalence.Right)}]";

        /// <summary>
        /// Returns a string representation of a given <see cref="ExistentialQuantification"/> instance.
        /// </summary>
        /// <param name="existentialQuantification">The existential quantification to be formatted.</param>
        /// <returns>A string representation of the given existential quantification.</returns>
        public string Format(ExistentialQuantification existentialQuantification) =>
            $"[∃ {Format(existentialQuantification.Variable)}, {Format(existentialQuantification.Sentence)}]";

        /// <summary>
        /// Returns a string representation of a given <see cref="Implication"/> instance.
        /// </summary>
        /// <param name="implication">The implication to be formatted.</param>
        /// <returns>A string representation of the given implication.</returns>
        public string Format(Implication implication) =>
            $"[{Format(implication.Antecedent)} ⇒ {Format(implication.Consequent)}]";

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
            $"{predicate.Symbol}({string.Join(", ", predicate.Arguments.Select(a => Format(a)))})";

        /// <summary>
        /// Returns a string representation of a given <see cref="UniversalQuantification"/> instance.
        /// </summary>
        /// <param name="universalQuantification">The universal quantification to be formatted.</param>
        /// <returns>A string representation of the given universal quantification.</returns>
        public string Format(UniversalQuantification universalQuantification) =>
            $"[∀ {Format(universalQuantification.Variable)}, {Format(universalQuantification.Sentence)}]";

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
            constant.Symbol.ToString() ?? throw new ArgumentException("Cannot format constant because ToString of its symbol returned null", nameof(constant));

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
            var label = function.Symbol is SkolemFunctionSymbol skm ? Format(skm) : function.Symbol.ToString();
            return $"{label}({string.Join(", ", function.Arguments.Select(a => Format(a)))})";
        }

        /// <summary>
        /// Returns a string representation of a given <see cref="SkolemFunctionSymbol"/> instance.
        /// </summary>
        /// <param name="symbol">The symbol to be formatted.</param>
        /// <returns>A string representation of the given symbol.</returns>
        public string Format(SkolemFunctionSymbol symbol) => skolemFunctionLabellingScope.GetLabel(symbol);

        /// <summary>
        /// Returns a string representation of a given <see cref="VariableDeclaration"/> instance.
        /// </summary>
        /// <param name="variableDeclaration">The variable declaration to be formatted.</param>
        /// <returns>A string representation of the given variable declaration.</returns>
        public string Format(VariableDeclaration variableDeclaration)
        {
            return variableDeclaration.Symbol switch
            {
                StandardisedVariableSymbol std => Format(std),
                _ => variableDeclaration.Symbol.ToString() ?? throw new ArgumentException("Cannot format variable declaration because ToString of its symbol returned null", nameof(variableDeclaration))
            };
        }

        /// <summary>
        /// Returns a string representation of a given <see cref="StandardisedVariableSymbol"/> instance.
        /// </summary>
        /// <param name="symbol">The symbol to be formatted.</param>
        /// <returns>A string representation of the given standardised variable symbol.</returns>
        public string Format(StandardisedVariableSymbol symbol) => standardisedVariableLabellingScope.GetLabel(symbol);
    }
}
