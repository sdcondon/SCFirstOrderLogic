// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaFormatting;
using SCFirstOrderLogic.FormulaManipulation;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a formula of first order logic.
/// </summary>
public abstract class Formula
{
    /// <summary>
    /// Accepts a <see cref="IFormulaVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    public abstract void Accept(IFormulaVisitor visitor);

    /// <summary>
    /// Accepts a <see cref="IFormulaVisitor{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">The state that the visitor is to work with.</param>
    public abstract void Accept<TState>(IFormulaVisitor<TState> visitor, TState state);

    /// <summary>
    /// Accepts a <see cref="IFormulaVisitorR{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">A reference to the state that the visitor is to work with.</param>
    public abstract void Accept<TState>(IFormulaVisitorR<TState> visitor, ref TState state);

    /// <summary>
    /// Accepts a <see cref="IFormulaTransformation{TOut}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <returns>The result of the transformation.</returns>
    public abstract TOut Accept<TOut>(IFormulaTransformation<TOut> transformation);

    /// <summary>
    /// Accepts a <see cref="IFormulaTransformation{TOut,TState}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <typeparam name="TState">The type of state that the transformation works with.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <param name="state">The state that the transformation is to work with.</param>
    public abstract TOut Accept<TOut, TState>(IFormulaTransformation<TOut, TState> transformation, TState state);

    /// <summary>
    /// Accepts a <see cref="IAsyncFormulaVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public abstract Task AcceptAsync(IAsyncFormulaVisitor visitor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accepts a <see cref="IAsyncFormulaVisitor{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">The state that the visitor is to work with.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    public abstract Task AcceptAsync<TState>(IAsyncFormulaVisitor<TState> visitor, TState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="FormulaFormatter"/> object and uses it to format the formula.
    /// If the formula has been normalised (i.e. contains standardised variables or Skolem functions), its worth noting that this
    /// will not guarantee unique labelling of any normalisation terms (standardised variables and Skolem functions) across a set
    /// of formulas, or provide any choice as to the sets of labels used for them. If you want either of these behaviours,
    /// instantiate your own <see cref="FormulaFormatter"/> instance.
    /// </para>
    /// <para>
    /// Aside: I have wondered if it would perhaps better to just enforce explicit FormulaFormatter use by not defining an override here.
    /// That would however be a PITA if you just want to print out your nice, simple formula. It may even be non-normalised - in which
    /// case you definitely won't want to be messing around with sets of labels. So its important that this stays - to avoid a barrier to
    /// entry for the library.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new FormulaFormatter().Format(this);
}
