using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation;

[MemoryDiagnoser]
[InProcess]
public class RecursiveSentenceVisitorBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Formula NonTrivialSentence { get; } = ForAll(X, If(
            ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
            ThereExists(Y, Loves(Y, X))));

    public record TestCase(string Label, Formula Sentence)
    {
        public override string ToString() => Label;
    }

    public static IEnumerable<TestCase> TestCases { get; } = new TestCase[]
    {
        new(
            Label: "Non-Trivial Sentence",
            Sentence: NonTrivialSentence),
    };

    [ParamsSource(nameof(TestCases))]
    public TestCase? CurrentTestCase { get; set; }

    [Benchmark(Baseline = true)]
    public void CurrentImpl() => new NullVisitor().Visit(CurrentTestCase!.Sentence);

    [Benchmark]
    public void Enumerators() => new NullVisitor_Enumerators().Visit(CurrentTestCase!.Sentence);

    private class NullVisitor : RecursiveFormulaVisitor
    {
    }

    private class NullVisitor_Enumerators : RecursiveSentenceVisitor_Enumerators
    {
    }
}
