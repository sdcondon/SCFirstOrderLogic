using static SCFirstOrderLogic.FormulaCreation.OperableFormulaFactory;

namespace SCFirstOrderLogic.FormulaCreation.Specialised;

/// <summary>
/// <para>
/// Specialised formula factory intended for use with very simple generic domains, such as those found in tests and basic examples.
/// Identical to <see cref="OperableFormulaFactory"/>, except that the A-Z variable declaration properties are replaced by:
/// </para>
/// <list type="bullet">
/// <item>Properties 'A', 'B', 'C' &amp; 'D': zero-arity functions (i.e. constants)</item>
/// <item>Properties 'U', 'V', 'W', 'X', 'Y' &amp; 'Z': variable declarations</item>
/// <item>Methods 'F', 'G', 'H' &amp; 'I': functions (with arbitrary arity, via a Term[]-valued params parameter)</item>
/// <item>Methods 'P', 'Q', 'R' &amp; 'S': predicates (with arbitrary arity, via a Term[]-valued params parameter)</item>
/// </list>
/// </summary>
public static class GenericDomainOperableFormulaFactory
{
    /// <summary>
    /// Shorthand factory method for a new <see cref="OperableUniversalQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
    public static OperableFormula ForAll(OperableVariableDeclaration variableDeclaration, OperableFormula formula) => 
        OperableFormulaFactory.ForAll(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="OperableUniversalQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
    public static OperableFormula ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableFormula formula) =>
        OperableFormulaFactory.ForAll(variableDeclaration1, variableDeclaration2, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="OperableUniversalQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
    public static OperableFormula ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableFormula formula) =>
        OperableFormulaFactory.ForAll(variableDeclaration1, variableDeclaration2, variableDeclaration3, formula);

    /// <summary>
    /// Shorthand factory method for a new <see cref="OperableExistentialQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
    public static OperableFormula ThereExists(OperableVariableDeclaration variableDeclaration, OperableFormula formula) =>
        OperableFormulaFactory.ThereExists(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="OperableExistentialQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
    public static OperableFormula ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableFormula formula) =>
        OperableFormulaFactory.ThereExists(variableDeclaration1, variableDeclaration2, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="OperableExistentialQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
    public static OperableFormula ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableFormula formula) =>
        OperableFormulaFactory.ThereExists(variableDeclaration1, variableDeclaration2, variableDeclaration3, formula);

    /// <summary>
    /// Shorthand factory method for a new <see cref="OperableImplication"/> instance.
    /// </summary>
    /// <param name="antecedent">The antecedent formula of the implication.</param>
    /// <param name="consequent">The consequent formula of the implication.</param>
    /// <returns>A new <see cref="OperableImplication"/> instance.</returns>
    public static OperableFormula If(OperableFormula antecedent, OperableFormula consequent) =>
        OperableFormulaFactory.If(antecedent, consequent);

    /// <summary>
    /// Shorthand factory method for a new <see cref="OperableEquivalence"/> instance.
    /// </summary>
    /// <param name="left">The left-hand operand of the equivalence.</param>
    /// <param name="right">The right-hand operand of the equivalence.</param>
    /// <returns>A new <see cref="OperableEquivalence"/> instance.</returns>
    public static OperableFormula Iff(OperableFormula left, OperableFormula right) =>
        OperableFormulaFactory.Iff(left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="OperablePredicate"/> instance with the <see cref="EqualityIdentifier.Instance"/> identifier.
    /// </summary>
    /// <param name="left">The left-hand operand of the equality.</param>
    /// <param name="right">The right-hand operand of the equality.</param>
    /// <returns>A new <see cref="OperablePredicate"/> instance.</returns>
    public static OperableFormula AreEqual(OperableTerm left, OperableTerm right) =>
        OperableFormulaFactory.AreEqual(left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="OperableVariableDeclaration"/> instance.
    /// </summary>
    /// <param name="identifier">The identifier of the variable.</param>>
    /// <returns>A new <see cref="OperableVariableDeclaration"/> instance.</returns>
    public static OperableVariableDeclaration Var(object identifier) =>
        OperableFormulaFactory.Var(identifier);

    /// <summary>
    /// Gets a zero-arity <see cref="OperableFunction"/> with the identifier "A".
    /// </summary>
    public static OperableFunction A { get; } = new Function(nameof(A));
    /// <summary>
    /// Gets a zero-arity <see cref="OperableFunction"/> with the identifier "B".
    /// </summary>
    public static OperableFunction B { get; } = new Function(nameof(B));
    /// <summary>
    /// Gets a zero-arity <see cref="OperableFunction"/> with the identifier "C".
    /// </summary>
    public static OperableFunction C { get; } = new Function(nameof(C));
    /// <summary>
    /// Gets a zero-arity <see cref="OperableFunction"/> with the identifier "D".
    /// </summary>
    public static OperableFunction D { get; } = new Function(nameof(D));

    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "U".
    /// </summary>
    public static OperableVariableDeclaration U { get; } = new VariableDeclaration(nameof(U));
    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "V".
    /// </summary>
    public static OperableVariableDeclaration V { get; } = new VariableDeclaration(nameof(V));
    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "W".
    /// </summary>
    public static OperableVariableDeclaration W { get; } = new VariableDeclaration(nameof(W));
    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "X".
    /// </summary>
    public static OperableVariableDeclaration X { get; } = new VariableDeclaration(nameof(X));
    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "Y".
    /// </summary>
    public static OperableVariableDeclaration Y { get; } = new VariableDeclaration(nameof(Y));
    /// <summary>
    /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the identifier "Z".
    /// </summary>
    public static OperableVariableDeclaration Z { get; } = new VariableDeclaration(nameof(Z));

    /// <summary>
    /// Returns a new <see cref="OperableFunction"/> instance with the identifier "F".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="OperableFunction"/> instance with the identifier "F".</returns>
    public static OperableFunction F(params Term[] args) => new Function(nameof(F), args);
    /// <summary>
    /// Returns a new <see cref="OperableFunction"/> instance with the identifier "G".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="OperableFunction"/> instance with the identifier "G".</returns>
    public static OperableFunction G(params Term[] args) => new Function(nameof(G), args);
    /// <summary>
    /// Returns a new <see cref="OperableFunction"/> instance with the identifier "H".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="OperableFunction"/> instance with the identifier "H".</returns>
    public static OperableFunction H(params Term[] args) => new Function(nameof(H), args);
    /// <summary>
    /// Returns a new <see cref="OperableFunction"/> instance with the identifier "I".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="OperableFunction"/> instance with the identifier "I".</returns>
    public static OperableFunction I(params Term[] args) => new Function(nameof(I), args);

    /// <summary>
    /// Returns a new <see cref="OperablePredicate"/> instance with the identifier "P".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="OperablePredicate"/> instance with the identifier "P".</returns>
    public static OperablePredicate P(params Term[] args) => new Predicate(nameof(P), args);
    /// <summary>
    /// Returns a new <see cref="OperablePredicate"/> instance with the identifier "Q".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="OperablePredicate"/> instance with the identifier "Q".</returns>
    public static OperablePredicate Q(params Term[] args) => new Predicate(nameof(Q), args);
    /// <summary>
    /// Returns a new <see cref="OperablePredicate"/> instance with the identifier "R".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="OperablePredicate"/> instance with the identifier "R".</returns>
    public static OperablePredicate R(params Term[] args) => new Predicate(nameof(R), args);
    /// <summary>
    /// Returns a new <see cref="OperablePredicate"/> instance with the identifier "S".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="OperablePredicate"/> instance with the identifier "S".</returns>
    public static OperablePredicate S(params Term[] args) => new Predicate(nameof(S), args);
}
