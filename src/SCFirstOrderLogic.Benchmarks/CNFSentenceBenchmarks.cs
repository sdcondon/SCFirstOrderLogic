using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic;

[MemoryDiagnoser]
[InProcess]
public class CNFSentenceBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Sentence NonTrivialSentence { get; } = ForAll(X, If(
        ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
        ThereExists(Y, Loves(Y, X))));

    // todo: strictly speaking not fair test any more
    [Benchmark(Baseline = true)]
    public static CNFSentence DoCNFCtor_ProductionVersion() => NonTrivialSentence.ToCNF();

    [Benchmark]
    public static CNFSentence_WithTypeSwitchCtorVisitors DoCNFCtor_WithTypeSwitch() => new(NonTrivialSentence);
}
