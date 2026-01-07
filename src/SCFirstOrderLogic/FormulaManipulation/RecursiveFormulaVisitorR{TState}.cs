// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Base class for recursive visitors of <see cref="Formula"/> instances that reference external state that is passed in by reference.
/// </para>
/// <para>
/// That is, a base class for visitors in which the default implementation for any non-terminal
/// formula/term element simply visits the element's children - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveFormulaVisitorR<TState> : IFormulaVisitorR<TState>, ITermVisitorR<TState>
{
    /// <summary>
    /// Visits a <see cref="Formula"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the formula (via <see cref="Formula.Accept{TState}(IFormulaVisitorR{TState}, ref TState)"/>.
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Formula formula, ref TState state) => formula.Accept(this, ref state);

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Conjunction conjunction, ref TState state)
    {
        Visit(conjunction.Left, ref state);
        Visit(conjunction.Right, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Disjunction disjunction, ref TState state)
    {
        Visit(disjunction.Left, ref state);
        Visit(disjunction.Right, ref state);
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Equivalence equivalence, ref TState state)
    {
        Visit(equivalence.Left, ref state);
        Visit(equivalence.Right, ref state);
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sub-formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(ExistentialQuantification existentialQuantification, ref TState state)
    {
        Visit(existentialQuantification.Variable, ref state);
        Visit(existentialQuantification.Formula, ref state);
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Implication implication, ref TState state)
    {
        Visit(implication.Antecedent, ref state);
        Visit(implication.Consequent, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Predicate predicate, ref TState state)
    {
        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            Visit(predicate.Arguments[i], ref state);
        }
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Negation negation, ref TState state)
    {
        Visit(negation.Formula, ref state);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sub-formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(UniversalQuantification universalQuantification, ref TState state)
    {
        Visit(universalQuantification.Variable, ref state);
        Visit(universalQuantification.Formula, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept{TState}(ITermVisitorR{TState}, ref TState)"/>.
    /// </summary>
    /// <param name="term">The term to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Term term, ref TState state) => term.Accept(this, ref state);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(VariableReference variable, ref TState state)
    {
        Visit(variable.Declaration, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(Function function, ref TState state)
    {
        for (int i = 0; i < function.Arguments.Count; i++)
        {
            Visit(function.Arguments[i], ref state);
        }
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    /// <param name="state">A reference to the state of this visitation.</param>
    public virtual void Visit(VariableDeclaration variableDeclaration, ref TState state)
    {
    }
}
