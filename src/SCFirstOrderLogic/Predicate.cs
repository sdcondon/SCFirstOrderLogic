// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
/// <code>{predicate identifier}({term}, ..)</code>
/// </summary>
public sealed class Predicate : Sentence
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Predicate"/> class.
    /// </summary>
    /// <param name="identifier">
    /// <para>
    /// An object that serves as the unique identifier of the predicate.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the same predicate in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </param>
    /// <param name="arguments">The arguments of this predicate.</param>
    public Predicate(object identifier, params Term[] arguments)
        : this(identifier, (IList<Term>)arguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Predicate"/> class.
    /// </summary>
    /// <param name="identifier">
    /// <para>
    /// An object that serves as the unique identifier of the predicate.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the same predicate in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </param>
    /// <param name="arguments">The arguments of this predicate.</param>
    public Predicate(object identifier, IEnumerable<Term> arguments)
    {
        Identifier = identifier;
        Arguments = new ReadOnlyCollection<Term>(arguments.ToArray());
    }

    /// <summary>
    /// <para>
    /// Gets an object that serves as the unique identifier of the predicate.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the same predicate in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </summary>
    public object Identifier { get; }

    /// <summary>
    /// Gets the arguments of this predicate.
    /// </summary>
    public ReadOnlyCollection<Term> Arguments { get; }

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
    public override bool Equals(object? obj)
    {
        if (obj is not Predicate otherPredicate
            || !otherPredicate.Identifier.Equals(Identifier)
            || otherPredicate.Arguments.Count != Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < Arguments.Count; i++)
        {
            if (!Arguments[i].Equals(otherPredicate.Arguments[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Identifier);
        foreach (var argument in Arguments)
        {
            hashCode.Add(argument);
        }

        return hashCode.ToHashCode();
    }
}
