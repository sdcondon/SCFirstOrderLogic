// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a negation sentence of first order logic. In typical FOL syntax, this is written as:
/// <code>¬{sentence}</code>
/// </summary>
public sealed class Negation : Sentence
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Negation"/> class.
    /// </summary>
    /// <param name="sentence">The sentence that is negated.</param>
    public Negation(Sentence sentence) => Sentence = sentence;

    /// <summary>
    /// Gets the sentence that is negated.
    /// </summary>
    public Sentence Sentence { get; }

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
    public override Task AcceptAsync(IAsyncSentenceVisitor visitor) => visitor.VisitAsync(this);

    /// <inheritdoc />
    public override Task AcceptAsync<T>(IAsyncSentenceVisitor<T> visitor, T state) => visitor.VisitAsync(this, state);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Negation negation && Sentence.Equals(negation.Sentence);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Sentence);
}
