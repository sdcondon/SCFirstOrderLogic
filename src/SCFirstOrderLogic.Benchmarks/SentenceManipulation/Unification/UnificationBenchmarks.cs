using BenchmarkDotNet.Attributes;

namespace SCFirstOrderLogic.SentenceManipulation.Unification;

[MemoryDiagnoser]
[InProcess]
public class UnificationBenchmarks
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

    private static readonly Literal JohnKnowsX_Literal = new(Knows(john, x));
    private static readonly Literal JohnKnowsJane_Literal = new(Knows(john, jane));
    private static readonly Literal YKnowsJane_Literal = new(Knows(y, jane));
    private static readonly Literal YKnowsMotherOfY_Literal = new(Knows(y, Mother(y)));

    [Benchmark]
    public static bool Unify1_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify1_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, JohnKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify1_Sentence() => LiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, JohnKnowsJane, out _);

    [Benchmark]
    public static LiteralUnifier_FromAIaMA.Substitution Unify1_SentenceRaw() => LiteralUnifier_FromAIaMA.Unify(JohnKnowsX, JohnKnowsJane, null);

    [Benchmark]
    public static bool Unify2_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify2_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsJane_Literal, out _);

    [Benchmark]
    public static bool Unify2_Sentence() => LiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsJane, out _);

    [Benchmark]
    public static LiteralUnifier_FromAIaMA.Substitution Unify2_SentenceRaw() => LiteralUnifier_FromAIaMA.Unify(JohnKnowsX, YKnowsJane, null);

    [Benchmark]
    public static bool Unify3_Actual() => Unifier.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

    [Benchmark]
    public static bool Unify3_OccursCheckTransform() => LiteralUnifier_WithOccursCheckAsTransformation.TryCreate(JohnKnowsX_Literal, YKnowsMotherOfY_Literal, out _);

    [Benchmark]
    public static bool Unify3_Sentence() => LiteralUnifier_OptimisedFromAIaMA.TryUnify(JohnKnowsX, YKnowsMotherOfY, out _);

    [Benchmark]
    public static LiteralUnifier_FromAIaMA.Substitution Unify3_SentenceRaw() => LiteralUnifier_FromAIaMA.Unify(JohnKnowsX, YKnowsMotherOfY, null);
}
