using BenchmarkDotNet.Attributes;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation
{
    [MemoryDiagnoser]
    [InProcess]
    public class CNFConstructionBenchmarks
    {
        private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
        private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

        private static Sentence NonTrivialSentence { get; } = ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X))));

        [Benchmark(Baseline = true)]
        public static CNFSentence DoCNFCtor_ProductionVersion() => new(NonTrivialSentence);

        [Benchmark]
        public static AltCNFSentence_WithTypeSwitchCtorVisitors DoCNFCtor_WithTypeSwitch() => new(NonTrivialSentence);
    }
}
