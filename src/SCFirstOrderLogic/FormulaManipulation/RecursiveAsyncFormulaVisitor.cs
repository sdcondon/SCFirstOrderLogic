// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Base class for recursive asynchronous visitors of <see cref="Formula"/> instances.
/// </para>
/// <para>
/// That is, a base class for asynchronous visitors in which the default implementation for any non-terminal
/// formula/term element simply visits the element's children (in parallel) - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveAsyncFormulaVisitor : IAsyncFormulaVisitor, IAsyncTermVisitor
{
    /// <summary>
    /// Visits a <see cref="Formula"/> instance.
    /// The default implementation just invokes the VisitAsync method appropriate to the type of the formula (via <see cref="Formula.Accept(IFormulaVisitor)"/>).
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    public virtual async Task VisitAsync(Formula formula)
    {
        await formula.AcceptAsync(this);
    }

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    public virtual async Task VisitAsync(Conjunction conjunction)
    {
        await Task.WhenAll(
            VisitAsync(conjunction.Left),
            VisitAsync(conjunction.Right));
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    public virtual async Task VisitAsync(Disjunction disjunction)
    {
        await Task.WhenAll(
            VisitAsync(disjunction.Left),
            VisitAsync(disjunction.Right));
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    public virtual async Task VisitAsync(Equivalence equivalence)
    {
        await Task.WhenAll(
            VisitAsync(equivalence.Left),
            VisitAsync(equivalence.Right));
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    public virtual async Task VisitAsync(ExistentialQuantification existentialQuantification)
    {
        await Task.WhenAll(
            VisitAsync(existentialQuantification.Variable),
            VisitAsync(existentialQuantification.Formula));
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    public virtual async Task VisitAsync(Implication implication)
    {
        await Task.WhenAll(
            VisitAsync(implication.Antecedent),
            VisitAsync(implication.Consequent));
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    public virtual async Task VisitAsync(Predicate predicate)
    {
        await Task.WhenAll(predicate.Arguments.Select(a => VisitAsync(a)));
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    public virtual async Task VisitAsync(Negation negation)
    {
        await VisitAsync(negation.Formula);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    public virtual async Task VisitAsync(UniversalQuantification universalQuantification)
    {
        await Task.WhenAll(
            VisitAsync(universalQuantification.Variable),
            VisitAsync(universalQuantification.Formula));
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation just invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept(ITermVisitor)"/>).
    /// </summary>
    /// <param name="term">The term to visit.</param>
    public virtual async Task VisitAsync(Term term)
    {
        await term.AcceptAsync(this);
    }

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    public virtual async Task VisitAsync(VariableReference variable)
    {
        await VisitAsync(variable.Declaration);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    public virtual async Task VisitAsync(Function function)
    {
        await Task.WhenAll(function.Arguments.Select(a => VisitAsync(a)));
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    public virtual Task VisitAsync(VariableDeclaration variableDeclaration)
    {
        return Task.CompletedTask;
    }
}
