using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation;

[MemoryDiagnoser]
[InProcess]
public class RecursiveFormulaVisitorBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Formula NonTrivialFormula { get; } = ForAll(X, If(
            ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
            ThereExists(Y, Loves(Y, X))));

    public record TestCase(string Label, Formula Formula)
    {
        public override string ToString() => Label;
    }

    public static IEnumerable<TestCase> TestCases { get; } =
    [
        new(
            Label: "Non-Trivial Formula",
            Formula: NonTrivialFormula),
    ];

    [ParamsSource(nameof(TestCases))]
    public TestCase? CurrentTestCase { get; set; }

    [Benchmark(Baseline = true)]
    public void CurrentImpl() => new NullVisitor().Visit(CurrentTestCase!.Formula);

    [Benchmark]
    public void Enumerators() => new NullVisitor_Enumerators().Visit(CurrentTestCase!.Formula);

    private class NullVisitor : RecursiveFormulaVisitor
    {
    }

    private class NullVisitor_Enumerators : RecursiveFormulaVisitor_Enumerators
    {
    }
}
