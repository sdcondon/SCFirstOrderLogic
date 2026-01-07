// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaFormatting;
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a literal (i.e. an atomic formula or a negated atomic formula) of first-order logic within a formula in conjunctive normal form.
/// </summary>
public class Literal_WithTypeSwitchCtorVisitors : IEquatable<Literal_WithTypeSwitchCtorVisitors>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AltCNFLiteral_WithTypeSwitchCtorVisitors"/> class.
    /// </summary>
    /// <param name="formula">The literal, represented as a <see cref="Formula"/> object. An exception will be thrown if it is neither a predicate nor a negated predicate.</param>
    public Literal_WithTypeSwitchCtorVisitors(Formula formula)
    {
        if (formula is Negation negation)
        {
            IsNegated = true;
            formula = negation.Formula;
        }

        if (formula is Predicate predicate)
        {
            Predicate = predicate;
        }
        else
        {
            throw new ArgumentException($"Provided formula must be either a predicate or a negated predicate. {formula} is neither.", nameof(formula));
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFLiteral_WithoutTypeSwitch"/> class.
    /// </summary>
    /// <param name="predicate">The atomic formula to which this literal refers.</param>
    /// <param name="isNegated">A value indicating whether the atomic formula is negated.</param>
    public Literal_WithTypeSwitchCtorVisitors(Predicate predicate, bool isNegated)
    {
        Predicate = predicate;
        IsNegated = isNegated;
    }

    /// <summary>
    /// Gets a value indicating whether this literal is a negation of the underlying atomic formula.
    /// </summary>
    public bool IsNegated { get; }

    /// <summary>
    /// Gets a value indicating whether this literal is not a negation of the underlying atomic formula.
    /// </summary>
    public bool IsPositive => !IsNegated;

    /// <summary>
    /// Gets the underlying atomic formula of this literal.
    /// </summary>
    public Predicate Predicate { get; }

    /// <summary>
    /// Constructs and returns a literal that is the negation of this one.
    /// </summary>
    /// <returns>A literal that is the negation of this one.</returns>
    public Literal Negate() => new(Predicate, !IsNegated);

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="FormulaFormatter"/> object and uses it to format the literal.
    /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
    /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
    /// of these things, instantiate your own <see cref="FormulaFormatter"/> instance.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    ////public override string ToString() => new FormulaFormatter().Print(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Literal_WithTypeSwitchCtorVisitors literal && Equals(literal);

    /// <inheritdoc />
    public bool Equals(Literal_WithTypeSwitchCtorVisitors? other)
    {
        return other != null && other.Predicate.Equals(Predicate) && other.IsNegated.Equals(IsNegated);
    }

    /// <summary>
    /// Converts the literal to a <see cref="Formula"/>
    /// </summary>
    /// <returns>A representation of this literal as a <see cref="Formula"/>.</returns>
    public Formula ToFormula()
    {
        return IsNegated ? new Negation(Predicate) : Predicate;
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Predicate, IsNegated);

    /// <summary>
    /// Defines the (explicit) conversion of a <see cref="Formula"/> instance to a <see cref="CNFLiteral_WithoutTypeSwitch"/>. NB: This conversion is explicit because it can fail (if the formula isn't actually a literal).
    /// </summary>
    /// <param name="formula">The formula to convert.</param>
    public static explicit operator Literal_WithTypeSwitchCtorVisitors(Formula formula)
    {
        try
        {
            return new Literal_WithTypeSwitchCtorVisitors(formula);
        }
        catch (ArgumentException e)
        {
            throw new InvalidCastException($"To be converted to a literal, formulas must be either a predicate or a negated predicate. {formula} is neither.", e);
        }
    }

    /// <summary>
    /// Defines the (implicit) conversion of a <see cref="Predicate"/> instance to a <see cref="Literal"/>. NB: This conversion is implicit because it is always valid.
    /// </summary>
    /// <param name="predicate">The predicate to convert.</param>
    public static implicit operator Literal_WithTypeSwitchCtorVisitors(Predicate predicate) => new(predicate);
}