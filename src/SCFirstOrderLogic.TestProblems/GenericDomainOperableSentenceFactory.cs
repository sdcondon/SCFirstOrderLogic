using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.TestProblems;

/// <summary>
/// <para>
/// Identical to <see cref="SentenceCreation.OperableSentenceFactory"/>, except that the A-Z variable declaration properties are replaced by:
/// </para>
/// <list type="bullet">
/// <item>Properties 'A', 'B', 'C' &amp; 'D': zero-arity functions (i.e. constants).</item>
/// <item>Properties 'U', 'V', 'W', 'X', 'Y' &amp; 'Z': variable declarations</item>
/// <item>Methods 'F', 'G' &amp; 'H': functions (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// <item>Methods 'P', 'Q' &amp; 'R': predicates (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// </list>
/// </summary>
public static class GenericDomainOperableSentenceFactory
{
    public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration, OperableSentence sentence) => 
        SentenceCreation.OperableSentenceFactory.ForAll(variableDeclaration, sentence);

    public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableSentence sentence) =>
        SentenceCreation.OperableSentenceFactory.ForAll(variableDeclaration1, variableDeclaration2, sentence);

    public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableSentence sentence) =>
        SentenceCreation.OperableSentenceFactory.ForAll(variableDeclaration1, variableDeclaration2, variableDeclaration3, sentence);

    public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration, OperableSentence sentence) =>
        SentenceCreation.OperableSentenceFactory.ThereExists(variableDeclaration, sentence);

    public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableSentence sentence) =>
        SentenceCreation.OperableSentenceFactory.ThereExists(variableDeclaration1, variableDeclaration2, sentence);

    public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableSentence sentence) =>
        SentenceCreation.OperableSentenceFactory.ThereExists(variableDeclaration1, variableDeclaration2, variableDeclaration3, sentence);

    public static OperableSentence If(OperableSentence antecedent, OperableSentence consequent) =>
        SentenceCreation.OperableSentenceFactory.If(antecedent, consequent);

    public static OperableSentence Iff(OperableSentence left, OperableSentence right) =>
        SentenceCreation.OperableSentenceFactory.Iff(left, right);

    public static OperableSentence AreEqual(OperableTerm left, OperableTerm right) =>
        SentenceCreation.OperableSentenceFactory.AreEqual(left, right);

    public static OperableVariableDeclaration Var(object identifier) =>
        SentenceCreation.OperableSentenceFactory.Var(identifier);

    public static OperableFunction A { get; } = new Function(nameof(A));
    public static OperableFunction B { get; } = new Function(nameof(B));
    public static OperableFunction C { get; } = new Function(nameof(C));
    public static OperableFunction D { get; } = new Function(nameof(D));

    public static OperableVariableDeclaration U { get; } = new VariableDeclaration(nameof(U));
    public static OperableVariableDeclaration V { get; } = new VariableDeclaration(nameof(V));
    public static OperableVariableDeclaration W { get; } = new VariableDeclaration(nameof(W));
    public static OperableVariableDeclaration X { get; } = new VariableDeclaration(nameof(X));
    public static OperableVariableDeclaration Y { get; } = new VariableDeclaration(nameof(Y));
    public static OperableVariableDeclaration Z { get; } = new VariableDeclaration(nameof(Z));

    public static OperableFunction F(params Term[] args) => new Function(nameof(F), args);
    public static OperableFunction G(params Term[] args) => new Function(nameof(G), args);
    public static OperableFunction H(params Term[] args) => new Function(nameof(H), args);

    public static OperablePredicate P(params Term[] args) => new Predicate(nameof(P), args);
    public static OperablePredicate Q(params Term[] args) => new Predicate(nameof(Q), args);
    public static OperablePredicate R(params Term[] args) => new Predicate(nameof(R), args);
}
