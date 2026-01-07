// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaCreation;

/// <summary>
/// <para>
/// Shorthand static factory methods for <see cref="Formula"/> instances. Intended to be used with a 'using static' directive to make method invocations acceptably succinct.
/// </para>
/// <para>
/// For domain-specific formula elements (i.e. predicates &amp; functions), the recommendation is to create appropriate methods and properties to create them, thus avoiding repetition of identifiers. For example:
/// <code>Predicate MyBinaryPredicate(Term arg1, Term arg2) => new Predicate(nameof(MyBinaryPredicate), arg1, arg2);</code>
/// ..which means that (if you include an appropriate 'using static' directive) you can then write things like:
/// <code>ForAll(X, ThereExists(Y, MyBinaryPredicate(X, Y)));</code>
/// See the <see cref="Linq.LinqFormulaFactory"/> class for an alternative method of creating formulas (from lambda expressions acting on interfaces representing the domain and entities therein).
/// </para>
/// </summary>
public static class FormulaFactory
{
    /// <summary>
    /// Shorthand factory method for a new <see cref="UniversalQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration, Formula formula) =>
        new UniversalQuantification(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="UniversalQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Formula formula) =>
        new UniversalQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, formula));

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="UniversalQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Formula formula) =>
        new UniversalQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, new UniversalQuantification(variableDeclaration3, formula)));

    /// <summary>
    /// Shorthand factory method for a new <see cref="ExistentialQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration, Formula formula) =>
        new ExistentialQuantification(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="ExistentialQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Formula formula) =>
        new ExistentialQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, formula));

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="ExistentialQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Formula formula) =>
        new ExistentialQuantification(variableDeclaration1, new ExistentialQuantification(variableDeclaration2, new ExistentialQuantification(variableDeclaration3, formula)));

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="Conjunction"/>(s) of two (or more) operands.
    /// </summary>
    /// <param name="operand1">The first operand of the conjunction.</param>
    /// <param name="operand2">The second operand of the conjunction.</param>
    /// <param name="otherOperands">Any additional operands.</param>
    /// <returns>A new <see cref="Conjunction"/> instance.</returns>
    public static Formula And(Formula operand1, Formula operand2, params Formula[] otherOperands)
    {
        var conjunction = new Conjunction(operand1, operand2);

        foreach (var operand in otherOperands)
        {
            conjunction = new Conjunction(conjunction, operand);
        }

        return conjunction;
    }

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="Disjunction"/>(s) of two (or more) operands.
    /// </summary>
    /// <param name="operand1">The first operand of the disjunction.</param>
    /// <param name="operand2">The second operand of the disjunction.</param>
    /// <param name="otherOperands">Any additional operands.</param>
    /// <returns>A new <see cref="Disjunction"/> instance.</returns>
    public static Formula Or(Formula operand1, Formula operand2, params Formula[] otherOperands)
    {
        var disjunction = new Disjunction(operand1, operand2);

        foreach (var operand in otherOperands)
        {
            disjunction = new Disjunction(disjunction, operand);
        }

        return disjunction;
    }

    /// <summary>
    /// Shorthand factory method for a new <see cref="Implication"/> instance.
    /// </summary>
    /// <param name="antecedent">The antecedent formula of the implication.</param>
    /// <param name="consequent">The consequent formula of the implication.</param>
    /// <returns>A new <see cref="Implication"/> instance.</returns>
    public static Formula If(Formula antecedent, Formula consequent) =>
        new Implication(antecedent, consequent);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Equivalence"/> instance.
    /// </summary>
    /// <param name="left">The left-hand operand of the equivalence.</param>
    /// <param name="right">The right-hand operand of the equivalence.</param>
    /// <returns>A new <see cref="Equivalence"/> instance.</returns>
    public static Formula Iff(Formula left, Formula right) =>
        new Equivalence(left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Negation"/> instance.
    /// </summary>
    /// <param name="formula">The negated formula.</param>
    /// <returns>A new <see cref="Negation"/> instance.</returns>
    public static Formula Not(Formula formula) =>
        new Negation(formula);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Predicate"/> instance with the <see cref="EqualityIdentifier.Instance"/> identifier.
    /// </summary>
    /// <param name="left">The left-hand operand of the equality.</param>
    /// <param name="right">The right-hand operand of the equality.</param>
    /// <returns>A new <see cref="Predicate"/> instance.</returns>
    public static Formula AreEqual(Term left, Term right) =>
        new Predicate(EqualityIdentifier.Instance, left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="VariableDeclaration"/> instance.
    /// </summary>
    /// <param name="identifier">The identifier of the variable.</param>>
    /// <returns>A new <see cref="VariableDeclaration"/> instance.</returns>
    public static VariableDeclaration Var(object identifier) => new(identifier);

    #region VariableDeclarations

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "A".
    /// </summary>
    public static VariableDeclaration A { get; } = new(nameof(A));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "B".
    /// </summary>
    public static VariableDeclaration B { get; } = new(nameof(B));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "C".
    /// </summary>
    public static VariableDeclaration C { get; } = new(nameof(C));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "D".
    /// </summary>
    public static VariableDeclaration D { get; } = new(nameof(D));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "E".
    /// </summary>
    public static VariableDeclaration E { get; } = new(nameof(E));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "F".
    /// </summary>
    public static VariableDeclaration F { get; } = new(nameof(F));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "G".
    /// </summary>
    public static VariableDeclaration G { get; } = new(nameof(G));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "H".
    /// </summary>
    public static VariableDeclaration H { get; } = new(nameof(H));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "I".
    /// </summary>
    public static VariableDeclaration I { get; } = new(nameof(I));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "J".
    /// </summary>
    public static VariableDeclaration J { get; } = new(nameof(J));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "K".
    /// </summary>
    public static VariableDeclaration K { get; } = new(nameof(K));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "L".
    /// </summary>
    public static VariableDeclaration L { get; } = new(nameof(L));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "M".
    /// </summary>
    public static VariableDeclaration M { get; } = new(nameof(M));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "N".
    /// </summary>
    public static VariableDeclaration N { get; } = new(nameof(N));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "O".
    /// </summary>
    public static VariableDeclaration O { get; } = new(nameof(O));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "P".
    /// </summary>
    public static VariableDeclaration P { get; } = new(nameof(P));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "Q".
    /// </summary>
    public static VariableDeclaration Q { get; } = new(nameof(Q));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "R".
    /// </summary>
    public static VariableDeclaration R { get; } = new(nameof(R));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "S".
    /// </summary>
    public static VariableDeclaration S { get; } = new(nameof(S));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "T".
    /// </summary>
    public static VariableDeclaration T { get; } = new(nameof(T));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "U".
    /// </summary>
    public static VariableDeclaration U { get; } = new(nameof(U));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "V".
    /// </summary>
    public static VariableDeclaration V { get; } = new(nameof(V));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "W".
    /// </summary>
    public static VariableDeclaration W { get; } = new(nameof(W));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "X".
    /// </summary>
    public static VariableDeclaration X { get; } = new(nameof(X));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "Y".
    /// </summary>
    public static VariableDeclaration Y { get; } = new(nameof(Y));

    /// <summary>
    /// Gets a <see cref="VariableDeclaration"/> for a variable with the identifier "Z".
    /// </summary>
    public static VariableDeclaration Z { get; } = new(nameof(Z));

    #endregion
}
