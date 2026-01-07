// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// <para>
/// Representation of a variable declaration within a formula of first order logic. These occur in quantifier formulas.
/// </para>
/// <para>
/// The existence of this type (as distinct from <see cref="VariableReference"/>) is for robust formula transformations. When transforming a
/// <see cref="VariableReference"/> (which is a <see cref="Term"/> - it can occur anywhere in a formula where a <see cref="Term"/>
/// is valid), it will always be valid to transform it into a different kind of <see cref="Term"/>. <see cref="VariableDeclaration"/>s
/// however (which are NOT <see cref="Term"/>s), occur only in <see cref="Quantification"/> and <see cref="VariableReference"/> instances,
/// both of which require the <see cref="VariableDeclaration"/> type exactly.
/// </para>
/// <para>
/// <see cref="VariableDeclaration"/> instances are implicitly convertible to <see cref="VariableReference"/> instances referring to them,
/// to aid in the succinct creation of formulas.
/// </para>
/// </summary>
public sealed class VariableDeclaration : IEquatable<VariableDeclaration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableDeclaration"/> class.
    /// </summary>
    /// <param name="identifier">
    /// <para>
    /// An object that serves as the unique identifier of the variable.
    /// </para>
    /// <para>
    /// Equality of identifiers should indicate that it is the same variable in the domain, 
    /// and ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </param>
    public VariableDeclaration(object identifier) => Identifier = identifier;

    /// <summary>
    /// <para>
    /// Gets an object that serves as the unique identifier of the variable.
    /// </para>
    /// <para>
    /// Equality of identifiers should indicate that it is the same variable in the domain, 
    /// and ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </summary>
    public object Identifier { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is VariableDeclaration otherVariableDeclaration && Equals(otherVariableDeclaration);

    /// <inheritdoc />
    public bool Equals(VariableDeclaration? other) => other != null && Identifier.Equals(other.Identifier);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Identifier);

    /// <summary>
    /// Defines the implicit conversion operator from a variable declaration to a reference.
    /// </summary>
    /// <param name="declaration">The declaration to convert.</param>
    public static implicit operator VariableReference(VariableDeclaration declaration) => new(declaration);
}
