using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Temporary..
    /// Will do while I figure out what I need (formatprovider, ToString implementations in inidividual classes, ...?).
    /// Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed), too.
    /// </summary>
    internal static class SentenceFormatter
    {
        // TODO-BUG: Name uniqueness should really be scoped (by formatter instance?) rather than global.
        // This approach will quickly start throwing index out of range exceptions in any real-world scenario.
        private static readonly string[] GreekAlphabet = new[] { "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο", "π", "ρ", "σ", "τ", "υ", "φ", "χ", "ψ", "ω" };
        private static readonly Dictionary<CNFConversion.StandardisedVariableSymbol, string> StandardisedVariableLabels = new Dictionary<CNFConversion.StandardisedVariableSymbol, string>();

        private static readonly string[] SkolemFunctionAlphabet = new[] { "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private static readonly Dictionary<CNFConversion.SkolemFunctionSymbol, string> SkolemFunctionLabels = new Dictionary<CNFConversion.SkolemFunctionSymbol, string>();

        public static string Print(this Sentence sentence) => sentence switch
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

        private static string Print(Conjunction conjunction) =>
            $"({Print(conjunction.Left)} ∧ {Print(conjunction.Right)})";

        public static string Print(Disjunction disjunction) =>
            $"({Print(disjunction.Left)} ∨ {Print(disjunction.Right)})";

        public static string Print(Equivalence equivalence) =>
            $"({Print(equivalence.Left)} ⇔ {Print(equivalence.Right)})";

        public static string Print(ExistentialQuantification existentialQuantification) =>
            $"∃ {Print(existentialQuantification.Variable)}, {Print(existentialQuantification.Sentence)}";

        public static string Print(Implication implication) =>
            $"({Print(implication.Antecedent)} ⇒ {Print(implication.Consequent)})";

        public static string Print(Negation negation) =>
            $"¬{Print(negation.Sentence)}";

        public static string Print(Predicate predicate) =>
            $"{predicate.Symbol}({string.Join(", ", predicate.Arguments.Select(a => Print(a)))})";

        public static string Print(UniversalQuantification universalQuantification) =>
            $"∀ {Print(universalQuantification.Variable)}, {Print(universalQuantification.Sentence)}";

        public static string Print(Term term) => term switch
        {
            Constant constant => Print(constant),
            VariableReference variable => Print(variable),
            Function function => Print(function),
            _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'")
        };

        public static string Print(Constant constant) =>
            constant.Symbol.ToString();

        public static string Print(VariableReference variable) =>
            Print(variable.Declaration);

        public static string Print(Function function) => function.Symbol switch
        {
            CNFConversion.SkolemFunctionSymbol skm => $"{(SkolemFunctionLabels.ContainsKey(skm) ? SkolemFunctionLabels[skm] : SkolemFunctionLabels[skm] = SkolemFunctionAlphabet[SkolemFunctionLabels.Count])}({string.Join(", ", function.Arguments.Select(a => Print(a)))})",
            _ => $"{function.Symbol}({string.Join(", ", function.Arguments.Select(a => Print(a)))})"
        };

        public static string Print(VariableDeclaration variableDeclaration) => variableDeclaration.Symbol switch
        {
            CNFConversion.StandardisedVariableSymbol std => StandardisedVariableLabels.ContainsKey(std) ? StandardisedVariableLabels[std] : StandardisedVariableLabels[std] = GreekAlphabet[StandardisedVariableLabels.Count],
            _ => variableDeclaration.Symbol.ToString()
        };
    }
}
