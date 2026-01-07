// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a material equivalence formula of first order logic. In typical FOL syntax, this is written as:
/// <code>{formula} ⇔ {formula}</code>
/// </summary>
public sealed class Equivalence : Formula
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Equivalence"/> class.
    /// </summary>
    /// <param name="left">The left side of the equivalence.</param>
    /// <param name="right">The right side of the equivalence.</param>
    public Equivalence(Formula left, Formula right) => (Left, Right) = (left, right);

    /// <summary>
    /// Gets the left side of the equivalence.
    /// </summary>
    public Formula Left { get; }

    /// <summary>
    /// Gets the right side of the equivalence.
    /// </summary>
    public Formula Right { get; }

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
    public override bool Equals(object? obj) => obj is Equivalence otherEquivalence && Left.Equals(otherEquivalence.Left) && Right.Equals(otherEquivalence.Right);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Left, Right);
}
