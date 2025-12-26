// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a variable term within a formula of first order logic. These are declared in quantifier formulas.
/// </summary>
public sealed class VariableReference : Term
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableReference"/> class.
    /// </summary>
    /// <param name="declaration">The declaration of the variable.</param>
    public VariableReference(VariableDeclaration declaration) => Declaration = declaration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableReference"/> class.
    /// </summary>
    /// <param name="identifier">
    /// The identifier of the variable. Equality of identifiers should indicate that it is the same variable in the domain,
    /// and ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </param>
    public VariableReference(object identifier) => Declaration = new VariableDeclaration(identifier);

    /// <summary>
    /// Gets the declaration of the variable.
    /// </summary>
    public VariableDeclaration Declaration { get; }

    /// <summary>
    /// Gets the identifier of the variable.
    /// </summary>
    public object Identifier => Declaration.Identifier;

    /// <inheritdoc />
    public override bool IsGroundTerm => false;

    /// <inheritdoc />
    public override void Accept(ITermVisitor visitor) => visitor.Visit(this);

    /// <inheritdoc />
    public override void Accept<T>(ITermVisitor<T> visitor, T state) => visitor.Visit(this, state);

    /// <inheritdoc />
    public override void Accept<T>(ITermVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

    /// <inheritdoc />
    public override TOut Accept<TOut>(ITermTransformation<TOut> transformation) => transformation.ApplyTo(this);

    /// <inheritdoc />
    public override TOut Accept<TOut, TState>(ITermTransformation<TOut, TState> transformation, TState state) => transformation.ApplyTo(this, state);

    /// <inheritdoc />
    public override Task AcceptAsync(IAsyncTermVisitor visitor, CancellationToken cancellationToken = default) => visitor.VisitAsync(this, cancellationToken);

    /// <inheritdoc />
    public override Task AcceptAsync<T>(IAsyncTermVisitor<T> visitor, T state, CancellationToken cancellationToken = default) => visitor.VisitAsync(this, state, cancellationToken);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is VariableReference otherVariable && Declaration.Equals(otherVariable.Declaration); 
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Declaration);
    }
}
