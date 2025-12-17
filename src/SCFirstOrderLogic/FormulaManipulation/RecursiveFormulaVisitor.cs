// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Base class for recursive visitors of <see cref="Formula"/> instances.
/// </para>
/// <para>
/// That is, a base class for visitors in which the default implementation for any non-terminal
/// formula/term element simply visits the element's children - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveFormulaVisitor : IFormulaVisitor, ITermVisitor
{
    /// <summary>
    /// Visits a <see cref="Formula"/> instance.
    /// The default implementation just invokes the Visit method appropriate to the type of the formula (via <see cref="Formula.Accept(IFormulaVisitor)"/>).
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    public virtual void Visit(Formula formula) => formula.Accept(this);

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    public virtual void Visit(Conjunction conjunction)
    {
        Visit(conjunction.Left);
        Visit(conjunction.Right);
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    public virtual void Visit(Disjunction disjunction)
    {
        Visit(disjunction.Left);
        Visit(disjunction.Right);
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    public virtual void Visit(Equivalence equivalence)
    {
        Visit(equivalence.Left);
        Visit(equivalence.Right);
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    public virtual void Visit(ExistentialQuantification existentialQuantification)
    {
        Visit(existentialQuantification.Variable);
        Visit(existentialQuantification.Formula);
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    public virtual void Visit(Implication implication)
    {
        Visit(implication.Antecedent);
        Visit(implication.Consequent);
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    public virtual void Visit(Predicate predicate)
    {
        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            Visit(predicate.Arguments[i]);
        }
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    public virtual void Visit(Negation negation)
    {
        Visit(negation.Formula);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    public virtual void Visit(UniversalQuantification universalQuantification)
    {
        Visit(universalQuantification.Variable);
        Visit(universalQuantification.Formula);
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation just invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept(ITermVisitor)"/>).
    /// </summary>
    /// <param name="term">The term to visit.</param>
    public virtual void Visit(Term term) => term.Accept(this);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    public virtual void Visit(VariableReference variable)
    {
        Visit(variable.Declaration);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    public virtual void Visit(Function function)
    {
        for (int i = 0; i < function.Arguments.Count; i++)
        {
            Visit(function.Arguments[i]);
        }
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    public virtual void Visit(VariableDeclaration variableDeclaration)
    {
    }
}
