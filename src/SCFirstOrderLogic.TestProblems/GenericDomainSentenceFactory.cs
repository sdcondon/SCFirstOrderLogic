namespace SCFirstOrderLogic.TestProblems;

/// <summary>
/// <para>
/// Identical to <see cref="SentenceCreation.SentenceFactory"/>, except that the A-Z variable declaration properties are replaced by:
/// </para>
/// <list type="bullet">
/// <item>Properties 'A', 'B', 'C' &amp; 'D': zero-arity functions (i.e. constants).</item>
/// <item>Properties 'U', 'V', 'W', 'X', 'Y' &amp; 'Z': variable declarations</item>
/// <item>Methods 'F', 'G' &amp; 'H': functions (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// <item>Methods 'P', 'Q' &amp; 'R': predicates (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// </list>
/// </summary>
public static class GenericDomainSentenceFactory
{
    public static Sentence ForAll(VariableDeclaration variableDeclaration, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ForAll(variableDeclaration, sentence);

    public static Sentence ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ForAll(variableDeclaration1, variableDeclaration2, sentence);

    public static Sentence ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ForAll(variableDeclaration1, variableDeclaration2, variableDeclaration3, sentence);

    public static Sentence ThereExists(VariableDeclaration variableDeclaration, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ThereExists(variableDeclaration, sentence);

    public static Sentence ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ThereExists(variableDeclaration1, variableDeclaration2, sentence);

    public static Sentence ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Sentence sentence) =>
        SentenceCreation.SentenceFactory.ThereExists(variableDeclaration1, variableDeclaration2, variableDeclaration3, sentence);

    public static Sentence If(Sentence antecedent, Sentence consequent) =>
        SentenceCreation.SentenceFactory.If(antecedent, consequent);

    public static Sentence Iff(Sentence left, Sentence right) =>
        SentenceCreation.SentenceFactory.Iff(left, right);

    public static Sentence AreEqual(Term left, Term right) =>
        SentenceCreation.SentenceFactory.AreEqual(left, right);

    public static VariableDeclaration Var(object identifier) =>
        SentenceCreation.SentenceFactory.Var(identifier);

    public static Function A { get; } = new(nameof(A));
    public static Function B { get; } = new(nameof(B));
    public static Function C { get; } = new(nameof(C));
    public static Function D { get; } = new(nameof(D));

    public static VariableDeclaration U { get; } = new(nameof(U));
    public static VariableDeclaration V { get; } = new(nameof(V));
    public static VariableDeclaration W { get; } = new(nameof(W));
    public static VariableDeclaration X { get; } = new(nameof(X));
    public static VariableDeclaration Y { get; } = new(nameof(Y));
    public static VariableDeclaration Z { get; } = new(nameof(Z));

    public static Function F(params Term[] args) => new(nameof(F), args);
    public static Function G(params Term[] args) => new(nameof(G), args);
    public static Function H(params Term[] args) => new(nameof(H), args);

    public static Predicate P(params Term[] args) => new(nameof(P), args);
    public static Predicate Q(params Term[] args) => new(nameof(Q), args);
    public static Predicate R(params Term[] args) => new(nameof(R), args);
}
