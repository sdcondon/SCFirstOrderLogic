// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation;
using System;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a universal quantification formula of first order logic. In typical FOL syntax, this is written as:
/// <code>∀ {variable}, {formula}</code>
/// </summary>
public sealed class UniversalQuantification : Quantification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalQuantification"/> class.
    /// </summary>
    /// <param name="variable">The variable declared by this quantification.</param>
    /// <param name="formula">The formula that this quantification applies to.</param>
    public UniversalQuantification(VariableDeclaration variable, Formula formula)
        : base(variable, formula)
    {
    }

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
    public override Task AcceptAsync(IAsyncFormulaVisitor visitor) => visitor.VisitAsync(this);

    /// <inheritdoc />
    public override Task AcceptAsync<T>(IAsyncFormulaVisitor<T> visitor, T state) => visitor.VisitAsync(this, state);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is UniversalQuantification universalQuantification && base.Equals(universalQuantification);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Variable, Formula);
}
