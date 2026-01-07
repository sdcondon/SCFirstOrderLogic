// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a material implication formula of first order logic. In typical FOL syntax, this is written as:
/// <code>{formula} ⇒ {formula}</code>
/// </summary>
public sealed class Implication : Formula
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Implication"/> class.
    /// </summary>
    /// <param name="antecedent">The antecedent formula.</param>
    /// <param name="consequent">The consequent formula.</param>
    public Implication(Formula antecedent, Formula consequent) => (Antecedent, Consequent) = (antecedent, consequent);

    /// <summary>
    /// Gets the antecedent formula.
    /// </summary>
    public Formula Antecedent { get; }

    /// <summary>
    /// Gets the consequent formula.
    /// </summary>
    public Formula Consequent { get; }

    /// <inheritdoc />
    public override void Accept(IFormulaVisitor visitor) => visitor.Visit(this);

    /// <inheritdoc />
    public override void Accept<T>(IFormulaVisitor<T> visitor, T state) => visitor.Visit(this, state);

    /// <inheritdoc />
    public override void Accept<T>(IFormulaVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

    /// <inheritdoc />
    public override TOut Accept<TOut>(IFormulaTransformation<TOut> transformation) => transformation.ApplyTo(this);

    /// <inheritdoc />
    public override TOut Accept<TOut, TState>(IFormulaTransformation<TOut, TState> transformation, TState state) => transformation.ApplyTo(this, state);

    /// <inheritdoc />
    public override Task AcceptAsync(IAsyncFormulaVisitor visitor, CancellationToken cancellationToken = default) => visitor.VisitAsync(this, cancellationToken);

    /// <inheritdoc />
    public override Task AcceptAsync<T>(IAsyncFormulaVisitor<T> visitor, T state, CancellationToken cancellationToken = default) => visitor.VisitAsync(this, state, cancellationToken);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Implication otherImplication
        && Antecedent.Equals(otherImplication.Antecedent)
        && Consequent.Equals(otherImplication.Consequent);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
}
