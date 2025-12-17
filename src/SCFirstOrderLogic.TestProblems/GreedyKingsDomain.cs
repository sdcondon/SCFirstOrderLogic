using static SCFirstOrderLogic.FormulaCreation.OperableFormulaFactory;

namespace SCFirstOrderLogic.TestProblems;

/// <summary>
/// Extremely simple domain for basic test cases. Just because it is used for a couple of examples in the source material.
/// </summary>
internal static class GreedyKingsDomain
{
    public static OperableFunction John { get; } = new Function(nameof(John));
    public static OperableFunction Richard { get; } = new Function(nameof(Richard));
    public static OperableFunction Mary { get; } = new Function(nameof(Mary));

    public static OperableFormula AllGreedyAreEvil { get; } = ForAll(X, If(IsGreedy(X), IsEvil(X)));
    public static OperableFormula AllGreedyKingsAreEvil { get; } = ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X)));
    public static OperableFormula AllGreedyQueensAreEvil { get; } = ForAll(X, If(IsQueen(X) & IsGreedy(X), IsEvil(X)));
    public static OperableFormula AllEvilKnowEachOther { get; } = ForAll(X, Y, If(IsEvil(X) & IsEvil(Y), Knows(X, Y)));

    public static OperablePredicate IsKing(OperableTerm term) => new Predicate(nameof(IsKing), term);
    public static OperablePredicate IsQueen(OperableTerm term) => new Predicate(nameof(IsQueen), term);
    public static OperablePredicate IsGreedy(OperableTerm term) => new Predicate(nameof(IsGreedy), term);
    public static OperablePredicate IsEvil(OperableTerm term) => new Predicate(nameof(IsEvil), term);
    public static OperablePredicate Knows(OperableTerm x, OperableTerm y) => new Predicate(nameof(Knows), x, y);
}
