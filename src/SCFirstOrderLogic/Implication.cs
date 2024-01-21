// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a material implication sentence of first order logic. In typical FOL syntax, this is written as:
/// <code>{sentence} ⇒ {sentence}</code>
/// </summary>
public sealed class Implication : Sentence
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Implication"/> class.
    /// </summary>
    /// <param name="antecedent">The antecedent sentence.</param>
    /// <param name="consequent">The consequent sentence.</param>
    public Implication(Sentence antecedent, Sentence consequent) => (Antecedent, Consequent) = (antecedent, consequent);

    /// <summary>
    /// Gets the antecedent sentence.
    /// </summary>
    public Sentence Antecedent { get; }

    /// <summary>
    /// Gets the consequent sentence.
    /// </summary>
    public Sentence Consequent { get; }

    /// <inheritdoc />
    public override void Accept(ISentenceVisitor visitor) => visitor.Visit(this);

    /// <inheritdoc />
    public override void Accept<T>(ISentenceVisitor<T> visitor, T state) => visitor.Visit(this, state);

    /// <inheritdoc />
    public override void Accept<T>(ISentenceVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

    /// <inheritdoc />
    public override TOut Accept<TOut>(ISentenceTransformation<TOut> transformation) => transformation.ApplyTo(this);

    /// <inheritdoc />
    public override TOut Accept<TOut, TState>(ISentenceTransformation<TOut, TState> transformation, TState state) => transformation.ApplyTo(this, state);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Implication otherImplication
        && Antecedent.Equals(otherImplication.Antecedent)
        && Consequent.Equals(otherImplication.Consequent);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
}
