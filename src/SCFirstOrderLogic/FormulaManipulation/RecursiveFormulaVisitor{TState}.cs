// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Base class for recursive visitors of <see cref="Formula"/> (and <see cref="Term"/>) instances that reference external state.
/// </para>
/// <para>
/// That is, a base class for visitors in which the default implementation for any non-terminal
/// element simply visits the element's children - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveFormulaVisitor<TState> : IFormulaVisitor<TState>, ITermVisitor<TState>
{
    /// <summary>
    /// Visits a <see cref="Formula"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the formula (via <see cref="Formula.Accept{TState}(IFormulaVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Formula formula, TState state) => formula.Accept(this, state);

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Conjunction conjunction, TState state)
    {
        Visit(conjunction.Left, state);
        Visit(conjunction.Right, state);
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Disjunction disjunction, TState state)
    {
        Visit(disjunction.Left, state);
        Visit(disjunction.Right, state);
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Equivalence equivalence, TState state)
    {
        Visit(equivalence.Left, state);
        Visit(equivalence.Right, state);
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sub-formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(ExistentialQuantification existentialQuantification, TState state)
    {
        Visit(existentialQuantification.Variable, state);
        Visit(existentialQuantification.Formula, state);
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Implication implication, TState state)
    {
        Visit(implication.Antecedent, state);
        Visit(implication.Consequent, state);
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Predicate predicate, TState state)
    {
        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            Visit(predicate.Arguments[i], state);
        }
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Negation negation, TState state)
    {
        Visit(negation.Formula, state);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sub-formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(UniversalQuantification universalQuantification, TState state)
    {
        Visit(universalQuantification.Variable, state);
        Visit(universalQuantification.Formula, state);
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept{TState}(ITermVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="term">The term to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Term term, TState state) => term.Accept(this, state);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(VariableReference variable, TState state)
    {
        Visit(variable.Declaration, state);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(Function function, TState state)
    {
        for (int i = 0; i < function.Arguments.Count; i++)
        {
            Visit(function.Arguments[i], state);
        }
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual void Visit(VariableDeclaration variableDeclaration, TState state)
    {
    }
}
