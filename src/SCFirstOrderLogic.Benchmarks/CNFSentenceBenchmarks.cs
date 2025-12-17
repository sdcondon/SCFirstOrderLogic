using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.FormulaManipulation.Normalisation;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic;

[MemoryDiagnoser]
[InProcess]
public class CNFSentenceBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Formula NonTrivialSentence { get; } = ForAll(X, If(
        ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
        ThereExists(Y, Loves(Y, X))));

    // todo: strictly speaking not fair test any more
    [Benchmark(Baseline = true)]
    public static CNFFormula DoCNFCtor_ProductionVersion() => NonTrivialSentence.ToCNF();

    [Benchmark]
    public static CNFSentence_WithTypeSwitchCtorVisitors DoCNFCtor_WithTypeSwitch() => new(NonTrivialSentence);
}
