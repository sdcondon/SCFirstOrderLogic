using BenchmarkDotNet.Attributes;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

[MemoryDiagnoser]
[InProcess]
public class CNFConversionBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Formula NonTrivialSentence { get; } = ForAll(X, If(
            ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
            ThereExists(Y, Loves(Y, X))));

    [Benchmark(Baseline = true)]
    public static Formula DoCNFConversion_ProductionVersion() => CNFConversion.ApplyTo(NonTrivialSentence);

    [Benchmark]
    public static Formula DoCNFConversion_WithoutTypeSwitch() => CNFConversion_WithoutTypeSwitch.ApplyTo(NonTrivialSentence);
}
