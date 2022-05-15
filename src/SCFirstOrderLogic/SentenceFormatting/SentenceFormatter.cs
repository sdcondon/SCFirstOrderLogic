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
        private readonly Dictionary<StandardisedVariableSymbol, string> labelsByStandardisedVariableSymbol = new Dictionary<StandardisedVariableSymbol, string>();
        private readonly IEnumerator<string> skolemFunctionLabels;
        private readonly Dictionary<SkolemFunctionSymbol, string> labelsBySkolemFunctionSymbol = new Dictionary<SkolemFunctionSymbol, string>();

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
        /// Gets or sets the default label set to use for standardised variables.
        /// </summary>
        public static IEnumerable<string> DefaultStandardisedVariableLabelSet { get; set; } = LabelSets.LowerGreekAlphabet;

        /// <summary>
        /// Gets or sets the default label set to use for Skolem functions.
        /// </summary>
        public static IEnumerable<string> DefaultSkolemFunctionLabelSet { get; set; } = LabelSets.UpperModernLatinAlphabet;

        /// <summary>
        /// Gets the mapping from standardised variables to labels used by this formatter.
        /// </summary>
        public IReadOnlyDictionary<StandardisedVariableSymbol, string> LabelsByStandardisedVariableSymbol => labelsByStandardisedVariableSymbol;

        /// <summary>
        /// Gets the mapping from Skolem functions to labels used by this formatter.
        /// </summary>
        public IReadOnlyDictionary<SkolemFunctionSymbol, string> LabelsBySkolemFunctionSymbol => labelsBySkolemFunctionSymbol;

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
            $"∃ {Print(existentialQuantification.Variable)}, {Print(existentialQuantification.Sentence)}";

        public string Print(Implication implication) =>
            $"[{Print(implication.Antecedent)} ⇒ {Print(implication.Consequent)}]";

        public string Print(Negation negation) =>
            $"¬{Print(negation.Sentence)}";

        public string Print(Predicate predicate) =>
            $"{predicate.Symbol}({string.Join(", ", predicate.Arguments.Select(a => Print(a)))})";

        public string Print(UniversalQuantification universalQuantification) =>
            $"∀ {Print(universalQuantification.Variable)}, {Print(universalQuantification.Sentence)}";

        public string Print(Term term) => term switch
        {
            Constant constant => Print(constant),
            VariableReference variable => Print(variable),
            Function function => Print(function),
            _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'")
        };

        public string Print(Constant constant) =>
            constant.Symbol.ToString();

        public string Print(VariableReference variable) =>
            Print(variable.Declaration);

        public string Print(Function function)
        {
            return function.Symbol switch
            {
                SkolemFunctionSymbol skm => $"{GetOrCreateSkolemFunctionLabel(skm)}({string.Join(", ", function.Arguments.Select(a => Print(a)))})",
                _ => $"{function.Symbol}({string.Join(", ", function.Arguments.Select(a => Print(a)))})"
            };
        }

        public string GetOrCreateSkolemFunctionLabel(SkolemFunctionSymbol symbol)
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
            }
        }

        public string Print(VariableDeclaration variableDeclaration)
        {
            return variableDeclaration.Symbol switch
            {
                StandardisedVariableSymbol std => GetOrCreateStandardisedVariableLabel(std),
                _ => variableDeclaration.Symbol.ToString()
            };
        }

        public string GetOrCreateStandardisedVariableLabel(StandardisedVariableSymbol symbol)
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
                // But obviously then we lose the unique representation guarentee, and it should be relatively
                // easy to use essentially infinite label sets - so I'd rather just fail.
                throw new InvalidOperationException("Skolem function label set is exhausted");
            }
        }
    }
}
