using BenchmarkDotNet.Attributes;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

[MemoryDiagnoser]
[InProcess]
public class UnificationBenchmarks
{
    private static Function Mother(Term child) => new("Mother", child);
    private static Predicate Knows(Term knower, Term known) => new("Knows", knower, known);
    private static readonly Function john = new("John");
    private static readonly Function jane = new("Jane");
    private static readonly VariableDeclaration x = new("x");
    private static readonly VariableDeclaration y = new("y");

    private static readonly Formula JohnKnowsX = Knows(john, x);
    private static readonly Formula JohnKnowsJane = Knows(john, jane);
    private static readonly Formula YKnowsJane = Knows(john, jane);
    private static readonly Formula YKnowsMotherOfY = Knows(john, jane);

    private static readonly Literal JohnKnowsX_Literal = new(Knows(john, x));
    private static readonly Literal JohnKnowsJane_Literal = new(Knows(john, jane));
    private static readonly Literal YKnowsJane_Literal = new(Knows(y, jane));
    private static readonly Literal YKnowsMotherOfY_Literal = new(Knows(y, Mother(y)));

    [Benchmark]
    public static bool Unify1_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify1_OccursCheckTransform() => Unifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify1_Formula() => Unifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, JohnKnowsJane, out _);

    [Benchmark]
    public static Unifier_FromAIaMA.Substitution Unify1_FormulaRaw() => Unifier_FromAIaMA.Unify(JohnKnowsX, JohnKnowsJane, null);

    [Benchmark]
    public static bool Unify2_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify2_OccursCheckTransform() => Unifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify2_Formula() => Unifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsJane, out _);

    [Benchmark]
    public static Unifier_FromAIaMA.Substitution Unify2_FormulaRaw() => Unifier_FromAIaMA.Unify(JohnKnowsX, YKnowsJane, null);

    [Benchmark]
    public static bool Unify3_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

    [Benchmark]
    public static bool Unify3_OccursCheckTransform() => Unifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

    [Benchmark]
    public static bool Unify3_Formula() => Unifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsMotherOfY, out _);

    [Benchmark]
    public static Unifier_FromAIaMA.Substitution Unify3_FormulaRaw() => Unifier_FromAIaMA.Unify(JohnKnowsX, YKnowsMotherOfY, null);
}
