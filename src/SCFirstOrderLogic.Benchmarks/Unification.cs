using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.Benchmarks.AlternativeImplementations.Unification;
using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;

namespace SCFirstOrderLogic.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class Unification
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

        private static readonly CNFLiteral JohnKnowsX_Literal = new CNFLiteral(Knows(john, x));
        private static readonly CNFLiteral JohnKnowsJane_Literal = new CNFLiteral(Knows(john, jane));
        private static readonly CNFLiteral YKnowsJane_Literal = new CNFLiteral(Knows(y, jane));
        private static readonly CNFLiteral YKnowsMotherOfY_Literal = new CNFLiteral(Knows(y, Mother(y)));

        [Benchmark]
        public bool Unify1() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

        [Benchmark]
        public bool Unify1Alt() => SentenceUnifier.TryUnify(JohnKnowsX, JohnKnowsJane, out _);

        [Benchmark]
        public SentenceUnifierRaw.Substitution Unify1Raw() => SentenceUnifierRaw.Unify(JohnKnowsX, JohnKnowsJane, null);

        [Benchmark]
        public bool Unify2() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

        [Benchmark]
        public bool Unify2Alt() => SentenceUnifier.TryUnify(JohnKnowsX, YKnowsJane, out _);

        [Benchmark]
        public SentenceUnifierRaw.Substitution Unify2Raw() => SentenceUnifierRaw.Unify(JohnKnowsX, YKnowsJane, null);

        [Benchmark]
        public bool Unify3() => LiteralUnifier.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

        [Benchmark]
        public bool Unify3Alt() => SentenceUnifier.TryUnify(JohnKnowsX, YKnowsMotherOfY, out _);

        [Benchmark]
        public SentenceUnifierRaw.Substitution Unify3Raw() => SentenceUnifierRaw.Unify(JohnKnowsX, YKnowsMotherOfY, null);
    }
}
