using System;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    /// <summary>
    /// Just for now..
    /// Will do while I figure out what I need (format provider et all, ToString implementations in inidividual classes, ...?).
    /// Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed), too.
    /// </summary>
    internal static class SentencePrinter
    {
        public static string Print(this Sentence sentence)
        {
            return sentence switch
            {
                Conjunction conjunction => Print(conjunction),
                Disjunction disjunction => Print(disjunction),
                Equality equality => Print(equality),
                Equivalence equivalence => Print(equivalence),
                Implication implication => Print(implication),
                Negation negation => Print(negation),
                Predicate predicate => Print(predicate),
                Quantification quantification => Print(quantification),
                _ => throw new ArgumentException("Unsupported sentence type")
            };
        }

        private static string Print(Conjunction conjunction) => $"({Print(conjunction.Left)} ∧ {Print(conjunction.Right)})";

        public static string Print(Disjunction disjunction) => $"({Print(disjunction.Left)} ∨ {Print(disjunction.Right)})";

        public static string Print(Equality equality) => $"({Print(equality.Left)} = {Print(equality.Right)})";

        public static string Print(Equivalence equivalence) => $"({Print(equivalence.Left)} ⇔ {Print(equivalence.Right)})";

        public static string Print(ExistentialQuantification existentialQuantification) => $"∃ {Print(existentialQuantification.Variable)}, {Print(existentialQuantification.Sentence)}";

        public static string Print(Implication implication) => $"({Print(implication.Antecedent)} ⇒ {Print(implication.Consequent)})";

        public static string Print(MemberPredicate predicate) => $"{predicate.Member.Name}({string.Join(", ", predicate.Arguments.Select(a => Print(a)))})";

        public static string Print(Negation negation) => $"¬{Print(negation.Sentence)}";

        public static string Print(Predicate predicate)
        {
            return predicate switch
            {
                MemberPredicate memberPredicate => Print(memberPredicate),
                _ => throw new ArgumentException()
            };
        }

        public static string Print(Quantification quantification)
        {
            return quantification switch
            {
                ExistentialQuantification existentialQuantification => Print(existentialQuantification),
                UniversalQuantification universalQuantification => Print(universalQuantification),
                _ => throw new ArgumentException()
            };
        }

        public static string Print(UniversalQuantification universalQuantification) => $"∀ {Print(universalQuantification.Variable)}, {Print(universalQuantification.Sentence)}";

        public static string Print(Term term)
        {
            return term switch
            {
                Constant constant => Print(constant),
                Variable variable => Print(variable),
                Function function => Print(function),
                _ => throw new ArgumentException()
            };
        }

        public static string Print(Constant constant)
        {
            return constant switch
            {
                MemberConstant memberConstant => Print(memberConstant),
                _ => throw new ArgumentException()
            };
        }

        public static string Print(MemberConstant constant) => constant.Member.Name;

        public static string Print(Variable variable) => Print(variable.Declaration);

        public static string Print(Function function)
        {
            return function switch
            {
                MemberFunction domainFunction => Print(domainFunction),
                SkolemFunction skolemFunction => Print(skolemFunction),
                _ => throw new ArgumentException()
            };
        }

        public static string Print(MemberFunction domainFunction) => $"{domainFunction.Member.Name}({string.Join(", ", domainFunction.Arguments.Select(a => Print(a)))})";

        public static string Print(SkolemFunction skolemFunction) => $"{skolemFunction.Label}({string.Join(", ", skolemFunction.Arguments.Select(a => Print(a)))})";

        public static string Print(VariableDeclaration variableDeclaration) => variableDeclaration.Name;
    }
}
