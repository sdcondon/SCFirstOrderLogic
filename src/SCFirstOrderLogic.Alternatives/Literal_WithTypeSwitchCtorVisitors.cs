﻿// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a literal (i.e. an atomic sentence or a negated atomic sentence) of first-order logic within a sentence in conjunctive normal form.
/// </summary>
/// <remarks>
/// NB: Yes, literals are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. Hence this type being called CNFLiteral and residing in this namespace.
/// </remarks>
public class Literal_WithTypeSwitchCtorVisitors : IEquatable<Literal_WithTypeSwitchCtorVisitors>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AltCNFLiteral_WithTypeSwitchCtorVisitors"/> class.
    /// </summary>
    /// <param name="sentence">The literal, represented as a <see cref="Sentence"/> object. An exception will be thrown if it is neither a predicate nor a negated predicate.</param>
    public Literal_WithTypeSwitchCtorVisitors(Sentence sentence)
    {
        if (sentence is Negation negation)
        {
            IsNegated = true;
            sentence = negation.Sentence;
        }

        if (sentence is Predicate predicate)
        {
            Predicate = predicate;
        }
        else
        {
            throw new ArgumentException($"Provided sentence must be either a predicate or a negated predicate. {sentence} is neither.", nameof(sentence));
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFLiteral_WithoutTypeSwitch"/> class.
    /// </summary>
    /// <param name="predicate">The atomic sentence to which this literal refers.</param>
    /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
    public Literal_WithTypeSwitchCtorVisitors(Predicate predicate, bool isNegated)
    {
        Predicate = predicate;
        IsNegated = isNegated;
    }

    /// <summary>
    /// Gets a value indicating whether this literal is a negation of the underlying atomic sentence.
    /// </summary>
    public bool IsNegated { get; }

    /// <summary>
    /// Gets a value indicating whether this literal is not a negation of the underlying atomic sentence.
    /// </summary>
    public bool IsPositive => !IsNegated;

    /// <summary>
    /// Gets the underlying atomic sentence of this literal.
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
    /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the literal.
    /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
    /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
    /// of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    ////public override string ToString() => new SentenceFormatter().Print(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Literal_WithTypeSwitchCtorVisitors literal && Equals(literal);

    /// <inheritdoc />
    public bool Equals(Literal_WithTypeSwitchCtorVisitors? other)
    {
        return other != null && other.Predicate.Equals(Predicate) && other.IsNegated.Equals(IsNegated);
    }

    /// <summary>
    /// Converts the literal to a <see cref="Sentence"/>
    /// </summary>
    /// <returns>A representation of this literal as a <see cref="Sentence"/>.</returns>
    public Sentence ToSentence()
    {
        return IsNegated ? new Negation(Predicate) : Predicate;
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Predicate, IsNegated);

    /// <summary>
    /// Defines the (explicit) conversion of a <see cref="Sentence"/> instance to a <see cref="CNFLiteral_WithoutTypeSwitch"/>. NB: This conversion is explicit because it can fail (if the sentence isn't actually a literal).
    /// </summary>
    /// <param name="sentence">The sentence to convert.</param>
    public static explicit operator Literal_WithTypeSwitchCtorVisitors(Sentence sentence)
    {
        try
        {
            return new Literal_WithTypeSwitchCtorVisitors(sentence);
        }
        catch (ArgumentException e)
        {
            throw new InvalidCastException($"To be converted to a literal, sentences must be either a predicate or a negated predicate. {sentence} is neither.", e);
        }
    }

    /// <summary>
    /// Defines the (implicit) conversion of a <see cref="Predicate"/> instance to a <see cref="Literal"/>. NB: This conversion is implicit because it is always valid.
    /// </summary>
    /// <param name="sentence">The predicate to convert.</param>
    public static implicit operator Literal_WithTypeSwitchCtorVisitors(Predicate predicate) => new(predicate);
}