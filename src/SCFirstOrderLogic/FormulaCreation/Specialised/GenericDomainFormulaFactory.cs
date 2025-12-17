namespace SCFirstOrderLogic.FormulaCreation.Specialised;

/// <summary>
/// <para>
/// Specialised formula factory intended for use with very simple generic domains, such as those found in tests and basic examples.
/// Identical to <see cref="FormulaFactory"/>, except that the A-Z variable declaration properties are replaced by:
/// </para>
/// <list type="bullet">
/// <item>Properties 'A', 'B', 'C' &amp; 'D': zero-arity functions (i.e. constants)</item>
/// <item>Properties 'U', 'V', 'W', 'X', 'Y' &amp; 'Z': variable declarations</item>
/// <item>Methods 'F', 'G', 'H' &amp; 'I': functions (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// <item>Methods 'P', 'Q', 'R' &amp; 'S': predicates (with an arbitrary number of args, via a Term[]-valued params parameter)</item>
/// </list>
/// </summary>
public static class GenericDomainFormulaFactory
{
    /// <summary>
    /// Shorthand factory method for a new <see cref="UniversalQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration, Formula formula) =>
        FormulaFactory.ForAll(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="UniversalQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Formula formula) =>
        FormulaFactory.ForAll(variableDeclaration1, variableDeclaration2, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="UniversalQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="UniversalQuantification"/> instance.</returns>
    public static Formula ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Formula formula) =>
        FormulaFactory.ForAll(variableDeclaration1, variableDeclaration2, variableDeclaration3, formula);

    /// <summary>
    /// Shorthand factory method for a new <see cref="ExistentialQuantification"/> instance.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variable.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration, Formula formula) =>
        FormulaFactory.ThereExists(variableDeclaration, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="ExistentialQuantification"/> instances that declares two universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Formula formula) =>
        FormulaFactory.ThereExists(variableDeclaration1, variableDeclaration2, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="ExistentialQuantification"/> instances that declares three universally quantified variables.
    /// </summary>
    /// <param name="variableDeclaration1">The first variable declaration.</param>
    /// <param name="variableDeclaration2">The second variable declaration.</param>
    /// <param name="variableDeclaration3">The third variable declaration.</param>
    /// <param name="formula">The body formula that refers to the declared variables.</param>
    /// <returns>A new <see cref="ExistentialQuantification"/> instance.</returns>
    public static Formula ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Formula formula) =>
        FormulaFactory.ThereExists(variableDeclaration1, variableDeclaration2, variableDeclaration3, formula);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="Conjunction"/>(s) of two (or more) operands.
    /// </summary>
    /// <param name="operand1">The first operand of the conjunction.</param>
    /// <param name="operand2">The second operand of the conjunction.</param>
    /// <param name="otherOperands">Any additional operands.</param>
    /// <returns>A new <see cref="Conjunction"/> instance.</returns>
    public static Formula And(Formula operand1, Formula operand2, params Formula[] otherOperands) =>
        FormulaFactory.And(operand1, operand2, otherOperands);

    /// <summary>
    /// Shorthand factory method for a (tree of) new <see cref="Disjunction"/>(s) of two (or more) operands.
    /// </summary>
    /// <param name="operand1">The first operand of the disjunction.</param>
    /// <param name="operand2">The second operand of the disjunction.</param>
    /// <param name="otherOperands">Any additional operands.</param>
    /// <returns>A new <see cref="Disjunction"/> instance.</returns>
    public static Formula Or(Formula operand1, Formula operand2, params Formula[] otherOperands) =>
        FormulaFactory.Or(operand1, operand2, otherOperands);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Implication"/> instance.
    /// </summary>
    /// <param name="antecedent">The antecedent formula of the implication.</param>
    /// <param name="consequent">The consequent formula of the implication.</param>
    /// <returns>A new <see cref="Implication"/> instance.</returns>
    public static Formula If(Formula antecedent, Formula consequent) =>
        FormulaFactory.If(antecedent, consequent);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Equivalence"/> instance.
    /// </summary>
    /// <param name="left">The left-hand operand of the equivalence.</param>
    /// <param name="right">The right-hand operand of the equivalence.</param>
    /// <returns>A new <see cref="Equivalence"/> instance.</returns>
    public static Formula Iff(Formula left, Formula right) =>
        FormulaFactory.Iff(left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Negation"/> instance.
    /// </summary>
    /// <param name="formula">The negated formula.</param>
    /// <returns>A new <see cref="Negation"/> instance.</returns>
    public static Formula Not(Formula formula) =>
        FormulaFactory.Not(formula);

    /// <summary>
    /// Shorthand factory method for a new <see cref="Predicate"/> instance with the <see cref="EqualityIdentifier.Instance"/> identifier.
    /// </summary>
    /// <param name="left">The left-hand operand of the equality.</param>
    /// <param name="right">The right-hand operand of the equality.</param>
    /// <returns>A new <see cref="Predicate"/> instance.</returns>
    public static Formula AreEqual(Term left, Term right) =>
        FormulaFactory.AreEqual(left, right);

    /// <summary>
    /// Shorthand factory method for a new <see cref="VariableDeclaration"/> instance.
    /// </summary>
    /// <param name="identifier">The identifier of the variable.</param>>
    /// <returns>A new <see cref="VariableDeclaration"/> instance.</returns>
    public static VariableDeclaration Var(object identifier) =>
        FormulaFactory.Var(identifier);

    /// <summary>
    /// Gets a zero-arity <see cref="Function"/> with the identifier "A".
    /// </summary>
    public static Function A { get; } = new(nameof(A));
    /// <summary>
    /// Gets a zero-arity <see cref="Function"/> with the identifier "B".
    /// </summary>
    public static Function B { get; } = new(nameof(B));
    /// <summary>
    /// Gets a zero-arity <see cref="Function"/> with the identifier "C".
    /// </summary>
    public static Function C { get; } = new(nameof(C));
    /// <summary>
    /// Gets a zero-arity <see cref="Function"/> with the identifier "D".
    /// </summary>
    public static Function D { get; } = new(nameof(D));

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

    /// <summary>
    /// Returns a new <see cref="Function"/> instance with the identifier "F".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="Function"/> instance with the identifier "F".</returns>
    public static Function F(params Term[] args) => new(nameof(F), args);
    /// <summary>
    /// Returns a new <see cref="Function"/> instance with the identifier "G".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="Function"/> instance with the identifier "G".</returns>
    public static Function G(params Term[] args) => new(nameof(G), args);
    /// <summary>
    /// Returns a new <see cref="Function"/> instance with the identifier "H".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="Function"/> instance with the identifier "H".</returns>
    public static Function H(params Term[] args) => new(nameof(H), args);
    /// <summary>
    /// Returns a new <see cref="Function"/> instance with the identifier "I".
    /// </summary>
    /// <param name="args">The arguments of the function.</param>
    /// <returns>A new <see cref="Function"/> instance with the identifier "I".</returns>
    public static Function I(params Term[] args) => new(nameof(I), args);

    /// <summary>
    /// Returns a new <see cref="Predicate"/> instance with the identifier "P".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="Predicate"/> instance with the identifier "P".</returns>
    public static Predicate P(params Term[] args) => new(nameof(P), args);
    /// <summary>
    /// Returns a new <see cref="Predicate"/> instance with the identifier "Q".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="Predicate"/> instance with the identifier "Q".</returns>
    public static Predicate Q(params Term[] args) => new(nameof(Q), args);
    /// <summary>
    /// Returns a new <see cref="Predicate"/> instance with the identifier "R".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="Predicate"/> instance with the identifier "R".</returns>
    public static Predicate R(params Term[] args) => new(nameof(R), args);
    /// <summary>
    /// Returns a new <see cref="Predicate"/> instance with the identifier "S".
    /// </summary>
    /// <param name="args">The arguments of the predicate.</param>
    /// <returns>A new <see cref="Predicate"/> instance with the identifier "S".</returns>
    public static Predicate S(params Term[] args) => new(nameof(S), args);
}
