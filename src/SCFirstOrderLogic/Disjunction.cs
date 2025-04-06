// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a disjunction sentence of first order logic. In typical FOL syntax, this is written as:
/// <code>{sentence} ∨ {sentence}</code>
/// </summary>
public sealed class Disjunction : Sentence
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Disjunction"/> class.
    /// </summary>
    /// <param name="left">The left side of the disjunction.</param>
    /// <param name="right">The right side of the disjunction.</param>
    public Disjunction(Sentence left, Sentence right) => (Left, Right) = (left, right);

    /// <summary>
    /// Gets the left side of the disjunction.
    /// </summary>
    public Sentence Left { get; }

    /// <summary>
    /// Gets the right side of the disjunction.
    /// </summary>
    public Sentence Right { get; }

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
    public override bool Equals(object? obj) => obj is Disjunction otherDisjunction && Left.Equals(otherDisjunction.Left) && Right.Equals(otherDisjunction.Right);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Left, Right);
}
