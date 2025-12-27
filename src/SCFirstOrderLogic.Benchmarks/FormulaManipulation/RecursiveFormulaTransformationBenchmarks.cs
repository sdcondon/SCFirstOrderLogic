using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation;

[MemoryDiagnoser]
[InProcess]
public class RecursiveFormulaTransformationBenchmarks
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Formula NonTrivialFormula { get; } = ForAll(X, If(
            ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
            ThereExists(Y, Loves(Y, X))));

    public record TestCase(string Label, bool DoSomething, Formula Formula)
    {
        public override string ToString() => Label;
    }

    public static IEnumerable<TestCase> TestCases { get; } =
    [
        new(
            Label: "Non-Trivial NO-OP",
            DoSomething: false,
            Formula: NonTrivialFormula),

        new(
            Label: "Non-Trivial ALL-LEAFS-OP",
            DoSomething: true,
            Formula: NonTrivialFormula),
    ];

    [ParamsSource(nameof(TestCases))]
    public TestCase? CurrentTestCase { get; set; }

    [Benchmark(Baseline = true)]
    public Formula CurrentImpl() => new VarTransform(CurrentTestCase!.DoSomething).ApplyTo(CurrentTestCase!.Formula);

    [Benchmark]
    public Formula Linq() => new VarTransform_Linq(CurrentTestCase!.DoSomething).ApplyTo(CurrentTestCase!.Formula);

    [Benchmark]
    public Formula LinqIterateTwice() => new VarTransform_LinqIterateTwice(CurrentTestCase!.DoSomething).ApplyTo(CurrentTestCase!.Formula);

    private class VarTransform_LinqIterateTwice(bool doSomething) : RecursiveFormulaTransformation_LinqIterateTwice
    {
        private readonly bool doSomething = doSomething;

        public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            return doSomething ? new VariableDeclaration("X") : variableDeclaration;
        }
    }

    private class VarTransform_Linq(bool doSomething) : RecursiveFormulaTransformation_Linq
    {
        private readonly bool doSomething = doSomething;

        public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            return doSomething ? new VariableDeclaration("X") : variableDeclaration;
        }
    }

    private class VarTransform(bool doSomething) : RecursiveFormulaTransformation
    {
        private readonly bool doSomething = doSomething;

        public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            return doSomething ? new VariableDeclaration("X") : variableDeclaration;
        }
    }
}
