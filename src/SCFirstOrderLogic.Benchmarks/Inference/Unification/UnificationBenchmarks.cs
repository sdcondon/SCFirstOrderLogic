using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic.Inference.Unification
{
    [MemoryDiagnoser]
    [InProcess]
    public class UnificationBenchmarks
    {
        private static Function Mother(Term child) => new("Mother", child);
        private static Predicate Knows(Term knower, Term known) => new("Knows", knower, known);
        private static readonly Constant john = new("John");
        private static readonly Constant jane = new("Jane");
        private static readonly VariableDeclaration x = new("x");
        private static readonly VariableDeclaration y = new("y");

        private static readonly Sentence JohnKnowsX = Knows(john, x);
        private static readonly Sentence JohnKnowsJane = Knows(john, jane);
        private static readonly Sentence YKnowsJane = Knows(john, jane);
        private static readonly Sentence YKnowsMotherOfY = Knows(john, jane);

        private static readonly CNFLiteral JohnKnowsX_Literal = new(Knows(john, x));
        private static readonly CNFLiteral JohnKnowsJane_Literal = new(Knows(john, jane));
        private static readonly CNFLiteral YKnowsJane_Literal = new(Knows(y, jane));
        private static readonly CNFLiteral YKnowsMotherOfY_Literal = new(Knows(y, Mother(y)));

        [Benchmark]
        public static bool Unify1_Actual() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify1_OccursCheckTransform() => AltLiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify1_Sentence() => AltLiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, JohnKnowsJane, out _);

        [Benchmark]
        public static AltLiteralUnifier_FromAIaMA.Substitution Unify1_SentenceRaw() => AltLiteralUnifier_FromAIaMA.Unify(JohnKnowsX, JohnKnowsJane, null);

        [Benchmark]
        public static bool Unify2_Actual() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify2_OccursCheckTransform() => AltLiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify2_Sentence() => AltLiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsJane, out _);

        [Benchmark]
        public static AltLiteralUnifier_FromAIaMA.Substitution Unify2_SentenceRaw() => AltLiteralUnifier_FromAIaMA.Unify(JohnKnowsX, YKnowsJane, null);

        [Benchmark]
        public static bool Unify3_Actual() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

        [Benchmark]
        public static bool Unify3_OccursCheckTransform() => AltLiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

        [Benchmark]
        public static bool Unify3_Sentence() => AltLiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsMotherOfY, out _);

        [Benchmark]
        public static AltLiteralUnifier_FromAIaMA.Substitution Unify3_SentenceRaw() => AltLiteralUnifier_FromAIaMA.Unify(JohnKnowsX, YKnowsMotherOfY, null);
    }
}
