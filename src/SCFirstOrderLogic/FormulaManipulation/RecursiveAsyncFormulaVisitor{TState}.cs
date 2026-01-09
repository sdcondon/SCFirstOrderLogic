// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Base class for recursive asynchronous visitors of <see cref="Formula"/> (and <see cref="Term"/>) instances that reference external state.
/// </para>
/// <para>
/// That is, a base class for visitors in which the default implementation for any non-terminal
/// element simply visits the element's children - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveAsyncFormulaVisitor<TState> : IAsyncFormulaVisitor<TState>, IAsyncTermVisitor<TState>
{
    /// <summary>
    /// Visits a <see cref="Formula"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the formula (via <see cref="Formula.Accept{TState}(IFormulaVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Formula formula, TState state, CancellationToken cancellationToken = default)
    {
        await formula.AcceptAsync(this, state, cancellationToken);
    }

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Conjunction conjunction, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(conjunction.Left, state, cancellationToken),
            VisitAsync(conjunction.Right, state, cancellationToken));
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Disjunction disjunction, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(disjunction.Left, state, cancellationToken),
            VisitAsync(disjunction.Right, state, cancellationToken));
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Equivalence equivalence, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(equivalence.Left, state, cancellationToken),
            VisitAsync(equivalence.Right, state, cancellationToken));
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(ExistentialQuantification existentialQuantification, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(existentialQuantification.Variable, state, cancellationToken),
            VisitAsync(existentialQuantification.Formula, state, cancellationToken));
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Implication implication, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(implication.Antecedent, state, cancellationToken),
            VisitAsync(implication.Consequent, state, cancellationToken));
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Predicate predicate, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(predicate.Arguments.Select(a => VisitAsync(a, state, cancellationToken)));
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Negation negation, TState state, CancellationToken cancellationToken = default)
    {
        await VisitAsync(negation.Formula, state, cancellationToken);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(UniversalQuantification universalQuantification, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            VisitAsync(universalQuantification.Variable, state, cancellationToken),
            VisitAsync(universalQuantification.Formula, state, cancellationToken));
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept{TState}(ITermVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="term">The term to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Term term, TState state, CancellationToken cancellationToken = default)
    {
        await term.AcceptAsync(this, state, cancellationToken);
    }

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(VariableReference variable, TState state, CancellationToken cancellationToken = default)
    {
        await VisitAsync(variable.Declaration, state, cancellationToken);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual async Task VisitAsync(Function function, TState state, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(function.Arguments.Select(a => VisitAsync(a, state, cancellationToken)));
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public virtual Task VisitAsync(VariableDeclaration variableDeclaration, TState state, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
