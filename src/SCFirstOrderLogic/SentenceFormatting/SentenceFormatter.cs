using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// Temporary..
    /// Will do while I figure out what I need (formatprovider, ToString implementations in inidividual classes, ...?).
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

        public string Print(CNFClause clause) => string.Join(" ∨ ", clause.Literals.Select(l => Print(l)));

        public string Print(CNFLiteral literal) => $"{(literal.IsNegated ? "¬" : "")}{Print(literal.Predicate)}";

        public string Print(Sentence sentence) => sentence switch
        {
            Conjunction conjunction => Print(conjunction),
            Disjunction disjunction => Print(disjunction),
            Equivalence equivalence => Print(equivalence),
            ExistentialQuantification existentialQuantification => Print(existentialQuantification),
            Implication implication => Print(implication),
            Negation negation => Print(negation),
            Predicate predicate => Print(predicate),
            UniversalQuantification universalQuantification => Print(universalQuantification),
            _ => throw new ArgumentException("Unsupported sentence type")
        };

        private string Print(Conjunction conjunction) =>
            $"[{Print(conjunction.Left)} ∧ {Print(conjunction.Right)}]";

        public string Print(Disjunction disjunction) =>
            $"[{Print(disjunction.Left)} ∨ {Print(disjunction.Right)}]";

        public string Print(Equivalence equivalence) =>
            $"[{Print(equivalence.Left)} ⇔ {Print(equivalence.Right)}]";

        public string Print(ExistentialQuantification existentialQuantification) =>
            $"[∃ {Print(existentialQuantification.Variable)}, {Print(existentialQuantification.Sentence)}]";

        public string Print(Implication implication) =>
            $"[{Print(implication.Antecedent)} ⇒ {Print(implication.Consequent)}]";

        public string Print(Negation negation) =>
            $"¬{Print(negation.Sentence)}";

        public string Print(Predicate predicate) =>
            $"{predicate.Symbol}({string.Join(", ", predicate.Arguments.Select(a => Print(a)))})";

        public string Print(UniversalQuantification universalQuantification) =>
            $"[∀ {Print(universalQuantification.Variable)}, {Print(universalQuantification.Sentence)}]";

        public string Print(Term term) => term switch
        {
            Constant constant => Print(constant),
            VariableReference variable => Print(variable),
            Function function => Print(function),
            _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'")
        };

        public string Print(Constant constant) =>
            constant.Symbol.ToString() ?? throw new ArgumentException("Cannot print constant because ToString of its symbol returned null", nameof(constant));

        public string Print(VariableReference variable) =>
            Print(variable.Declaration);

        public string Print(Function function)
        {
            var label = function.Symbol is SkolemFunctionSymbol skm ? Print(skm) : function.Symbol.ToString();
            return $"{label}({string.Join(", ", function.Arguments.Select(a => Print(a)))})";
        }

        public string Print(SkolemFunctionSymbol symbol)
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

        public string Print(VariableDeclaration variableDeclaration)
        {
            return variableDeclaration.Symbol switch
            {
                StandardisedVariableSymbol std => Print(std),
                _ => variableDeclaration.Symbol.ToString() ?? throw new ArgumentException("Cannot print variable declaration because ToString of its symbol returned null", nameof(variableDeclaration))
            };
        }

        public string Print(StandardisedVariableSymbol symbol)
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
