using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// Temporary..
    /// Will do while I figure out what I need (formatprovider, ToString implementations in individual classes, ...?).
    /// Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed), too.
    /// </summary>
    public class SentenceFormatter
    {
        private readonly IEnumerator<string> standardisedVariableLabels;
        private readonly Dictionary<StandardisedVariableSymbol, string> labelsByStandardisedVariableSymbol = new();
        private readonly IEnumerator<string> skolemFunctionLabels;
        private readonly Dictionary<SkolemFunctionSymbol, string> labelsBySkolemFunctionSymbol = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceFormatter"/> class that uses the <see cref="DefaultStandardisedVariableLabelSet"/>
        /// and <see cref="DefaultSkolemFunctionLabelSet"/>.
        /// </summary>
        public SentenceFormatter()
            : this(DefaultStandardisedVariableLabelSet, DefaultSkolemFunctionLabelSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceFormatter"/> class.
        /// </summary>
        /// <param name="standardisedVariableLabels">The set of labels to use for standardised variables.</param>
        /// <param name="skolemFunctionLabels">The set of labels to use for Skolem functions.</param>
        public SentenceFormatter(IEnumerable<string> standardisedVariableLabels, IEnumerable<string> skolemFunctionLabels)
        {
            this.standardisedVariableLabels = standardisedVariableLabels.GetEnumerator();
            this.skolemFunctionLabels = skolemFunctionLabels.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the default label set to use for standardised variables. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to the (lower-case) Greek alphabet.
        /// </summary>
        public static IEnumerable<string> DefaultStandardisedVariableLabelSet { get; set; } = LabelSets.LowerGreekAlphabet;

        /// <summary>
        /// Gets or sets the default label set to use for Skolem functions. Used by
        /// <see cref="SentenceFormatter"/> instances constructed with the parameterless constructor.
        /// Defaults to the (upper-case) modern Latin alphabet;
        /// </summary>
        public static IEnumerable<string> DefaultSkolemFunctionLabelSet { get; set; } = LabelSets.UpperModernLatinAlphabet;

        /// <summary>
        /// Returns a string representation of a given <see cref="CNFClause"/> instance.
        /// </summary>
        /// <param name="clause">The clause to be formatted.</param>
        /// <returns>A string representation of the given clause.</returns>
        public string Format(CNFClause clause) => string.Join(" ∨ ", clause.Literals.Select(l => Format(l)));

        /// <summary>
        /// Returns a string representation of a given <see cref="CNFLiteral"/> instance.
        /// </summary>
        /// <param name="literal">The literal to be formatted.</param>
        /// <returns>A string representation of the given literal.</returns>
        public string Format(CNFLiteral literal) => $"{(literal.IsNegated ? "¬" : "")}{Format(literal.Predicate)}";

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
        private string Format(Conjunction conjunction) =>
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
        public string Format(SkolemFunctionSymbol symbol)
        {
            if (labelsBySkolemFunctionSymbol.TryGetValue(symbol, out var label))
            {
                return label;
            }
            else if (skolemFunctionLabels.MoveNext())
            {
                return labelsBySkolemFunctionSymbol[symbol] = skolemFunctionLabels.Current;
            }
            else
            {
                // Suppose we *could* fall back on the ToString of the underlying variable symbol here.
                // But obviously then we lose the unique representation guarentee, and it should be relatively
                // easy to use essentially infinite label sets - so I'd rather just fail.
                throw new InvalidOperationException("Skolem function label set is exhausted");
                // Should come back to this at some point though. Difficult to use a common formatter
                // when e.g. debugging.
                // asterisk supposed to represent some kind of puff of smoke for the existential instantiation..
                ////return $"*{symbol.StandardisedVariableSymbol.OriginalSymbol}";
                // .. Or ILabeller instead of IEnumerable<string>?
            }
        }

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
        public string Format(StandardisedVariableSymbol symbol)
        {
            if (labelsByStandardisedVariableSymbol.TryGetValue(symbol, out var label))
            {
                return label;
            }
            else if (standardisedVariableLabels.MoveNext())
            {
                return labelsByStandardisedVariableSymbol[symbol] = standardisedVariableLabels.Current;
            }
            else
            {
                // Suppose we *could* fall back on the ToString of the underlying variable symbol here.
                // But obviously then we lose the unique representation guarantee, and it should be relatively
                // easy to use essentially infinite label sets - so I'd rather just fail.
                throw new InvalidOperationException("Skolem function label set is exhausted");
                // Should come back to this at some point though. Difficult to use a common formatter
                // when e.g. debugging.
                // double arrow supposed to represent standardising variables "apart".
                ////return $"↔{symbol.OriginalSymbol}";
                // .. Or ILabeller instead of IEnumerable<string>?
            }
        }
    }
}
