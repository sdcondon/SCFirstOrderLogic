using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic.Inference.Unification
{
    [MemoryDiagnoser]
    [InProcess]
    public class _UnificationBenchmarks
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
        public static bool Unify1_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify1_Sentence() => SentenceUnifier.TryUnify(JohnKnowsX, JohnKnowsJane, out _);

        [Benchmark]
        public static SentenceUnifierRaw.Substitution Unify1_SentenceRaw() => SentenceUnifierRaw.Unify(JohnKnowsX, JohnKnowsJane, null);

        [Benchmark]
        public static bool Unify2_Actual() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify2_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

        [Benchmark]
        public static bool Unify2_Sentence() => SentenceUnifier.TryUnify(JohnKnowsX, YKnowsJane, out _);

        [Benchmark]
        public static SentenceUnifierRaw.Substitution Unify2_SentenceRaw() => SentenceUnifierRaw.Unify(JohnKnowsX, YKnowsJane, null);

        [Benchmark]
        public static bool Unify3_Actual() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

        [Benchmark]
        public static bool Unify3_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

        [Benchmark]
        public static bool Unify3_Sentence() => SentenceUnifier.TryUnify(JohnKnowsX, YKnowsMotherOfY, out _);

        [Benchmark]
        public static SentenceUnifierRaw.Substitution Unify3_SentenceRaw() => SentenceUnifierRaw.Unify(JohnKnowsX, YKnowsMotherOfY, null);
    }
}
