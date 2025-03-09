﻿// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a term within a sentence of first order logic.
/// </summary>
public abstract class Term
{
    /// <summary>
    /// Gets a value indicating whether the term is a ground term - that is, that it contains no variables.
    /// </summary>
    public abstract bool IsGroundTerm { get; }

    /// <summary>
    /// Accepts a <see cref="ITermVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    public abstract void Accept(ITermVisitor visitor);

    /// <summary>
    /// Accepts a <see cref="ITermVisitor{TState}"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">A reference to the state that the visitor is working with.</param>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    public abstract void Accept<TState>(ITermVisitor<TState> visitor, TState state);

    /// <summary>
    /// Accepts a <see cref="ITermVisitorR{TState}"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">A reference to the state that the visitor is working with.</param>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    public abstract void Accept<TState>(ITermVisitorR<TState> visitor, ref TState state);

    /// <summary>
    /// Accepts a <see cref="ITermTransformation{TOut}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <returns>The result of the transformation.</returns>
    public abstract TOut Accept<TOut>(ITermTransformation<TOut> transformation);

    /// <summary>
    /// Accepts a <see cref="ITermTransformation{TOut,TState}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <typeparam name="TState">The type of state that the transformation works with.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <param name="state">The state that the transformation is to work with.</param>
    public abstract TOut Accept<TOut, TState>(ITermTransformation<TOut, TState> transformation, TState state);

    /// <summary>
    /// Accepts a <see cref="IAsyncTermVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    public abstract Task AcceptAsync(IAsyncTermVisitor visitor);

    /// <summary>
    /// Accepts a <see cref="IAsyncTermVisitor{TState}"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">A reference to the state that the visitor is working with.</param>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    public abstract Task AcceptAsync<TState>(IAsyncTermVisitor<TState> visitor, TState state);

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the term.
    /// If the term is from a normalised sentence, its worth noting that this will not guarantee unique labelling of any normalisation terms
    /// (standardised variables or Skolem functions) across a set of sentences, or provide any choice as to the sets of labels used for
    /// normalisation terms. If you want either of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
    /// </para>
    /// <para>
    /// Aside: I have wondered if it would perhaps better to just enforce explicit SentenceFormatter use. That would however be a PITA if you
    /// just want to print out your nice, simple sentence. It may even be non-normalised - in which case you definitely won't want to be
    /// messing around with sets of labels. So its important that this stays - to avoid a barrier to entry for the library.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new SentenceFormatter().Format(this);
}
