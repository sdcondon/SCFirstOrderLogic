// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a quantification formula of first order logic.
/// </summary>
public abstract class Quantification : Formula
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quantification"/> class.
    /// </summary>
    /// <param name="variable">The variable declared by this quantification.</param>
    /// <param name="formula">The formula that this quantification applies to.</param>
    protected Quantification(VariableDeclaration variable, Formula formula) => (Variable, Formula) = (variable, formula);

    /// <summary>
    /// Gets the variable declared by this quantification.
    /// </summary>
    public VariableDeclaration Variable { get; }

    /// <summary>
    /// Gets the formula that this quantification applies to.
    /// </summary>
    public Formula Formula { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Quantification quantification
            && Variable.Equals(quantification.Variable)
            && Formula.Equals(quantification.Formula);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Variable, Formula);
}
