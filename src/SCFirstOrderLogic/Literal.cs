// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaFormatting;
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// <para>
/// Streamlined representation of a literal (that is, a predicate or a negated predicate) of first-order logic.
/// Consists of the <see cref="SCFirstOrderLogic.Predicate"/> and a <see cref="Boolean"/> that indicates positivity/negativity.
/// </para>
/// <para>
/// Note that this type is NOT a subtype of <see cref="Formula"/>. To represent a literal as a <see cref="Formula"/>, 
/// <see cref="SCFirstOrderLogic.Predicate"/> and <see cref="Negation"/> should be used as appropriate.
/// </para>
/// </summary>
public sealed class Literal : IEquatable<Literal>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="Literal"/> class.
    /// </summary>
    /// <param name="formula">The literal, represented as a <see cref="Formula"/> object. An exception will be thrown if it is neither a predicate nor a negated predicate.</param>
    public Literal(Formula formula)
    {
        if (formula is Negation negation)
        {
            IsPositive = false;
            formula = negation.Formula;
        }

        if (formula is Predicate predicate)
        {
            Predicate = predicate;
        }
        else
        {
            throw new ArgumentException($"Provided formula must be a predicate or a negated predicate. {formula} is neither.", nameof(formula));
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Literal"/> class.
    /// </summary>
    /// <param name="predicate">The atomic formula to which this literal refers.</param>
    /// <param name="isNegative">A value indicating whether the atomic formula is negated.</param>
    // TODO*-BREAKING: looking at this with fresh eyes, its really counterintuitive - new Literal(MyPredicate, false)
    // would make more intuitive sense to be negated. That is, this should probably be Literal(predicate, isPositive)
    // This is unfortunately a really dangerous breaking change (because it'll still compile..)
    public Literal(Predicate predicate, bool isNegative)
    {
        Predicate = predicate;
        IsPositive = !isNegative;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Literal"/> class that is not negated.
    /// </summary>
    /// <param name="predicate">The atomic formula to which this literal refers.</param>
    public Literal(Predicate predicate)
        : this(predicate, false)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this literal is a negation of the underlying atomic formula.
    /// </summary>
    public bool IsNegative => !IsPositive;

    /// <summary>
    /// Gets a value indicating whether this literal is not a negation of the underlying atomic formula.
    /// </summary>
    public bool IsPositive { get; }

    /// <summary>
    /// Gets the underlying atomic formula of this literal.
    /// </summary>
    public Predicate Predicate { get; }

    /// <summary>
    /// Constructs and returns a literal that is the negation of this one.
    /// </summary>
    /// <returns>A literal that is the negation of this one.</returns>
    public Literal Negate() => new(Predicate, !IsNegative);

    /// <summary>
    /// Converts the literal to a <see cref="Formula"/>
    /// </summary>
    /// <returns>A representation of this literal as a <see cref="Formula"/>.</returns>
    public Formula ToFormula()
    {
        return IsNegative ? new Negation(Predicate) : Predicate;
    }

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
    public override string ToString() => new FormulaFormatter().Format(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Literal literal && Equals(literal);

    /// <inheritdoc />
    public bool Equals(Literal? other)
    {
        return other != null && other.Predicate.Equals(Predicate) && other.IsNegative.Equals(IsNegative);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Predicate, IsNegative);

    /// <summary>
    /// Defines the conversion of a <see cref="SCFirstOrderLogic.Predicate"/> instance to a <see cref="Literal"/>.
    /// </summary>
    /// <param name="predicate">The predicate to convert.</param>
    public static explicit operator Literal(Predicate predicate) => new(predicate);
}